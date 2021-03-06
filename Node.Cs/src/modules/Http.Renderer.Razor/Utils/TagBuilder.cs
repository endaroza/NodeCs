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


using System.Collections.Generic;
using System.Text;

namespace Http.Renderer.Razor.Utils
{
	public static class TagBuilder
	{
		public static string StartTag(string tagname, Dictionary<string, object> attributes = null, bool selfClosed = false)
		{
			var strbu = new StringBuilder();
			strbu.Append("<")
				.Append(tagname)
				.Append(" ");

			if (attributes != null)
			{
				foreach (var kvp in attributes)
				{
					strbu.Append(" ")
						.Append(kvp.Key);
					if (kvp.Value != null)
					{
						strbu.Append("=\"")
							.Append(kvp.Value.ToString().Replace("\"", "\\\""))
							.Append("\"");
					}
				}
			}
			if (selfClosed)
			{
				strbu.Append("/>");
				return strbu.ToString();
			}

			strbu.Append(">");
			return strbu.ToString();
		}

		public static string EndTag(string tagname)
		{
			return "</" + tagname + ">";
		}

		public static string TagWithValue(string tagname, string value, Dictionary<string, object> attributes)
		{
			return StartTag(tagname, attributes) + value + EndTag(tagname);
		}
	}
}
