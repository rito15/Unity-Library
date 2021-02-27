#pragma warning disable CS1591

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-26 AM 5:10:04
// 작성자 : Rito

namespace Rito.UnityLibrary.Extension
{
    public static class DebugExtension
    {
        public static void DebugLog<T>(this T @this)
        {
            Rito.UnityLibrary.Debug.Log(@this);
        }
    }
}