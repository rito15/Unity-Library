#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 날짜 : 2021-03-12 AM 3:51:29
// 작성자 : Rito

namespace Rito.UnityLibrary.EditorPlugins
{
    [CustomEditor(typeof(PlayModeSaver))]
    public class PlayModeSaverEditor : UnityEditor.Editor
    {
        PlayModeSaver pms;

        private void OnEnable()
        {
            pms = target as PlayModeSaver;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}

#endif