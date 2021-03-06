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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Http.Shared.Authorization
{
    public static class AuthenticationType
    {
        public static string None { get { return "none"; } }
        public static string Basic { get { return "basic"; } }
        public static string Form { get { return "form"; } }
        public static string Digest { get { return "digest"; } }
    }
    public class SecurityDefinition
    {
        public SecurityDefinition()
        {
            AuthenticationType = Http.Shared.Authorization.AuthenticationType.None;
            Realm = "Node.Cs";
            LoginPage = "~/";
        }
        public string AuthenticationType { get; set; }
        public string Realm { get; set; }
        public string LoginPage { get; set; }
    }
}
