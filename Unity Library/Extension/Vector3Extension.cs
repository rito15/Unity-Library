#pragma warning disable CS1591

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-26 AM 12:51:10
// 작성자 : Rito

namespace Rito.UnityLibrary.Extension
{
    public static class Vector3Extension
    {
        /***********************************************************************
        *                               Calculations
        ***********************************************************************/
        #region .
        /// <summary> (x1 * x2, y1 * y2, z1 * z2) </summary>
        public static Vector3 Ex_Multiply(in this Vector3 @this, in Vector3 other)
            => new Vector3(@this.x * other.x, @this.y * other.y, @this.z * other.z);

        /// <summary> (x1 / x2, y1 / y2, z1 / z2) </summary>
        public static Vector3 Ex_Divide(in this Vector3 @this, in Vector3 other)
            => new Vector3(@this.x / other.x, @this.y / other.y, @this.z / other.z);

        #endregion
        /***********************************************************************
        *                               Setters
        ***********************************************************************/
        #region .
        public static Vector3 Ex_SetX(in this Vector3 @this, float x)
            => new Vector3(x, @this.y, @this.z);
        public static Vector3 Ex_SetY(in this Vector3 @this, float y)
            => new Vector3(@this.x, y, @this.z);
        public static Vector3 Ex_SetZ(in this Vector3 @this, float z)
            => new Vector3(@this.x, @this.y, z);

        public static Vector3 Ex_SetXY(in this Vector3 @this, float x, float y)
            => new Vector3(x, y, @this.z);
        public static Vector3 Ex_SetYZ(in this Vector3 @this, float y, float z)
            => new Vector3(@this.x, y, z);
        public static Vector3 Ex_SetXZ(in this Vector3 @this, float x, float z)
            => new Vector3(x, @this.y, z);

        #endregion
    }
}