/*
 * Copyright(c) 2017 Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;
using System.Reflection;

namespace BardMusicPlayer.Config.JsonSettings.Inline
{
    internal static class ReflectionHelpers
    {
        public static bool IsValueType(Type targetType)
        {
            if (targetType == null)
            {
                throw new NullReferenceException("Must supply the targetType parameter");
            }
            return targetType.GetTypeInfo().IsValueType;
        }
    }
}