using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Program.Type
{

    public enum FAED_KeyEventType
    {

        GetKey,
        GetKeyDown,
        GetKeyUp

    }

    public enum FAED_ButtonEventType
    {

        GetButton,
        GetButtonDown,
        GetButtonUp,

    }

    public enum FAED_AxisEventType
    {

        GetAxis,
        GetAxisRaw,
        All

    }

    public enum FAED_MouseType
    {

        Left,
        Right,
        Middle,

    }

    public enum FAED_AxisType
    {

        Horizontal,
        Vertical,

    }

}