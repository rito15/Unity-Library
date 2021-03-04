#pragma warning disable CS1591

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-26 AM 5:10:04
// 작성자 : Rito

namespace Rito.UnityLibrary.Extension
{
    using static GlobalVariables;
    using Debug = Rito.UnityLibrary.Debug;

    public static class DebugExtension
    {
        /***********************************************************************
        *                               Log
        ***********************************************************************/
        #region .
        [System.Diagnostics.Conditional(ConditionalDebugKeyword)]
        public static void Ex_DebugLog<T>(this T @this, LogType logType = LogType.Log)
            => Debug.Log(@this, logType);

        [System.Diagnostics.Conditional(ConditionalDebugKeyword)]
        public static void Ex_DebugLog<T>(this T @this, string preString, LogType logType = LogType.Log)
            => Debug.Log(preString + @this, logType);

        #endregion
        /***********************************************************************
        *                               Assert
        ***********************************************************************/
        #region .
        /// <summary>
        /// 두 값이 같은지 여부를 콘솔에 출력
        /// </summary>
        /// <param name="printOnlyFailed">Assert 실패 시에만 로그를 출력할지 여부</param>
        /// <param name="failLogType">Assert 실패 시 출력할 로그의 타입</param>
        [System.Diagnostics.Conditional(ConditionalDebugKeyword)]
        public static void Ex_AssertLog<T>
            (this T @this, T other, bool printOnlyFailed = true, LogType failLogType = LogType.Error)
        {
            bool isEqual = @this.Equals(other);
            if (isEqual && printOnlyFailed) return;

            string result = $"ASSERT : {isEqual} [{(isEqual ? $"{@this}" : $"{@this}, {other}")}]";

            if (isEqual)
                Debug.Log(result);
            else
                Debug.Log(result, failLogType);
        }

        #endregion

    }
}