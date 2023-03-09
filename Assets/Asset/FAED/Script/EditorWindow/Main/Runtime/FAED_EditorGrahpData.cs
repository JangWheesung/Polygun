using FD.Program.Type;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Program.Editer.Runtime.Data
{

    [System.Serializable]
    public class FAED_EditorGrahpData
    {

        public string Guid;
        public string dialogueText;
        public Vector2 position;
        public FAED_AINodeType nodeType;

    }

    [System.Serializable]
    public class FAED_NodeLinkData
    {

        public string baseNodeGuid;
        public string portName;
        public string targetNodeGuid;
        public int portCount;

    }

    [System.Serializable]
    public class FAED_PortSaveData
    {

        public string GUID;
        public string portName;

        public FAED_PortSaveData(string GUID, string portName)
        {
            this.GUID = GUID;
            this.portName = portName;
        }
    }

}

