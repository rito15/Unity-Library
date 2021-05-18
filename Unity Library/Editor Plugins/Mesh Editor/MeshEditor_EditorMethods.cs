#if UNITY_EDITOR

using System;
using System.IO;
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
        private partial class Custom : UnityEditor.Editor
        {
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

            private void RecordTransform()
            {
                prevPosition = me.transform.position;
                prevRotation = me.transform.rotation;
                prevScale = me.transform.localScale;
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

            private void DrawBackgroundBox()
            {
                float inspectorHeight = me.editMode ? me.pivotEditMode ? 
                    FullContentHeight : 
                    ContentHeight : 
                    HeaderButtonHeight;

                if(me.showBounds && me.confineInBounds)
                    inspectorHeight += 48f;

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
                        
                        if(string.IsNullOrWhiteSpace(me.meshName))
                            me.meshName = me.meshFilter.sharedMesh.name;
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

            private void DrawEditPivotToggle()
            {
                GUI.color = ContentColor;

                using (var cs = new EditorGUI.ChangeCheckScope())
                {
                    Undo.RecordObject(me, "Change Edit Pivot Toggle");
                    me.pivotEditMode = EditorGUILayout.Toggle("Edit Pivot", me.pivotEditMode);

                    //if (cs.changed && !me.pivotEditMode)
                    //    Tools.current = Tool.Move;
                }
            }
            private void DrawEditModeFields()
            {
                if (me.pivotEditMode)
                {
                    if (me.showBounds && IsTransformChanged)
                    {
                        RecalculateMinMaxBounds();
                    }

                    // 1. Pivot Position Field
                    Undo.RecordObject(me, "Change Pivot Position");
                    Vector3 pivotPos = EditorGUILayout.Vector3Field("Pivot Position", me.pivotPos);

                    if (me.snapMode)
                    {
                        me.pivotPos = SnapVector3(pivotPos, me.snapValue);
                    }
                    else
                    {
                        me.pivotPos = pivotPos;
                    }

                    // Check Pivot Pos Changed
                    if(me.pivotPos != prevPivotPos && me.showBounds && me.confineInBounds)
                        Local_RecalculateNormalizedPivotPoint();
                    prevPivotPos = me.pivotPos;

                    EditorGUILayout.Space(4f);

                    // 2. Snap Toogle
                    Undo.RecordObject(me, "Change Snap");
                    me.snapMode = EditorGUILayout.Toggle("Snap", me.snapMode);

                    // 3. Snap Value Slider
                    using (new EditorGUI.DisabledGroupScope(!me.snapMode))
                    {
                        Undo.RecordObject(me, "Change Snap Value");
                        float snap = EditorGUILayout.Slider("", me.snapValue, 0f, 1f);
                        me.snapValue = Mathf.Round(snap / 0.05f) * 0.05f;
                    }

                    EditorGUILayout.Space(4f);

                    // 4. Bounds Toggle
                    using (var cc = new EditorGUI.ChangeCheckScope())
                    {
                        Undo.RecordObject(me, "Change Show Bounds");
                        me.showBounds = EditorGUILayout.Toggle("Show Bounds", me.showBounds);

                        if (cc.changed && me.showBounds)
                        {
                            RecalculateMinMaxBounds();

                            if(me.confineInBounds)
                                Local_RecalculateNormalizedPivotPoint();
                        }
                    }

                    // 5. Bounds - Confine Toggle
                    using (new EditorGUI.DisabledGroupScope(!me.showBounds))
                    using (var cc = new EditorGUI.ChangeCheckScope())
                    {
                        Undo.RecordObject(me, "Change Confine In Bounds");
                        me.confineInBounds = EditorGUILayout.Toggle("Confine Pivot In Bounds", me.confineInBounds);

                        if(cc.changed && me.confineInBounds)
                            Local_RecalculateNormalizedPivotPoint();
                    }

                    // Normalized X, Y, Z Pivot Point Slider
                    if (me.showBounds && me.confineInBounds)
                    {
                        me.pivotPos = ClampVector3(me.pivotPos, me.minBounds, me.maxBounds);

                        // 6. XYZ Sliders
                        using (var cc = new EditorGUI.ChangeCheckScope())
                        {
                            me.normalizedPivotPoint.x =
                                EditorGUILayout.Slider("X", me.normalizedPivotPoint.x, 0f, 1f);
                            me.normalizedPivotPoint.y =
                                EditorGUILayout.Slider("Y", me.normalizedPivotPoint.y, 0f, 1f);
                            me.normalizedPivotPoint.z =
                                EditorGUILayout.Slider("Z", me.normalizedPivotPoint.z, 0f, 1f);

                            if (cc.changed)
                            {
                                me.pivotPos.x = Mathf.Lerp(me.minBounds.x, me.maxBounds.x, me.normalizedPivotPoint.x);
                                me.pivotPos.y = Mathf.Lerp(me.minBounds.y, me.maxBounds.y, me.normalizedPivotPoint.y);
                                me.pivotPos.z = Mathf.Lerp(me.minBounds.z, me.maxBounds.z, me.normalizedPivotPoint.z);
                            }
                        }
                    }
                }

                void Local_RecalculateNormalizedPivotPoint()
                {
                    me.pivotPos = ClampVector3(me.pivotPos, me.minBounds, me.maxBounds);

                    me.normalizedPivotPoint.x = (me.pivotPos.x - me.minBounds.x) / (BoundsSize.x);
                    me.normalizedPivotPoint.y = (me.pivotPos.y - me.minBounds.y) / (BoundsSize.y);
                    me.normalizedPivotPoint.z = (me.pivotPos.z - me.minBounds.z) / (BoundsSize.z);
                }
            }
            private void DrawTransformResetButtons()
            {
                GUI.backgroundColor = DarkButtonColor;
                GUI.skin.button.fontStyle = FontStyle.Bold;

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Reset Transform", safeViewWidthHalfOption, ApplyButtonHeightOption))
                    {
                        Undo.RecordObject(me.transform, "Reset Transform");
                        me.transform.localPosition = Vector3.zero;
                        me.transform.localRotation = Quaternion.identity;
                        me.transform.localScale = Vector3.one;

                        // 피벗 위치도 함께 이동
                        Undo.RecordObject(me, "Reset Transform");
                        me.pivotPos = me.transform.position;

                        RecalculateMinMaxBounds();
                    }

                    if (GUILayout.Button("Reset Pivot Position", safeViewWidthHalfOption, ApplyButtonHeightOption))
                    {
                        Undo.RecordObject(me, "Reset Pivot Position");
                        me.pivotPos = me.transform.position;
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Reset Position", safeViewWidthThirdOption, ApplyButtonHeightOption))
                    {
                        Undo.RecordObject(me.transform, "Reset Position");
                        me.transform.localPosition = Vector3.zero;

                        // 피벗 위치도 함께 이동
                        Undo.RecordObject(me, "Reset Position");
                        me.pivotPos = me.transform.position;

                        RecalculateMinMaxBounds();
                    }

                    if (GUILayout.Button("Reset Rotation", safeViewWidthThirdOption, ApplyButtonHeightOption))
                    {
                        Undo.RecordObject(me.transform, "Reset Rotation");
                        me.transform.localRotation = Quaternion.identity;

                        RecalculateMinMaxBounds();
                    }

                    if (GUILayout.Button("Reset Scale", safeViewWidthThirdOption, ApplyButtonHeightOption))
                    {
                        Undo.RecordObject(me.transform, "Reset Scale");
                        me.transform.localScale = Vector3.one;

                        RecalculateMinMaxBounds();
                    }
                }
            }
            private void DrawApplyButtons()
            {

                GUI.backgroundColor = DarkButtonColor2;
                GUI.skin.button.fontStyle = FontStyle.Bold;

                //using (new EditorGUILayout.HorizontalScope())
                //{
                //    EditorGUILayout.LabelField("Mesh Name",
                //        boldLabelStyle,
                //        GUILayout.Width(80f)
                //    );

                //    Undo.RecordObject(me, "Change Mesh Name");
                //    me.meshName = EditorGUILayout.TextField(me.meshName);
                //}
                Undo.RecordObject(me, "Change Mesh Name");
                me.meshName = EditorGUILayout.TextField("Mesh Name", me.meshName);

                if (string.IsNullOrWhiteSpace(me.meshName))
                {
                    EditorGUILayout.HelpBox("Input Mesh Name", MessageType.Error);
                    return;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Apply", safeViewWidthHalfOption, ApplyButtonHeightOption))
                    {
                        Undo.RecordObject(me, "Click Edit Mesh Button");
                        ApplyToCurrentMesh();
                    }

                    if (GUILayout.Button("Save As Obj File", safeViewWidthHalfOption, ApplyButtonHeightOption))
                    {
                        SaveAsObjFile();
                    }
                }
            }

            private void DrawPivotHandle()
            {
                Handles.color = HandleColor;
                float size = HandleUtility.GetHandleSize(me.pivotPos) * 0.8f;

                Undo.RecordObject(me, "Move Pivot Position");
                me.pivotPos = Handles.Slider(me.pivotPos, Vector3.right,   size, Handles.ArrowHandleCap, 1f); // +X
                me.pivotPos = Handles.Slider(me.pivotPos, Vector3.left,    size, Handles.ArrowHandleCap, 1f); // -X
                me.pivotPos = Handles.Slider(me.pivotPos, Vector3.up,      size, Handles.ArrowHandleCap, 1f); // +Y
                me.pivotPos = Handles.Slider(me.pivotPos, Vector3.down,    size, Handles.ArrowHandleCap, 1f); // -Y
                me.pivotPos = Handles.Slider(me.pivotPos, Vector3.forward, size, Handles.ArrowHandleCap, 1f); // +Z
                me.pivotPos = Handles.Slider(me.pivotPos, Vector3.back,    size, Handles.ArrowHandleCap, 1f); // -Z

                Handles.DrawSphere(0, me.pivotPos, Quaternion.identity, size * 0.15f);

                // Snap 
                if (me.snapMode && me.snapValue > 0f)
                {
                    me.pivotPos = SnapVector3(me.pivotPos, me.snapValue);
                }

                // Bounds
                if (me.showBounds)
                {
                    //Vector3 boundsCenter = me.transform.position + me.meshFilter.sharedMesh.bounds.center;
                    //Vector3 boundsSize = me.meshFilter.sharedMesh.bounds.size;
                    //Vector3 boundsSize = me.transform.localToWorldMatrix.MultiplyPoint(me.mesh.bounds.size);

                    Handles.DrawWireCube(BoundsCenter, BoundsSize);
                }
            }

            private int windowID = 99;
            private Rect windowRect;
            private void DrawSceneGUI()
            {
                const float width = 160f;
                const float height = 100f;
                const float paddingX = 70f;
                const float paddingY = 30f;

                windowRect = new Rect
                (
                    Screen.width  - width  - paddingX, 
                    Screen.height - height - paddingY, 
                    width,
                    height
                );

                windowRect = GUILayout.Window(windowID, windowRect, (id) => {

                    EditorGUILayout.Space(4f);

                    Undo.RecordObject(me, "Move Pivot Position");
                    Vector3 pivotPos = EditorGUILayout.Vector3Field("", me.pivotPos);

                    if (me.snapMode && me.snapValue > 0f)
                    {
                        me.pivotPos = SnapVector3(pivotPos, me.snapValue);
                    }
                    else
                    {
                        me.pivotPos = pivotPos;
                    }

                    EditorGUILayout.Space(4f);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Snap", GUILayout.Width(32f));
                        me.snapMode = EditorGUILayout.ToggleLeft("", me.snapMode, GUILayout.Width(16f));
                    }

                    using (new EditorGUI.DisabledGroupScope(!me.snapMode))
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
                newMesh.name = me.meshName;

                Undo.RecordObject(me.meshFilter, "Edit Mesh");
                me.meshFilter.sharedMesh = newMesh;
            }

            private void SaveAsObjFile()
            {
                string meshName = me.meshName;
                string path = 
                    EditorUtility.SaveFilePanelInProject("Save Mesh As Obj File", meshName, "obj", "");

                if(string.IsNullOrWhiteSpace(path))
                    return;

                Mesh newMesh = EditMesh();
                ObjExporter.SaveMeshToFile(newMesh, me.meshRenderer, meshName, path);
                AssetDatabase.Refresh();
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

            private Vector3 ClampVector3(Vector3 vec, in Vector3 min, in Vector3 max)
            {
                vec.x = Mathf.Clamp(vec.x, min.x, max.x);
                vec.y = Mathf.Clamp(vec.y, min.y, max.y);
                vec.z = Mathf.Clamp(vec.z, min.z, max.z);
                return vec;
            }

            private void RecalculateMinMaxBounds()
            {
                Mesh mesh = me.meshFilter.sharedMesh;

                me.minBounds = Vector3.positiveInfinity;
                me.maxBounds = Vector3.negativeInfinity;

                foreach (var vert in mesh.vertices)
                {
                    Vector3 v = me.transform.TransformPoint(vert);

                    if (v.x > me.maxBounds.x) me.maxBounds.x = v.x;
                    else if (v.x < me.minBounds.x) me.minBounds.x = v.x;

                    if (v.y > me.maxBounds.y) me.maxBounds.y = v.y;
                    else if (v.y < me.minBounds.y) me.minBounds.y = v.y;

                    if (v.z > me.maxBounds.z) me.maxBounds.z = v.z;
                    else if (v.z < me.minBounds.z) me.minBounds.z = v.z;
                }

                Debug.Log("ReCalc");
            }
        }
    }
}

#endif