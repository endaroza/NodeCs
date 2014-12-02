﻿#region  Microsoft Public License
/* This code is part of Xipton.Razor v3.0
 * (c) Jaap Lamfers, 2013 - jaap.lamfers@xipton.net
 * Licensed under the Microsoft Public License (MS-PL) http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 */
#endregion

using System;

namespace Http.Renderer.Razor.Utils
{
    public static class TypeExtension
    {
        public static object CreateInstance(this Type type)
        {
					Console.WriteLine("REMOVE ACTIVATOR");
            return type == null ? null : Activator.CreateInstance(type, true);
        }

        public static T CreateInstance<T>(this Type type) {
					Console.WriteLine("REMOVE ACTIVATOR");
            return type == null ? default(T) : (T)Activator.CreateInstance(type, true);
        }
    }
}