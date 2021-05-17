#if UNITY_EDITOR

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
                   T          O             D             O

[Bounds]
- 트랜스폼 변경사항 생기면 Recalculate()
- 모든 버텍스를 TransformPoint해서 얻은 위치들로 Min,Max XYZ 계산

[Confine Pivot In Bounds 토글 활성화 시]
- x, y, z 각각 0~1 범위 슬라이더로 피벗을 바운드 내에서 위치 조정
- 활성화하는 순간 피벗을 바운드 내에 가두면서
  x, y, z 슬라이더 값도 알맞게 조정

*/

// 날짜 : 2021-05-16 PM 3:34:18
// 작성자 : Rito

namespace Rito.UnityLibrary.EditorPlugins
{
    public partial class MeshEditor : MonoBehaviour
    {
        [CustomEditor(typeof(MeshEditor))]
        private partial class Custom : UnityEditor.Editor
        {
            private static readonly Color HandleColor = new Color(0.6f, 0.4f, 0.9f, 1.0f);
            private static readonly Color DarkButtonColor = new Color(0.5f, 0.3f, 0.7f, 1.0f);
            private static readonly Color DarkButtonColor2 = new Color(0.4f, 0.1f, 0.5f, 1.0f);
            private static readonly Color LightButtonColor = new Color(0.9f, 0.7f, 1.4f, 1.0f);
            private static readonly Color ContentColor = new Color(1.4f, 1.1f, 1.8f, 1.0f);
            private static readonly Color BackgroundColor = new Color(0.3f, 0.1f, 0.6f, 0.3f);

            // 1. 기본(버튼 한개)
            private const float HeaderButtonHeight = 48f;

            // 2. Edit 버튼을 누른 경우
            private const float ContentHeight = HeaderButtonHeight + 160f;

            // 3. Edit Pivot 토글을 활성화한 경우
            private const float FullContentHeight = ContentHeight + 128f;

            private MeshEditor me;
            private float viewWidth;
            private float safeViewWidth;
            private GUILayoutOption safeViewWidthOption;
            private GUILayoutOption safeViewWidthHalfOption;  // 1/2
            private GUILayoutOption safeViewWidthThirdOption; // 1/3

            private static readonly GUILayoutOption ApplyButtonHeightOption 
                = GUILayout.Height(24f);

            private GUIStyle boldLabelStyle;
            GUIStyleState boldLabelStyleState;

            private void OnEnable()
            {
                me = target as MeshEditor;

                if (me.meshFilter == null)
                {
                    me.meshFilter = me.GetComponent<MeshFilter>();

                    //if(me.meshFilter != null)
                    //    me.mesh = me.meshFilter.sharedMesh;
                }

                if (me.meshRenderer == null)
                {
                    me.meshRenderer = me.GetComponent<MeshRenderer>();
                }
            }

            public override void OnInspectorGUI()
            {
                if (DrawWarnings()) return;

                // Remember Old Styles
                var oldColor = GUI.color;
                var oldBG = GUI.backgroundColor;
                var oldButtonFontStyle = GUI.skin.button.fontStyle;
                var oldLabelFontStyle = GUI.skin.label.fontStyle;

                InitValues();
                InitStyles();

                EditorGUILayout.Space(4f);
                DrawBackgroundBox();
                DrawEditOrCancleButton();

                if (me.editMode)
                {
                    EditorGUILayout.Space(12f);
                    DrawEditPivotToggle();
                    DrawEditModeFields();

                    EditorGUILayout.Space(12f);
                    DrawTransformResetButtons();

                    EditorGUILayout.Space(12f);
                    DrawApplyButtons();
                }

                EditorGUILayout.Space(4f);

                // Restore Styles
                GUI.color = oldColor;
                GUI.backgroundColor = oldBG;
                GUI.skin.button.fontStyle = oldButtonFontStyle;
                GUI.skin.label.fontStyle = oldLabelFontStyle;
            }

            public void OnSceneGUI()
            {
                if (me.pivotEditMode)
                {
                    //Tools.current = Tool.None;

                    DrawPivotHandle();

                    Handles.BeginGUI();
                    DrawSceneGUI();
                    Handles.EndGUI();
                }
            }

            private void InitValues()
            {
                viewWidth = EditorGUIUtility.currentViewWidth;
                safeViewWidth = viewWidth - 36f;
                safeViewWidthOption = GUILayout.Width(safeViewWidth);
                safeViewWidthHalfOption = GUILayout.Width(safeViewWidth * 0.5f - 1f);
                safeViewWidthThirdOption = GUILayout.Width(safeViewWidth / 3f - 2f);
            }

            private void InitStyles()
            {
                boldLabelStyleState = new GUIStyleState()
                {
                    textColor = Color.white,
                };
                boldLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    normal = boldLabelStyleState,
                };
            }
        }
    }
}

#endif