#pragma warning disable CS1591

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-28 AM 4:46:07
// 작성자 : Rito

namespace Rito.UnityLibrary.Extension
{
    public static class TransformExtension
    {
        /***********************************************************************
        *                       Transform Array Extensions
        ***********************************************************************/
        #region .
        
        /// <summary> 모두 같은 부모 아래 있는지 검사 </summary>
        public static bool Ex_HasSameParent(this Transform[] transforms)
        {
            List<string> parentIdList = new List<string>();
            foreach (var activeTr in transforms)
            {
                string id = "None";
                if (activeTr.parent != null) id = activeTr.parent.GetInstanceID().ToString();
                if (!parentIdList.Contains(id)) parentIdList.Add(id);
            }

            return parentIdList.Count <= 1;
        }

        #endregion
    }
}