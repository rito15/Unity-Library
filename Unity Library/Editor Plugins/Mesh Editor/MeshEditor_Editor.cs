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
    public partial class MeshEditor : MonoBehaviour
    {
        [CustomEditor(typeof(MeshEditor))]
        private class Custom : UnityEditor.Editor
        {
            private static readonly Color HandleColor      = new Color(0.6f, 0.4f, 0.9f, 1.0f);
            private static readonly Color DarkButtonColor  = new Color(0.5f, 0.3f, 0.7f, 1.0f);
            private static readonly Color DarkButtonColor2 = new Color(0.4f, 0.1f, 0.5f, 1.0f);
            private static readonly Color LightButtonColor = new Color(0.9f, 0.7f, 1.4f, 1.0f);
            private static readonly Color ContentColor     = new Color(1.4f, 1.1f, 1.8f, 1.0f);
            private static readonly Color BackgroundColor  = new Color(0.3f, 0.1f, 0.6f, 0.2f);

            private const float HeaderButtonHeight = 48f;
            private const float ContentHeight = HeaderButtonHeight + 132f;
            private const float FullContentHeight = ContentHeight + 120f;

            private MeshEditor me;
            private float viewWidth;
            private float safeViewWidth;
            private GUILayoutOption safeViewWidthOption;
            private GUILayoutOption safeViewWidthHalfOption;  // 1/2
            private GUILayoutOption safeViewWidthThirdOption; // 1/3

            private void OnEnable()
            {
                me = target as MeshEditor;

                if (me.meshFilter == null)
                {
                    me.meshFilter = me.GetComponent<MeshFilter>();

                    //if(me.meshFilter != null)
                    //    me.mesh = me.meshFilter.sharedMesh;
                }
            }

            public override void OnInspectorGUI()
            {
                if(DrawWarnings()) return;

                EditorGUILayout.Space(4f);

                InitValues();
                DrawBackgroundBox();
                DrawEditOrCancleButton();

                if (me.editMode)
                    DrawEditModeInspector();

                EditorGUILayout.Space(4f);
            }

            private bool DrawWarnings()
            {
                if (EditorApplication.isPlaying)
                {
                    EditorGUILayout.HelpBox("Cannot Edit in Playmode", MessageType.Warning);
                    return true;
                }

                if (me.meshFilter == null)
                {
                    EditorGUILayout.HelpBox("Mesh Filter Does Not Exist", MessageType.Error);
                    return true;
                }

                if (me.meshFilter.sharedMesh == null)
                {
                    EditorGUILayout.HelpBox("Mesh is Null", MessageType.Error);
                    return true;
                }

                return false;
            }

            private void InitValues()
            {
                viewWidth = EditorGUIUtility.currentViewWidth;
                safeViewWidth = viewWidth - 36f;
                safeViewWidthOption = GUILayout.Width(safeViewWidth);
                safeViewWidthHalfOption = GUILayout.Width(safeViewWidth * 0.5f - 1f);
                safeViewWidthThirdOption = GUILayout.Width(safeViewWidth / 3f - 2f);
            }

            private void DrawBackgroundBox()
            {
                float inspectorHeight = me.editMode ? me.pivotEditMode ? 
                    FullContentHeight : 
                    ContentHeight : 
                    HeaderButtonHeight;

                Rect box = new Rect(0f, 0f, viewWidth, inspectorHeight);

                EditorGUI.DrawRect(box, BackgroundColor);
            }

            private void DrawEditOrCancleButton()
            {
                Color oldBgColor = GUI.backgroundColor;
                Color oldcntColor = GUI.contentColor;
                int oldFontSize = GUI.skin.button.fontSize;
                FontStyle oldFontStyle = GUI.skin.button.fontStyle;

                GUI.backgroundColor = LightButtonColor;
                GUI.contentColor = Color.white * 3f;
                GUI.skin.button.fontSize = 16;
                GUI.skin.button.fontStyle = FontStyle.Bold;

                string buttonText = me.editMode ? "CANCEL" : "EDIT MESH";
                bool editOrCancleButton = GUILayout.Button(buttonText, safeViewWidthOption, GUILayout.Height(28f));
                if (editOrCancleButton)
                {
                    me.editMode = !me.editMode;
                    //Tools.current = Tool.Move;

                    // Click : Edit
                    if (me.editMode)
                    {
                        me.pivotPos = me.transform.position;
                        me.pivotEditMode = true;
                    }
                    // Click : Cancel
                    else
                    {
                        me.pivotEditMode = false;
                    }
                }

                GUI.backgroundColor = oldBgColor;
                GUI.contentColor = oldcntColor;
                GUI.skin.button.fontSize = oldFontSize;
                GUI.skin.button.fontStyle = oldFontStyle;
            }

            private static readonly GUILayoutOption ApplyButtonHeightOption = GUILayout.Height(24f);
            private void DrawEditModeInspector()
            {
                EditorGUILayout.Space(12f);

                // Remember Old Styles
                var oldColor = GUI.color;
                var oldBG = GUI.backgroundColor;
                var oldFontStyle = GUI.skin.button.fontStyle;

                // == 1. Toggle ==================================================================
                GUI.color = ContentColor;

                using (var cs = new EditorGUI.ChangeCheckScope())
                {
                    me.pivotEditMode = EditorGUILayout.Toggle("Edit Pivot", me.pivotEditMode);

                    //if (cs.changed && !me.pivotEditMode)
                    //    Tools.current = Tool.Move;
                }

                // TODO : Undo

                // == 2. Fields ==================================================================
                if (me.pivotEditMode)
                {
                    Vector3 pivotPos = EditorGUILayout.Vector3Field("Pivot Position", me.pivotPos);
                    Undo.RecordObject(me, "Change Pivot Position");

                    if (me.snapMode)
                    {
                        me.pivotPos = SnapVector3(pivotPos, me.snapValue);
                    }
                    else
                    {
                        me.pivotPos = pivotPos;
                    }

                    EditorGUILayout.Space(4f);
                    me.snapMode = EditorGUILayout.Toggle("Snap", me.snapMode);
                    //Undo.RecordObject(me, "Change Snap Value");
                    if (me.snapMode)
                    {
                        float snap = EditorGUILayout.Slider("", me.snapValue, 0f, 1f);
                        me.snapValue = Mathf.Round(snap / 0.05f) * 0.05f;
                    }

                    EditorGUILayout.Space(4f);
                    me.showBounds = EditorGUILayout.Toggle("Show Bounds", me.showBounds);

                    using (new EditorGUI.DisabledGroupScope(!me.showBounds))
                        me.confineInBounds = EditorGUILayout.Toggle("Confine Pivot In Bounds", me.confineInBounds);
                }

                EditorGUILayout.Space(8f);

                // == 3. Transform Reset Buttons ==================================================
                GUI.backgroundColor = DarkButtonColor;
                GUI.skin.button.fontStyle = FontStyle.Bold;

                if (GUILayout.Button("Reset Transform", safeViewWidthOption, ApplyButtonHeightOption))
                {
                    // 피벗 위치도 함께 이동
                    Undo.RecordObject(me, "Reset Transform");
                    me.pivotPos -= me.transform.position;

                    Undo.RecordObject(me.transform, "Reset Transform");
                    me.transform.localPosition = Vector3.zero;
                    me.transform.localRotation = Quaternion.identity;
                    me.transform.localScale = Vector3.one;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Reset Position", safeViewWidthThirdOption, ApplyButtonHeightOption))
                    {
                        // 피벗 위치도 함께 이동
                        Undo.RecordObject(me, "Reset Position");
                        me.pivotPos -= me.transform.position;

                        Undo.RecordObject(me.transform, "Reset Position");
                        me.transform.localPosition = Vector3.zero;
                    }

                    if (GUILayout.Button("Reset Rotation", safeViewWidthThirdOption, ApplyButtonHeightOption))
                    {
                        Undo.RecordObject(me.transform, "Reset Rotation");
                        me.transform.localRotation = Quaternion.identity;
                    }

                    if (GUILayout.Button("Reset Scale", safeViewWidthThirdOption, ApplyButtonHeightOption))
                    {
                        Undo.RecordObject(me.transform, "Reset Scale");
                        me.transform.localScale = Vector3.one;
                    }
                }

                EditorGUILayout.Space(8f);

                // == 4. Apply Buttons =============================================================
                GUI.backgroundColor = DarkButtonColor2;
                GUI.skin.button.fontStyle = FontStyle.Bold;

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Apply", safeViewWidthHalfOption, ApplyButtonHeightOption))
                    {
                        ApplyToCurrentMesh();
                    }

                    if (GUILayout.Button("Save As New File", safeViewWidthHalfOption, ApplyButtonHeightOption))
                    {
                        SaveAsNewMesh();
                    }
                }

                // Restore Styles
                GUI.color = oldColor;
                GUI.backgroundColor = oldBG;
                GUI.skin.button.fontStyle = oldFontStyle;
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

            private void DrawPivotHandle()
            {
                Handles.color = HandleColor;
                float size = HandleUtility.GetHandleSize(me.pivotPos) * 0.8f;

                me.pivotPos = Handles.Slider(me.pivotPos, Vector3.right,   size, Handles.ArrowHandleCap, 1f); // +X
                me.pivotPos = Handles.Slider(me.pivotPos, Vector3.left,    size, Handles.ArrowHandleCap, 1f); // -X
                me.pivotPos = Handles.Slider(me.pivotPos, Vector3.up,      size, Handles.ArrowHandleCap, 1f); // +Y
                me.pivotPos = Handles.Slider(me.pivotPos, Vector3.down,    size, Handles.ArrowHandleCap, 1f); // -Y
                me.pivotPos = Handles.Slider(me.pivotPos, Vector3.forward, size, Handles.ArrowHandleCap, 1f); // +Z
                me.pivotPos = Handles.Slider(me.pivotPos, Vector3.back,    size, Handles.ArrowHandleCap, 1f); // -Z

                Handles.DrawSphere(0, me.pivotPos, Quaternion.identity, size * 0.15f);

                // Snap
                if (me.snapValue > 0f)
                {
                    me.pivotPos = SnapVector3(me.pivotPos, me.snapValue);
                }

                // Bounds
                if (me.showBounds)
                {
                    Vector3 boundsCenter = me.transform.position + me.meshFilter.sharedMesh.bounds.center;
                    Vector3 boundsSize = me.meshFilter.sharedMesh.bounds.size;
                    //Vector3 boundsSize = me.transform.localToWorldMatrix.MultiplyPoint(me.mesh.bounds.size);

                    Handles.DrawWireCube(boundsCenter, boundsSize);
                }
            }

            private int windowID = 99;
            private Rect windowRect;
            private void DrawSceneGUI()
            {
                const float width = 160f;
                const float height = 80f;
                const float paddingX = 70f;
                const float paddingY = 30f;

                float H = height;
                if(me.snapMode) H += 20f;

                windowRect = new Rect(Screen.width - width - paddingX, Screen.height - H - paddingY, width, H);

                windowRect = GUILayout.Window(windowID, windowRect, (id) => {

                    EditorGUILayout.Space(4f);
                    EditorGUILayout.Vector3Field("", me.pivotPos);

                    EditorGUILayout.Space(4f);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Snap", GUILayout.Width(32f));
                        me.snapMode = EditorGUILayout.ToggleLeft("", me.snapMode, GUILayout.Width(16f));
                    }
                    if (me.snapMode)
                    {
                        float snap = EditorGUILayout.Slider("", me.snapValue, 0f, 1f);
                        me.snapValue = Mathf.Round(snap / 0.05f) * 0.05f;
                    }


                    //GUI.DragWindow();

                }, "Pivot Position");
            }

            /// <summary> 변경사항 적용한 새로운 메시 생성 </summary>
            private Mesh EditMesh()
            {
                Matrix4x4 mat = me.transform.localToWorldMatrix;

                Mesh srMesh = me.meshFilter.sharedMesh;
                Mesh newMesh = new Mesh();

                int vertCount = srMesh.vertexCount;
                int trisCount = srMesh.triangles.Length;

                Vector3[] verts = new Vector3[vertCount];
                Vector3[] normals = new Vector3[vertCount];
                Vector4[] tangents = new Vector4[vertCount];
                Vector2[] uvs = new Vector2[vertCount];
                int[] tris = new int[trisCount];

                for (int i = 0; i < vertCount; i++)
                {
                    verts[i] = mat.MultiplyPoint(srMesh.vertices[i]) - me.pivotPos;
                    normals[i] = mat.MultiplyVector(srMesh.normals[i]);
                    tangents[i] = mat.MultiplyVector(srMesh.tangents[i]);
                    uvs[i] = srMesh.uv[i];
                }
                for (int i = 0; i < trisCount; i++)
                {
                    tris[i] = srMesh.triangles[i];
                }

                newMesh.vertices = verts;
                newMesh.normals = normals;
                newMesh.tangents = tangents;
                newMesh.triangles = tris;
                newMesh.uv = uvs;

                // UV2
                int uv2Len = srMesh.uv2.Length;
                if (uv2Len > 0)
                {
                    Vector2[] uv2 = new Vector2[uv2Len];
                    for (int i = 0; i < uv2Len; i++)
                        uv2[i] = srMesh.uv2[i];
                    newMesh.uv2 = uv2;
                }

                newMesh.RecalculateBounds();

                me.transform.localPosition = me.pivotPos;
                me.transform.localRotation = Quaternion.identity;
                me.transform.localScale = Vector3.one;

                me.editMode = false;
                me.pivotEditMode = false;
                Tools.current = Tool.Move;

                return newMesh;
            }

            private void ApplyToCurrentMesh()
            {
                Mesh newMesh = EditMesh();
                newMesh.name = me.meshFilter.sharedMesh.name;

                Undo.RecordObject(me.meshFilter, "Edit Mesh");
                me.meshFilter.sharedMesh = newMesh;
            }

            private void SaveAsNewMesh()
            {
                Mesh newMesh = EditMesh();
                newMesh.name = me.meshFilter.sharedMesh.name + " (New)";

                Undo.RecordObject(me.meshFilter, "Edit Mesh - Save As New");
                me.meshFilter.sharedMesh = newMesh;
            }

            private void EditCollider(in Vector3 offset)
            {
                me.TryGetComponent(out Collider col);
                if (col)
                {
                    if(col is SphereCollider sc)
                        sc.center -= offset;
                    else if (col is BoxCollider bc)
                        bc.center -= offset;
                    else if (col is CapsuleCollider cc)
                        cc.center -= offset;
                }
            }

            /// <summary> 벡터를 일정 단위로 끊기 </summary>
            private Vector3 SnapVector3(Vector3 vec, float snapValue)
            {
                if(snapValue <= 0f) return vec;

                vec.x = Mathf.Round(vec.x / snapValue) * snapValue;
                vec.y = Mathf.Round(vec.y / snapValue) * snapValue;
                vec.z = Mathf.Round(vec.z / snapValue) * snapValue;
                return vec;
            }
        }
    }
}

#endif