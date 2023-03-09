using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FD.Program.Editer.Runtime.Data;

namespace FD.Program.Editer.SO
{

    [System.Serializable]
    public class FAED_DialougeContainer : ScriptableObject
    {
        
        public List<FAED_NodeLinkData> links = new List<FAED_NodeLinkData>();
        public List<FAED_EditorGrahpData> editorGrahpData = new List<FAED_EditorGrahpData>();

    }


}

