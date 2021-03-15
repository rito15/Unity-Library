using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-03-14 PM 7:54:51
// 작성자 : Rito

namespace Rito.UnityLibrary.Extensions
{
    public static class GameObjectExtension
    {
        public static T GetOrAddComponent<T>(GameObject @this)
            where T : Component
        {
            var component = @this.GetComponent<T>();
            if (component == null)
            {
                component = @this.AddComponent<T>();
            }

            return component;
        }
    }
}