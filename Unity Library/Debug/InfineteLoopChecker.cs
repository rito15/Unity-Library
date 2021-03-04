using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-03-05 AM 4:11:57
// 작성자 : Rito

namespace Rito.UnityLibrary
{
    /// <summary> 무한 루프 검사 </summary>
    public static class InfiniteLoopChecker
    {
        private static int infiniteLoopNum = 0;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Reset() => infiniteLoopNum = 0;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Check(int maxLoopNumber = 10000)
        {
            if(infiniteLoopNum++ > maxLoopNumber)
                throw new Exception("Infinite Loop Detected.");
        }
    }
}