using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FD.Program.Class;
using System;
using FD.Program.Type;

public class FAED_InputManager : MonoBehaviour
{

    [Header("---키---")]
    [SerializeField] private List<FAED_Keys> key = new List<FAED_Keys>();
    [Header("---마우스---")]
    [SerializeField] private List<FAED_Mouse> mouse = new List<FAED_Mouse>();
    [Header("---Axis---")]
    [SerializeField] private List<FAED_Axis> axis = new List<FAED_Axis>();

    private void Update()
    {

        KeyInput();
        MouseInput();
        AxisInput();

    }

    private void AxisInput()
    {

        foreach(var item in axis)
        {

            Action action = item.inputType switch
            {

                FAED_AxisEventType.GetAxis => () =>
                {

                    float value = Input.GetAxis(item.axis.ToString());
                    item.inputEvent.Invoke(value);

                },
                FAED_AxisEventType.GetAxisRaw => () =>
                {

                    float value = Input.GetAxisRaw(item.axis.ToString());
                    item.inputEvent.Invoke(value);

                },
                FAED_AxisEventType.All => () =>
                {

                    float value = Input.GetAxis(item.axis.ToString()) + Input.GetAxisRaw(item.axis.ToString());
                    item.inputEvent.Invoke(value);

                },
                _ => () => Debug.LogWarning("!!"),

            };

            action?.Invoke();

        }

    }

    private void MouseInput()
    {

        foreach(var item in mouse)
        {

            Action action = item.inputType switch
            {

                FAED_ButtonEventType.GetButton => () =>
                {

                    if (Input.GetMouseButton((int) item.button))
                    {

                        item.inputEvent?.Invoke();

                    }

                },
                FAED_ButtonEventType.GetButtonDown => () =>
                {

                    if (Input.GetMouseButtonDown((int)item.button))
                    {

                        item.inputEvent?.Invoke();

                    }

                },
                FAED_ButtonEventType.GetButtonUp => () =>
                {

                    if (Input.GetMouseButtonUp((int)item.button))
                    {

                        item.inputEvent?.Invoke();

                    }

                },
                _ => () => Debug.LogWarning("!!")
            };

            action?.Invoke();

        }

    }

    private void KeyInput()
    {

        foreach(var item in key)
        {

            Action action = item.inputType switch
            {

                FAED_KeyEventType.GetKey => () =>
                {

                    if (Input.GetKey(item.key))
                    {

                        item.inputEvent?.Invoke();

                    }

                },
                FAED_KeyEventType.GetKeyDown => () =>
                {

                    if (Input.GetKeyDown(item.key))
                    {

                        item.inputEvent?.Invoke();

                    }

                },
                FAED_KeyEventType.GetKeyUp => () =>
                {

                    if (Input.GetKeyUp(item.key))
                    {

                        item.inputEvent?.Invoke();

                    }

                },
                _ => () => Debug.LogWarning("!!"),
                

            };

            action?.Invoke();

        }

    }

}
