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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Instrumentation;
using System.Web.Profile;
using System.Web.SessionState;
using System.Web.WebSockets;
using Http.Shared.Contexts;

namespace Http.Contexts
{
	public class ListenerHttpContext : HttpContextBase, IHttpContext
	{
		public ListenerHttpContext()
		{
			RouteParams = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}

		public void ForceRootDir(string rootDir)
		{
			RootDir = rootDir;
		}
		public string RootDir { get; private set; }

		public IHttpContext Parent
		{
			get { return null; }
		}

		public IHttpContext RootContext
		{
			get
			{
				if (Parent == null) return this;
				return Parent.RootContext;
			}
		}

		public Dictionary<string, object> RouteParams { get; set; }
		public object SourceObject { get { return _httpListenerContext; } }


		private static readonly MethodInfo _getInnerCollection;
		static ListenerHttpContext()
		{
			var innerCollectionProperty = typeof(WebHeaderCollection).GetProperty("InnerCollection", BindingFlags.NonPublic | BindingFlags.Instance);
			_getInnerCollection = innerCollectionProperty.GetGetMethod(true);
		}

		public void ForceHeader(string key, string value)
		{
			var nameValueCollection = (NameValueCollection)_getInnerCollection.Invoke(_response.Headers, new object[] { });
			if (!_response.Headers.AllKeys.ToArray().Contains(key))
			{
				nameValueCollection.Add(key, value);
			}
			else
			{
				nameValueCollection.Set(key, value);
			}
		}

		private readonly HttpListenerContext _httpListenerContext;

		public ListenerHttpContext(HttpListenerContext httpListenerContext)
		{
			ContextsManager.OnOpen();
			_httpListenerContext = httpListenerContext;
			InitializeUnsettable();
		}
		public override ISubscriptionToken AddOnRequestCompleted(Action<HttpContextBase> callback)
		{
			//TODO Missing AddOnRequestCompleted for HttpListenerContext
			return null;
		}

		public override void AcceptWebSocketRequest(Func<AspNetWebSocketContext, Task> userFunc)
		{
			//TODO Missing AcceptWebSocketRequest for HttpListenerContext
		}

		public override void AcceptWebSocketRequest(Func<AspNetWebSocketContext, Task> userFunc, AspNetWebSocketOptions options)
		{
			//TODO Missing AcceptWebSocketRequest for HttpListenerContext
		}

		public override void AddError(Exception errorInfo)
		{
			if (_allErrors == null) _allErrors = new List<Exception>();
			_allErrors.Add(errorInfo);
			_error = errorInfo;
		}

		public override void ClearError()
		{
			if (_allErrors != null) _allErrors.Clear();
			_error = null;
		}

		public override ISubscriptionToken DisposeOnPipelineCompleted(IDisposable target)
		{
			//TODO Missing DisposeOnPipelineCompleted for HttpListenerContext
			return null;
		}

		public override Object GetGlobalResourceObject(String classKey, String resourceKey)
		{
			//TODO Missing GetGlobalResourceObject for HttpListenerContext
			return null;
		}

		public override Object GetGlobalResourceObject(String classKey, String resourceKey, CultureInfo culture)
		{
			//TODO Missing GetGlobalResourceObject for HttpListenerContext
			return null;
		}

		public override Object GetLocalResourceObject(String virtualPath, String resourceKey)
		{
			//TODO Missing GetLocalResourceObject for HttpListenerContext
			return null;
		}

		public override Object GetLocalResourceObject(String virtualPath, String resourceKey, CultureInfo culture)
		{
			//TODO Missing GetLocalResourceObject for HttpListenerContext
			return null;
		}

		public override Object GetSection(String sectionName)
		{
			//TODO Missing GetSection for HttpListenerContext
			return null;
		}

		public override void RemapHandler(IHttpHandler handler)
		{
			//TODO Missing RemapHandler for HttpListenerContext
		}

		public override void RewritePath(String path)
		{
			//TODO Missing RewritePath for HttpListenerContext
		}

		public override void RewritePath(String path, Boolean rebaseClientPath)
		{
			//TODO Missing RewritePath for HttpListenerContext
		}

		public override void RewritePath(String filePath, String pathInfo, String queryString)
		{
			//TODO Missing RewritePath for HttpListenerContext
		}

		public override void RewritePath(String filePath, String pathInfo, String queryString, Boolean setClientFilePath)
		{
			//TODO Missing RewritePath for HttpListenerContext
		}

		public override void SetSessionStateBehavior(SessionStateBehavior sessionStateBehavior)
		{
			//TODO Missing SetSessionStateBehavior for HttpListenerContext
		}

		public override Object GetService(Type serviceType)
		{
			//TODO Missing GetService for HttpListenerContext
			return null;
		}

		private List<Exception> _allErrors;
		public override Exception[] AllErrors { get { return _allErrors.ToArray(); } }

		public void SetAllErrors(Exception[] val)
		{
			_allErrors = new List<Exception>(val);
		}

		public override bool AllowAsyncDuringSyncStages { get; set; }


		private HttpApplicationStateBase _application;
		public override HttpApplicationStateBase Application { get { return _application; } }

		public void SetApplication(HttpApplicationStateBase val)
		{
			_application = val;
		}

		public override HttpApplication ApplicationInstance { get; set; }

		public override AsyncPreloadModeFlags AsyncPreloadMode { get; set; }


		private Cache _cache;
		public override Cache Cache { get { return _cache; } }

		public void SetCache(Cache val)
		{
			_cache = val;
		}


		private IHttpHandler _currentHandler;
		public override IHttpHandler CurrentHandler { get { return _currentHandler; } }

		public void SetCurrentHandler(IHttpHandler val)
		{
			_currentHandler = val;
		}


		private RequestNotification _currentNotification;
		public override RequestNotification CurrentNotification { get { return _currentNotification; } }

		public void SetCurrentNotification(RequestNotification val)
		{
			_currentNotification = val;
		}


		private Exception _error;
		public override Exception Error { get { return _error; } }

		public void SetError(Exception val)
		{
			_error = val;
		}

		public override IHttpHandler Handler { get; set; }


		private Boolean _isCustomErrorEnabled;
		public override Boolean IsCustomErrorEnabled { get { return _isCustomErrorEnabled; } }

		public void SetIsCustomErrorEnabled(Boolean val)
		{
			_isCustomErrorEnabled = val;
		}


		private Boolean _isDebuggingEnabled;
		public override Boolean IsDebuggingEnabled { get { return _isDebuggingEnabled; } }

		public void SetIsDebuggingEnabled(Boolean val)
		{
			_isDebuggingEnabled = val;
		}


		private Boolean _isPostNotification;
		public override Boolean IsPostNotification { get { return _isPostNotification; } }

		public void SetIsPostNotification(Boolean val)
		{
			_isPostNotification = val;
		}


		private Boolean _isWebSocketRequest;
		public override Boolean IsWebSocketRequest { get { return _isWebSocketRequest; } }

		public void SetIsWebSocketRequest(Boolean val)
		{
			_isWebSocketRequest = val;
		}


		private Boolean _isWebSocketRequestUpgrading;
		public override Boolean IsWebSocketRequestUpgrading { get { return _isWebSocketRequestUpgrading; } }

		public void SetIsWebSocketRequestUpgrading(Boolean val)
		{
			_isWebSocketRequestUpgrading = val;
		}


		private IDictionary _items;
		public override IDictionary Items { get { return _items; } }

		public void SetItems(IDictionary val)
		{
			_items = val;
		}


		private PageInstrumentationService _pageInstrumentation;
		public override PageInstrumentationService PageInstrumentation { get { return _pageInstrumentation; } }

		public void SetPageInstrumentation(PageInstrumentationService val)
		{
			_pageInstrumentation = val;
		}


		private IHttpHandler _previousHandler;
		public override IHttpHandler PreviousHandler { get { return _previousHandler; } }

		public void SetPreviousHandler(IHttpHandler val)
		{
			_previousHandler = val;
		}


		private ProfileBase _profile;
		public override ProfileBase Profile { get { return _profile; } }

		public void SetProfile(ProfileBase val)
		{
			_profile = val;
		}


		private HttpServerUtilityBase _server;
		public override HttpServerUtilityBase Server { get { return _server; } }

		public void SetServer(HttpServerUtilityBase val)
		{
			_server = val;
		}


		private HttpSessionStateBase _session = new SimpleHttpSessionState();
		public override HttpSessionStateBase Session { get { return _session; } }

		public void SetSession(HttpSessionStateBase val)
		{
			_session = val;
		}

		public override bool SkipAuthorization { get; set; }


		private DateTime _timestamp;
		public override DateTime Timestamp { get { return _timestamp; } }

		public void SetTimestamp(DateTime val)
		{
			_timestamp = val;
		}

		public override bool ThreadAbortOnTimeout { get; set; }


		private TraceContext _trace;
		public override TraceContext Trace { get { return _trace; } }

		public void SetTrace(TraceContext val)
		{
			_trace = val;
		}

		private IPrincipal _user;
		public override IPrincipal User
		{
			set
			{
				_user = value;
			}
			get
			{
				if (_user != null) return _user;
				return _httpListenerContext.User;
			}
		}


		private String _webSocketNegotiatedProtocol;
		public override String WebSocketNegotiatedProtocol { get { return _webSocketNegotiatedProtocol; } }

		public void SetWebSocketNegotiatedProtocol(String val)
		{
			_webSocketNegotiatedProtocol = val;
		}


		private IList<String> _webSocketRequestedProtocols;
		public override IList<String> WebSocketRequestedProtocols { get { return _webSocketRequestedProtocols; } }

		public void SetWebSocketRequestedProtocols(IList<String> val)
		{
			_webSocketRequestedProtocols = val;
		}


		private HttpRequestBase _request;
		private HttpResponseBase _response;
		public override HttpRequestBase Request { get { return _request; } }

		public void SetRequest(HttpRequestBase val)
		{
			_request = val;
		}


		public override HttpResponseBase Response { get { return _response; } }

		public void SetResponse(HttpResponseBase val)
		{
			_response = val;
		}

		public void InitializeUnsettable()
		{
			_request = new ListenerHttpRequest(_httpListenerContext.Request);
			_response = new ListenerHttpResponse(_httpListenerContext.Response);
			_response.ContentEncoding = _request.ContentEncoding;

			//_allErrors=_httpListenerContext.AllErrors;
			//_application=_httpListenerContext.Application;
			//_cache=_httpListenerContext.Cache;
			//_currentHandler=_httpListenerContext.CurrentHandler;
			//_currentNotification=_httpListenerContext.CurrentNotification;
			//_error=_httpListenerContext.Error;
			//_isCustomErrorEnabled=_httpListenerContext.IsCustomErrorEnabled;
			//_isDebuggingEnabled=_httpListenerContext.IsDebuggingEnabled;
			//_isPostNotification=_httpListenerContext.IsPostNotification;
			//_isWebSocketRequest=_httpListenerContext.IsWebSocketRequest;
			//_isWebSocketRequestUpgrading=_httpListenerContext.IsWebSocketRequestUpgrading;
			//_items=_httpListenerContext.Items;
			//_pageInstrumentation=_httpListenerContext.PageInstrumentation;
			//_previousHandler=_httpListenerContext.PreviousHandler;
			//_profile=_httpListenerContext.Profile;
			//_server=_httpListenerContext.Server;
			//_session=_httpListenerContext.Session;
			//_timestamp=_httpListenerContext.Timestamp;
			//_trace=_httpListenerContext.Trace;
			//_webSocketNegotiatedProtocol=_httpListenerContext.WebSocketNegotiatedProtocol;
			//_webSocketRequestedProtocols=_httpListenerContext.WebSocketRequestedProtocols;
		}

		private WebSocketContext _webSocketContext;
		private WebSocket _webSocket;
		//private WebSocketReceiveResult _webSocketReceiveResult;

		public Task InitializeWebSocket()
		{
			return Task.Run(() => _httpListenerContext.AcceptWebSocketAsync(subProtocol: null))
				.ContinueWith(ContinuationAction);
		}

		private void ContinuationAction(Task<HttpListenerWebSocketContext> task)
		{
			_webSocketContext = task.Result;
			_webSocket = _webSocketContext.WebSocket;
			//if(_webSocket.State == WebSocketState.)
		}

		/*private async void ProcessRequest(HttpListenerContext httpListenerContext, Webs)
		{
		}*/
	}
}