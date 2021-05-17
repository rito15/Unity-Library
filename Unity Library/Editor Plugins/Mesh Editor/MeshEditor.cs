#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 날짜 : 2021-05-16 PM 3:34:18
// 작성자 : Rito

namespace Rito.UnityLibrary.EditorPlugins
{
    [DisallowMultipleComponent]
    public partial class MeshEditor : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        [SerializeField] private Vector3 pivotPos = Vector3.zero;
        [SerializeField] private bool editMode;
        [SerializeField] private bool pivotEditMode;
        [SerializeField] private bool snapMode;
        [SerializeField] private float snapValue = 0.1f;

        // T O D O : 트랜스폼의 회전, 스케일 변경에 따라
        //           바운드 기즈모 영역 맞추기

        [SerializeField] private bool showBounds;
        [SerializeField] private bool confineInBounds;


        private const int ContextPriority = 100;

        [MenuItem("CONTEXT/MeshFilter/Edit Mesh", false, ContextPriority)]
        private static void Context_AddMeshEditor(MenuCommand mc)
        {
            var component = mc.context as Component;
            var me = component.gameObject.AddComponent<MeshEditor>();
            PutComponentOnTop(me);
        }

        [MenuItem("CONTEXT/MeshFilter/Edit Mesh", true, ContextPriority)]
        private static bool Context_AddMeshEditor_Validate(MenuCommand mc)
        {
            var component = mc.context as Component;
            MeshEditor me = component.GetComponent<MeshEditor>();
            return me == null;
        }

        /// <summary> 컴포넌트를 최상단에 올리기 </summary>
        private static void PutComponentOnTop(Component component)
        {
            for (int i = 0; i < 100 && UnityEditorInternal.ComponentUtility.MoveComponentUp(component); i++);
        }
    }
}

#endif