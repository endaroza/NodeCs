// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Routing;
using CoroutinesLib.Shared;
using GenericHelpers;
using Http.Contexts;
using Http.Renderer.Razor.Helpers;
using Http.Renderer.Razor.Utils;
using Http.Shared.Contexts;
using Http.Shared.Controllers;
using Http.Shared.Routing;
using HttpMvc.Controllers;
using NodeCs.Shared;

namespace Http.Renderer.Razor.Integration
{
	public class RazorTemplateGenerator : IRazorTemplateGenerator
	{
		private readonly ConcurrentDictionary<string, RazorTemplateEntry> _templateItems =
			new ConcurrentDictionary<string, RazorTemplateEntry>();



		public void RegisterTemplate(string templateString, string templateName)
		{
			Type templateType = ModelTypeFromTemplate(ref templateString);
			RegisterTemplate(templateName, templateString, templateType);
		}

		private Type ModelTypeFromTemplate(ref string templateString)
		{
			var splittedTemplate = templateString
				.Split(new[] { '\r', '\f', '\n' }).ToList();

			var modelIndex = -1;
			for (int index = 0; index < splittedTemplate.Count; index++)
			{
				var item = splittedTemplate[index];
				var trimmed = item.Trim();
				if (trimmed.StartsWith("@model", StringComparison.InvariantCultureIgnoreCase))
				{
					modelIndex = index;
					break;
				}
			}
			if (modelIndex == -1) return null;
			var modelString = splittedTemplate[modelIndex];
			splittedTemplate.RemoveAt(modelIndex);
			templateString = string.Join("\r\n", splittedTemplate);
			modelString = modelString.Substring("@model ".Length);
			return AssembliesManager.LoadType(modelString);
		}

		private void RegisterTemplate(string templateName, string templateString, Type modelType)
		{
			if (templateName == null)
				throw new ArgumentNullException("templateName");
			if (templateString == null)
				throw new ArgumentNullException("templateString");

			_templateItems[templateName] = new RazorTemplateEntry()
			{
				ModelType = modelType ?? typeof(object),
				TemplateString = templateString,
				TemplateName = "Rzr" + Guid.NewGuid().ToString("N"),
				IsNoModel = modelType == null
			};
		}

		public void CompileTemplates()
		{
			Compiler.Compile(_templateItems.Values);
		}

		private static readonly object _locker = new object();

		public IEnumerable<BufferItem> GenerateOutput(object model, string templateName, IHttpContext context,
			ModelStateDictionary modelStateDictionary,object viewBagPassed)
		{
			var viewBag = viewBagPassed as dynamic;
			if (templateName == null)
				throw new ArgumentNullException("templateName");

			RazorTemplateEntry entry = null;
			try
			{
				entry = _templateItems[templateName];
			}
			catch (KeyNotFoundException)
			{
				throw new ArgumentOutOfRangeException("No template has been registered under this model or name.");
			}
			var templateItem = _templateItems[templateName];
			if (templateItem.TemplateType == null)
			{
				lock (_locker)
				{
					templateItem.TemplateType =
						AssembliesManager.LoadType("Http.Renderer.Razor.Integration." + entry.TemplateName + "Template");
				}
			}
			var type = templateItem.TemplateType;
			var template = (RazorTemplateBase)Activator.CreateInstance(type);
			InjectData(template, type, context, modelStateDictionary, viewBag);

			template.ObjectModel = model;
			template.Execute();
			var layout = template.Layout;

			if (!string.IsNullOrWhiteSpace(layout))
			{
				viewBag.ChildBuffer = string.Join("",template.Buffer.Select(a=>a.Value.ToString()));
				return new List<BufferItem>{ RenderLayout(layout, context, viewBag)};
			}

			return template.Buffer;
		}

		public BufferItem RenderLayout(string name, IHttpContext mainContext, dynamic viewBag)
		{
			var http = ServiceLocator.Locator.Resolve<HttpModule>();

			var context = new WrappedHttpContext(mainContext);

			var newUrl = name.TrimStart('~');

			((IHttpRequest)context.Request).SetUrl(new Uri(newUrl, UriKind.Relative));
			((IHttpRequest)context.Request).SetQueryString(mainContext.Request.QueryString);
			var internalCoroutine = http.SetupInternalRequestCoroutine(context, null, viewBag);
			var task = CoroutineResult.WaitForCoroutine(internalCoroutine);
			//task.Wait();
			var stream = context.Response.OutputStream as MemoryStream;

			// ReSharper disable once PossibleNullReferenceException
			var result = Encoding.UTF8.GetString(stream.ToArray());
			return new BufferItem{Value = result};
		}

		private void InjectData(RazorTemplateBase template, Type type, IHttpContext context, 
			ModelStateDictionary modelStateDictionary, object viewBag)
		{
			string layout = null;
			var routeHandler = ServiceLocator.Locator.Resolve<IRoutingHandler>();
			var properties = type.GetProperties();
			var model = properties.FirstOrDefault(p => p.Name == "Model");

			var ga = type.GetGenericArguments();
			if (ga.Length == 0)
			{
				ga = new[] { model != null ? model.PropertyType : typeof(object) };
			}
			foreach (var property in type.GetProperties())
			{
				if (!property.CanWrite) continue;
				switch (property.Name)
				{
					case ("Html"):
						var urlHelper = typeof(HtmlHelper<>).MakeGenericType(ga);
						//public HtmlHelper(IHttpContext context, ViewContext viewContext,
						//	object nodeCsTemplateBase, string localPath, dynamic viewBag,IRoutingHandler routingHandler)
						property.SetValue(template, Activator.CreateInstance(urlHelper,
							new object[]
							{
								context,new ViewContext(context,template,modelStateDictionary),
								template,context==null?string.Empty:context.Request.Url.ToString(),new ExpandoObject(),routeHandler
							}
							));
						break;
					case ("Url"):
						property.SetValue(template, new UrlHelper(context, routeHandler));
						break;
					case ("Context"):
						property.SetValue(template, context);
						break;
					case ("ViewBag"):
						property.SetValue(template, viewBag??new ExpandoObject());
						break;
					default:
						var value = ServiceLocator.Locator.Resolve(property.PropertyType);
						if (value != null)
						{
							property.SetValue(template, value);
						}
						break;
				}
			}
		}

		public string GenerateOutputString(object model, string templateName, IHttpContext context, ModelStateDictionary modelStateDictionary,object viewBag)
		{
			var output = new StringBuilder();
			foreach (var item in GenerateOutput(model, templateName, context, modelStateDictionary,viewBag))
			{
				output.Append(item.Value);
			}
			return output.ToString();
		}

	}
}
