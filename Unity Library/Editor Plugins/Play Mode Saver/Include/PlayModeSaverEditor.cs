#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#pragma warning disable CS0618

// 날짜 : 2021-03-12 AM 3:51:29
// 작성자 : Rito

namespace Rito.UnityLibrary.EditorPlugins
{
    [CustomEditor(typeof(PlayModeSaver))]
    public class PlayModeSaverEditor : UnityEditor.Editor
    {
        private PlayModeSaver pms;

        private static readonly Color GreenColor = Color.green;
        private static readonly Color RedColor = Color.red * 2f;

        private static bool foldoutA = true;
        private static bool foldoutB = true;
        private static bool foldoutC = true;

        /// <summary> [수동] 현재 그려질 컨트롤의 Y 위치 </summary>
        private float currentY = 0f;

        /// <summary> [수동, 자동(Layout)] 모두 Y 공백 삽입 </summary>
        private void NextSpace(float value)
        {
            GUILayout.Space(value);
            currentY += value;
        }

        /// <summary> [수동] Y 공백 삽입</summary>
        private void NextY(float value)
        {
            currentY += value;
        }

        private void OnEnable()
        {
            pms = target as PlayModeSaver;
        }

        public override void OnInspectorGUI()
        {
            Color oldBgColor = GUI.backgroundColor;
            Color oldcntColor = GUI.contentColor;

            currentY = 0f; // 수동 Y 높이 0으로 초기화
            NextSpace(8f);

            DrawOptions();
            NextSpace(8f);

            DrawFunctions();
            NextSpace(8f);

            if (pms._targetList.Count > 0)
            {
                DrawComponentListBox();
                NextSpace(8f);
            }

            GUI.backgroundColor = oldBgColor;
            GUI.contentColor = oldcntColor;
        }

        private void DrawOptions()
        {
            foldoutA = DrawFoldoutHeaderBox(currentY, 22f, foldoutA, "Options",
                Color.black, Color.white * 4f, Color.black);

            if (foldoutA)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.contentColor = Color.white;
                    GUI.backgroundColor = pms._activated ? GreenColor : RedColor;
                    if (GUILayout.Button("Activated"))
                    {
                        Undo.RecordObject(pms, "Change Activated Value");
                        pms._activated = !pms._activated;
                    }

                    GUI.backgroundColor = pms._alwaysOnTop ? GreenColor : RedColor;
                    if (GUILayout.Button("Always On Top"))
                    {
                        Undo.RecordObject(pms, "Change Always On Top Value");
                        pms._alwaysOnTop = !pms._alwaysOnTop;
                    }
                }

                // 버튼을 레이아웃(자동) 요소로 그렸으므로 버튼 높이만큼 수동 공백 삽입
                NextY(22f);
            }
        }

        private void DrawFunctions()
        {
            foldoutB = DrawFoldoutHeaderBox(currentY, 22f, foldoutB, "Functions",
                Color.black, Color.white * 4f, Color.black);

            if (foldoutB)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.contentColor = Color.white;
                    GUI.backgroundColor = Color.blue;
                    if (GUILayout.Button("Add All Components"))
                    {
                        Undo.RecordObject(pms, "Add All PMS Components");
                        pms.AddAllComponentsInGameObject();
                    }

                    if (GUILayout.Button("Remove All Components"))
                    {
                        Undo.RecordObject(pms, "Remove All PMS Components");
                        pms.RemoveAllTargetComponents();
                    }
                }

                NextY(22f);
            }
        }

        private void DrawComponentListBox()
        {
            foldoutC = DrawFoldoutHeaderBox(currentY, 21f * pms._targetList.Count, foldoutC, "Components",
                Color.black, Color.white * 4f, Color.black);

            if (foldoutC)
            {
                GUI.contentColor = Color.white * 2f;
                for (int i = 0; i < pms._targetList.Count; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUI.backgroundColor = Color.black;
                        using (new EditorGUI.DisabledGroupScope(true))
                            _ = EditorGUILayout.ObjectField(pms._targetList[i], typeof(Component));

                        GUI.backgroundColor = RedColor;
                        if (GUILayout.Button("-", GUILayout.Width(20f)))
                        {
                            Undo.RecordObject(pms, "Remove PMS Components");
                            pms._targetList.RemoveAt(i);
                        }
                    }
                    //NextY(22f);
                }
            }
        }

        // boxY : 그려질 Y 위치
        // boxH : 헤더를 제외한, 순수한 박스의 높이
        /// <summary> 헤더(Foldout) + 박스 그리기 </summary>
        private bool DrawFoldoutHeaderBox(float boxY, float boxH, bool foldout, string titleText,
            in Color boxColor, in Color headerColor, in Color titleColor)
        {
            const float boxX = 14f;
            const float padding = 4f;
            const float headerX = boxX + padding;
            const float headerH = 18f;

            // 헤더 높이 + 패딩 * 2
            float headerAreaH = headerH + padding * 2f;

            float headerY = boxY + padding;
            float headerW = EditorGUIUtility.currentViewWidth - headerX - padding;
            float boxW = headerW + headerX;

            // 펼쳤을 때만 박스 보여주기
            boxH = foldout ? (boxH + headerAreaH) : (headerAreaH);

            GUI.backgroundColor = boxColor;
            GUI.Box(new Rect(boxX, boxY, boxW, boxH), ""); // Box

            GUI.backgroundColor = headerColor;
            GUI.contentColor = titleColor;
            GUI.Box(new Rect(headerX, headerY, headerW, headerH), ""); // Header

            foldout = EditorGUI.Foldout(new Rect(headerX + 16f, headerY, headerW, headerH),
                foldout, titleText, true, EditorStyles.boldLabel);

            // 수동 컨트롤을 그려낸 만큼 공백 삽입
            NextSpace(headerH + padding);

            return foldout;
        }
    }
}

#endif