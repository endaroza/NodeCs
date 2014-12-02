﻿#region  Microsoft Public License
/* This code is part of Xipton.Razor v3.0
 * (c) Jaap Lamfers, 2013 - jaap.lamfers@xipton.net
 * Licensed under the Microsoft Public License (MS-PL) http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 */
#endregion

using System;

namespace Http.Renderer.Razor.Utils
{
    public static class ObjectExtension
    {
        public static T CastTo<T>(this object target)
        {
            return target == null ? default(T) : (T) target;
        }

        public static bool TryDispose(this object target)
        {
            var disposable = target as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
                return true;
            }
            return false;
        }

    }
}
