using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FD.AI;

namespace FD.Program.Editer
{

    [CustomEditor(typeof(FAED_AI))]
    public class FAED_AIEditorUI : Editor
    {

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();

            FAED_AI aI = (FAED_AI) target;

            if (GUILayout.Button("SettingAI"))
            {

                aI.Setting();

            }

            if (GUILayout.Button("ResetAI"))
            {

                aI.ResetAI();

            }

        }

    }

}
