#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-03-14 AM 2:03:58
// 작성자 : Rito

namespace Rito.UnityLibrary.Editor
{
    /// <summary> 영역 내에서 컨텐츠 색상, 배경 색상을 지정한다.
    /// <para/> null로 지정한 색상은 영향을 주지 않는다.
    /// </summary>
    public class ColorScope : GUI.Scope
    {
        private readonly Color? originalContentColor;
        private readonly Color? originalBackgroundColor;

        public ColorScope(Color? contentColor, Color? backgroundColor)
        {
            if (contentColor != null)
            {
                originalContentColor = GUI.contentColor;
                GUI.contentColor = contentColor.Value;
            }
            else
                originalContentColor = null;

            if (backgroundColor != null)
            {
                originalBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = backgroundColor.Value;
            }
            else
                originalBackgroundColor = null;
        }

        protected override void CloseScope()
        {
            if(originalContentColor != null)
                GUI.contentColor = originalContentColor.Value;

            if (originalBackgroundColor != null)
                GUI.backgroundColor = originalBackgroundColor.Value;
        }
    }
}
#endif