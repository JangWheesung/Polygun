using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FD.Program.Type;
using UnityEngine.Events;

namespace FD.Program.Class
{

    [System.Serializable]
    public class FAED_PoolingList
    {

        public string poolName;
        public GameObject poolObj;
        public int poolSize;

    }

    [System.Serializable]
    public class FAED_ClipList
    {

        public string clipName;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0f, 1f)] public float pitch = 1f;
        public bool playOnAwake;
        public bool loop;

    }

    [System.Serializable]
    public class FAED_Keys
    {

        public KeyCode key;
        public FAED_KeyEventType inputType;
        public UnityEvent inputEvent;

    }

    [System.Serializable]
    public class FAED_Mouse
    {

        public FAED_MouseType button;
        public FAED_ButtonEventType inputType;
        public UnityEvent inputEvent;

    }

    [System.Serializable]
    public class FAED_Axis
    {

        public FAED_AxisType axis;
        public FAED_AxisEventType inputType;
        public UnityEvent<float> inputEvent;

    }

}
