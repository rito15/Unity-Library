using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Rito.UnityLibrary.Extension;

// 날짜 : 2021-02-27 PM 11:16:42
// 작성자 : Rito

namespace Rito.UnityLibrary.Editor
{
    public static class MenuItems
    {
        /***********************************************************************
        *                               Validation Check
        ***********************************************************************/
        #region .

        /// <summary> 지금 하이라키 윈도우가 활성화되었는지 검사 </summary>
        private static bool IsFocusedOnHierarchyWindow()
        {
#pragma warning disable CS0618 // focusedWindow.title : Obsolete
            return EditorWindow.focusedWindow.title.Equals("Hierarchy");
#pragma warning restore CS0618
        }

        /// <summary> 지금 씬 윈도우가 활성화되었는지 검사 </summary>
        private static bool IsFocusedOnSceneWindow()
        {
#pragma warning disable CS0618 // focusedWindow.title : Obsolete
            return EditorWindow.focusedWindow.title.Equals("Scene");
#pragma warning restore CS0618
        }

        /// <summary> 하이라키 내의 게임오브젝트를 선택한 상황인지 검사 </summary>
        private static bool IsGameObjectInHierarchySelected()
        {
            if (!IsFocusedOnHierarchyWindow() && !IsFocusedOnSceneWindow())
                return false;

            if (Selection.activeGameObject == null)
            {
                EditorUtility.DisplayDialog($"Rito", "게임오브젝트를 선택하세요", "OK");
                return false;
            }
            return true;
        }

        /// <summary> 해당 게임오브젝트에 동일 컴포넌트가 존재하는지 검사 </summary>
        private static bool IsNotDuplicatedComponent<T>(string componentName = "") where T : Component
        {
            if (IsGameObjectInHierarchySelected() == false)
                return false;

            var existed = Selection.activeGameObject.GetComponent<T>();
            if (existed != null)
            {
                EditorUtility.DisplayDialog("Rito", $"해당 게임오브젝트에 " +
                    $"{(componentName.Length > 0 ? componentName : typeof(T).Ex_ToStringSimple())} 컴포넌트가 이미 존재합니다.", "OK");
                return false;
            }

            return true;
        }

        /// <summary> 해당 게임오브젝트에 동일 컴포넌트가 존재하는지 검사 </summary>
        private static bool IsNotDuplicatedComponent(System.Type type, string componentName = "")
        {
            if (IsGameObjectInHierarchySelected() == false)
                return false;

            var existed = Selection.activeGameObject.GetComponent(type);
            if (existed != null)
            {
                EditorUtility.DisplayDialog("Rito", $"해당 게임오브젝트에 " +
                    $"{(componentName.Length > 0 ? componentName : type.Ex_ToStringSimple())} 컴포넌트가 이미 존재합니다.", "OK");
                return false;
            }

            return true;
        }

        /// <summary> 같은 부모를 공유하는지 검사 </summary>
        private static bool IsAllShareSameParent(Transform[] transforms)
        {
            List<Transform> parentList = new List<Transform>();
            foreach (var activeTr in transforms)
            {
                if (parentList.Contains(activeTr.parent) == false)
                    parentList.Add(activeTr.parent);
            }

            if (parentList.Count > 1)
            {
                EditorUtility.DisplayDialog($"Rito", "게임오브젝트들의 부모가 동일해야 합니다.", "OK");
                return false;
            }

            return true;
        }


        private static string _prevMethodCallInfo = "";

        /// <summary> 같은 메소드가 이미 실행됐었는지 검사 (중복 메소드 호출 제한용) </summary>
        private static bool IsPrevSameMethodCalled([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            string info = memberName + DateTime.Now.ToString();

            if (_prevMethodCallInfo.Equals(info))
            {
                return true;
            }
            else
            {
                _prevMethodCallInfo = info;
                return false;
            }
        }

        #endregion
        /***********************************************************************
        *                       MenuItem : GameObject/Rito
        ***********************************************************************/
        #region .

        [MenuItem("GameObject/Rito/공통 부모 게임오브젝트로 묶기", priority = -100)]
        private static void GameObject_GroupAsCommonEmptyParent()
        {
            if (IsPrevSameMethodCalled()) return;
            if (!IsGameObjectInHierarchySelected()) return;

            var len = Selection.transforms.Length;
            var name = Selection.activeObject.name;

            Debug.Log("Hi");
        }

        #endregion

    }
}