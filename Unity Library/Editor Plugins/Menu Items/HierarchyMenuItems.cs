#if UNITY_EDITOR

using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Rito.UnityLibrary.Extensions;

using Object = UnityEngine.Object;

// 날짜 : 2021-02-27 PM 11:16:42
// 작성자 : Rito

namespace Rito.UnityLibrary.EditorPlugins
{
    public static class HierarchyMenuItems
    {
        /***********************************************************************
        *                           Definitions
        ***********************************************************************/
        #region .
        [Flags]
        private enum EditorWindowType
        {
            Scene = 1,
            Game  = 2,
            Inspector = 4,
            Hierarchy = 8,
            Project   = 16,
            Console   = 32
        }

        #endregion
        /***********************************************************************
        *                           Validation Check
        ***********************************************************************/
        #region .

        private static string _prevMethodCallInfo = "";

        /// <summary> 같은 메소드가 이미 실행됐었는지 검사 (중복 메소드 호출 제한용) </summary>
        private static bool IsDuplicatedMethodCall([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
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

        /// <summary> 현재 활성화된 윈도우 타입 검사 (OR 연산으로 다중 검사 가능) </summary>
        private static bool CheckFocusedWindow(EditorWindowType type)
        {
            string currentWindowTitle = EditorWindow.focusedWindow.titleContent.text;
            var enumElements = Enum.GetValues(typeof(EditorWindowType)).Cast<EditorWindowType>();

            foreach (var item in enumElements)
            {
                if((type & item) != 0 && item.ToString() == currentWindowTitle)
                    return true;
            }

            return false;
        }

        /// <summary> 하이라키 내의 게임오브젝트를 선택한 상황인지 검사 </summary>
        private static bool IsGameObjectInHierarchySelected()
        {
            if (!CheckFocusedWindow(EditorWindowType.Hierarchy | EditorWindowType.Scene))
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

        #endregion
        /***********************************************************************
        *                           Utility Properties
        ***********************************************************************/
        #region .
        /// <summary> 현재 선택된 모든 트랜스폼을 필터링 안하고 그대로 가져오기 </summary>
        private static Transform[] SelectedAllTransforms => Selection.GetTransforms(SelectionMode.Unfiltered);

        /// <summary> 현재 선택된 트랜스폼들 중 계층 관계에 있는 것들은 최상위 부모만 필터링하여 가져오기 </summary>
        private static Transform[] SelectedTopLevelTransforms => Selection.GetTransforms(SelectionMode.TopLevel);

        #endregion
        /***********************************************************************
        *                           Utility Methods
        ***********************************************************************/
        #region .
        /// <summary> 메시지와 함께 "OK" 버튼만 존재하는 알림창 표시 </summary>
        private static bool DisplayRitoAlertDialog(in string msg)
            => EditorUtility.DisplayDialog("Rito", msg, "OK");

        /// <summary> 하이라키의 특정 게임오브젝트 선택 </summary>
        private static void SelectGameObject(GameObject go) => Selection.activeGameObject = go;

        /// <summary> 하이라키의 특정 트랜스폼 선택 </summary>
        private static void SelectTransform(Transform tr) => Selection.activeTransform = tr;

        /// <summary> 특정 윈도우 선택 </summary>
        private static void FocusOnWindow(EditorWindowType windowType)
        {
            EditorApplication.ExecuteMenuItem("Window/General/" + windowType.ToString());
        }

        /// <summary> 현재 선택된 윈도우에 특정 키 이벤트 발생시키기 </summary>
        private static void InvokeKeyEventOnFocusedWindow(KeyCode key, EventType eventType)
        {
            var keyEvent = new Event { keyCode = key, type = eventType };
            EditorWindow.focusedWindow.SendEvent(keyEvent);
        }

        #endregion
        /***********************************************************************
        *                           MenuItem : GameObject/Rito
        ***********************************************************************/
        #region .

        private const string GameObject_Rito_ = "GameObject/Rito/";
        private const int PriorityBegin = -1000;

        /*[MenuItem(GameObject_Rito_ + "Test", priority = PriorityBegin - 999)]
        private static void TEST()
        {
            if (IsDuplicatedMethodCall()) return;
            //if (!IsGameObjectInHierarchySelected()) return;

            CheckFocusedWindow(EditorWindowType.Game | EditorWindowType.Console).DebugLog();
        }*/

        /// <summary> 선택된 게임오브젝트들을 하나의 부모 게임오브젝트로 묶기  </summary>
        [MenuItem(GameObject_Rito_ + "Group", priority = PriorityBegin)]
        private static void GameObject_GroupAsCommonEmptyParent()
        {
            if (IsDuplicatedMethodCall()) return;
            if (!IsGameObjectInHierarchySelected()) return;
            if (!SelectedTopLevelTransforms.Ex_HasSameParent())
            {
                Debug.LogError("부모가 같은 게임오브젝트들을 선택해야 합니다.");
                return;
            }

            var selectedTransforms = Selection.transforms.OrderBy(tr => tr.GetSiblingIndex());

            // 선택된 트랜스폼들의 Sibling Index 중에 최솟값 찾기
            int sbIndex = selectedTransforms.Select(tr => tr.GetSiblingIndex()).Min();
            Transform prevParentTr = selectedTransforms.First().parent;

            // Root 게임오브젝트 생성
            Transform RootTr = new GameObject("Group").transform;
            Undo.RegisterCreatedObjectUndo(RootTr.gameObject, "Create Common Empty Parent");

            // 선택된 게임오브젝트들의 부모를 Root로 지정
            foreach (var tr in selectedTransforms)
            {
                Undo.SetTransformParent(tr, RootTr, "Group As Common Empty Parent");
            }

            if (prevParentTr != null)
            {
                Undo.SetTransformParent(RootTr, prevParentTr, "Change Root Parent");
                //RootTr.SetParent(prevParentTr);
            }

            // Root의 Sibling Index 설정
            Undo.RecordObject(RootTr, "Change Sibling Index");
            RootTr.SetSiblingIndex(sbIndex);

            // Root 선택하고 펼치기
            FocusOnWindow(EditorWindowType.Hierarchy);
            SelectTransform(RootTr);
            InvokeKeyEventOnFocusedWindow(KeyCode.RightArrow, EventType.KeyDown);
            InvokeKeyEventOnFocusedWindow(KeyCode.RightArrow, EventType.KeyDown); // 두번 해야 함
        }

        /// <summary> 자식들을 제거하지 않고 선택된 게임오브젝트만 제거 </summary>
        [MenuItem(GameObject_Rito_ + "Remove This Only", priority = PriorityBegin + 1)]
        private static void GameObject_RemoveThisOnly()
        {
            if (IsDuplicatedMethodCall()) return;
            if (!IsGameObjectInHierarchySelected()) return;
            if (SelectedAllTransforms.Length > 1)
            {
                Debug.LogError("하나의 게임오브젝트만 선택해야 합니다.");
                return;
            }

            Transform selected = Selection.activeTransform;
            int childCount = selected.childCount;
            List<Transform> childTrList = null;

            // 자식이 없는 경우 그냥 제거

            // 자식이 있는 경우
            if (childCount > 0)
            {
                Transform parent = selected.parent;

                // Sibling Index 저장
                int sbIndex = selected.GetSiblingIndex();

                // 꼬이지 않게 자식 목록 임시 저장
                childTrList = new List<Transform>();
                foreach (var child in selected)
                {
                    childTrList.Add(child as Transform);
                }
                // 부모 인계
                foreach (var child in childTrList)
                {
                    Undo.SetTransformParent(child, parent == null ? null : parent, "Change Parent");
                    child.SetSiblingIndex(sbIndex++);
                }
            }

            // 파괴
            Undo.DestroyObjectImmediate(selected.gameObject);

            // 자식들 선택
            if (childCount > 0)
            {
                Selection.objects = childTrList.Select(tr => tr.gameObject).ToArray();
            }
        }

        /// <summary> 같은 이름으로 바꾸기 </summary>
        [MenuItem(GameObject_Rito_ + "Rename (Same)", priority = PriorityBegin + 1)]
        private static void GameObject_RenameWithTheSame()
        {
            if (IsDuplicatedMethodCall()) return;
            if (!IsGameObjectInHierarchySelected()) return;
            if (Selection.gameObjects.Length <= 1) return; // 하나만 선택한 경우 안함

            // * 부모가 같지 않아도, 하이라키의 맨 위를 기준으로 이름 변경

            // 하이라키의 위에서부터 아래로 정렬
            var selectedGameObjects = 
                Selection.gameObjects
                .OrderBy(go => go.transform.GetSiblingIndex())
                .OrderBy(go => go.transform.Ex_GetDepth())
                .OrderBy(go => go.transform.root.GetSiblingIndex())
                .ToArray();

            for (int i = 1; i < selectedGameObjects.Length; i++)
            {
                Undo.RecordObject(selectedGameObjects[i], "Rename GameObject");
                selectedGameObjects[i].name = selectedGameObjects[0].name;
            }
        }

        /// <summary> 연속된 이름으로 바꾸기 (인덱스 유지) </summary>
        [MenuItem(GameObject_Rito_ + "Rename (Continuous)", priority = PriorityBegin + 1)]
        private static void GameObject_RenameWithTheContinuous()
        {
            if (IsDuplicatedMethodCall()) return;
            if (!IsGameObjectInHierarchySelected()) return;
            if (Selection.gameObjects.Length <= 1) return; // 하나만 선택한 경우 안함
            if (!SelectedTopLevelTransforms.Ex_HasSameParent())
            {
                Debug.LogError("부모가 같은 게임오브젝트들을 선택해야 합니다.");
                return;
            }

            // Sibling Index로 정렬
            var selectedGameObjects = Selection.gameObjects.OrderBy(go => go.transform.GetSiblingIndex()).ToArray();

            // Regex 이용
            string strInput = selectedGameObjects[0].name;
            string pattern = @"([\s\[\{\(\<]+)" + @"([0-9]+)" + @"([\s\]\}\)\>]+)$";
            //string replaceArea = @"$1X$3";
            string replaceNumber = @"$2";

            Regex regex = new Regex(pattern, RegexOptions.RightToLeft);

            // 문자열 내에 인덱스
            bool isMatched = regex.IsMatch(strInput);
            int index = 0;
            if (isMatched)
            {
                int.TryParse(regex.Match(strInput).Result(replaceNumber), out index);
            }

            string nameBase = selectedGameObjects[0].name;

            // 1. 이름 뒷부분에 숫자가 없는 경우 : "이름 [0]" 꼴
            if (!isMatched)
            {
                foreach (var go in selectedGameObjects)
                {
                    Undo.RecordObject(go, "Rename GameObject");
                    go.name = $"{nameBase} [{index++}]";
                }
            }
            // 2. 이름 뒷부분에 숫자가 존재하는 경우 : 현재 숫자와 특수문자꼴 유지하면서 차례로 변경
            else
            {
                foreach (var go in selectedGameObjects)
                {
                    Undo.RecordObject(go, "Rename GameObject");
                    go.name = regex.Replace(nameBase, @"$1") + (index++) + regex.Match(nameBase).Result(@"$3");
                }
            }
        }

        #endregion

    }
}

#endif