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
        private MeshFilter meshFilter;
        private Mesh mesh;

        private Vector3 pivotPos = Vector3.zero;
        private bool editMode;
        private bool pivotEditMode;
        private bool snapMode;
        private float snapValue = 0.1f;

        // T O D O : 트랜스폼의 회전, 스케일 변경에 따라
        //           바운드 기즈모 영역 맞추기

        private bool showBounds;
        private bool confineInBounds;


        private const int ContextPriority = 100;

        [MenuItem("CONTEXT/MeshFilter/Edit Mesh", false, ContextPriority)]
        private static void Context_AddMeshEditor(MenuCommand mc)
        {
            var component = mc.context as Component;
            component.gameObject.AddComponent<MeshEditor>();
        }

        [MenuItem("CONTEXT/MeshFilter/Edit Mesh", true, ContextPriority)]
        private static bool Context_AddMeshEditor_Validate(MenuCommand mc)
        {
            var component = mc.context as Component;
            MeshEditor me = component.GetComponent<MeshEditor>();
            return me == null;
        }
    }
}

#endif