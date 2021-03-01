using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-03-01 PM 3:39:51
// 작성자 : Rito
// UnityEngine.Mathf, System.Math 래핑 (성능 개선)

/*
    [성능 테스트 기록]
    * 각각 호출 횟수 : 5,000,000

    2021. 03. 01.
    -   (x % y)            => 299
    -   Mathf.Repeat(x, y) => 300
    - MathLib.Repeat(x, y) => 245
    - MathLib.Fmod(x, y)   => 245

    -   Mathf.Acos(x) => 82
    - MathLib.Acos(x) => 68

    -   Mathf.Cos(x) => 220
    - MathLib.Cos(x) => 213

    -   Mathf.Sin(x) => 221
    - MathLib.Sin(x) => 213

    -   Mathf.Approximately(x, y) => 464
    - MathLib.Approximately(x, y) => 160

    -   Mathf.Exp(x) => 121
    - MathLib.Exp(x) => 112

    -   Mathf.Abs(x) => 172
    - MathLib.Abs(x) => 66

    -   Mathf.Clamp(x, 0, 5) => 60
    - MathLib.Clamp(x, 0, 5) => 45
*/

namespace Rito.UnityLibrary
{
    public static class MathLib
    {
        /***********************************************************************
        *                           Mathf Internal Calls
        ***********************************************************************/
        #region .
        /*
            public static int ClosestPowerOfTwo(int value);
            public static Color CorrelatedColorTemperatureToRGB(float kelvin);
            
        */

        #endregion
        /***********************************************************************
        *                               Same as Mahtf
        ***********************************************************************/
        #region .
        public const float PI = 3.14159274F;
        public const float Infinity = float.PositiveInfinity;
        public const float NegativeInfinity = float.NegativeInfinity;
        public const float Deg2Rad = 0.0174532924F;
        public const float Rad2Deg = 57.29578F;

        // Epsilon : Mathf.Epsilon 사용

        public static bool Approximately(float a, float b)
            => Abs(b - a) < Max(1E-06f * Max(Abs(a), Abs(b)), Mathf.Epsilon * 8f);
        public static float Acos(float f) => (float)Math.Acos(f);
        public static float Asin(float f) => (float)Math.Asin(f);
        public static float Atan(float f) => (float)Math.Atan(f);
        public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);
        public static float Ceil(float f) => (float)Math.Ceiling(f);
        public static int CeilToInt(float f) => (int)Math.Ceiling(f);
        public static float Clamp01(float value)
        {
            if (value < 0f) return 0f;
            if (value > 1f) return 1f;
            return value;
        }
        public static float Cos(float f) => (float)Math.Cos(f);

        /// <summary> (target - current) % 360, 결과는 -180 ~ 180 </summary>
        public static float DeltaAngle(float current, float target)
        {
            float num = Repeat(target - current, 360f);
            if (num > 180f) num -= 360f;
            return num;
        }

        public static float Exp(float power) => (float)Math.Exp(power);

        public static float Floor(float f) => (float)Math.Floor(f);
        public static int FloorToInt(float f) => (int)Math.Floor(f);


        public static float Max(float a, float b) => (a > b) ? a : b;
        public static int Max(int a, int b) => (a > b) ? a : b;
        public static float Min(float a, float b) => (a < b) ? a : b;
        public static int Min(int a, int b) => (a < b) ? a : b;

        /// <summary> t % length </summary>
        public static float Repeat(float t, float length)
            => Clamp(t - Floor(t / length) * length, 0f, length);

        public static float Sin(float f) => (float)Math.Sin(f);

        #endregion
        /***********************************************************************
        *                           Improvements, Additions
        ***********************************************************************/
        #region .
        public static float Abs(float f) => f >= 0 ? f : -f;
        public static int Abs(int f) => f >= 0 ? f : -f;

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static float Saturate(float value)
        {
            if (value < 0f) return 0f;
            if (value > 1f) return 1f;
            return value;
        }

        /// <summary> x % y </summary>
        public static float Fmod(float x, float y)
            => Clamp(x - Floor(x / y) * y, 0f, y);

        #endregion
    }
}