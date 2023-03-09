using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FD.Dev;

public class EscWindow : MonoBehaviour
{
    private Image window;
    private Text text1;
    private Text text2;

    private bool openWindow;

    void Awake()
    {
        window = gameObject.GetComponent<Image>();
        text1 = gameObject.transform.GetChild(0).GetComponent<Text>();
        text2 = gameObject.transform.GetChild(1).GetComponent<Text>();

        window.enabled = false;
        text1.enabled = false;
        text2.enabled = false;

        openWindow = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !openWindow)
        {
            window.enabled = true;
            text1.enabled = true;
            text2.enabled = true;

            FAED.InvokeDelay(() => { openWindow = true; }, 0.2f);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && openWindow)
        {
            window.enabled = false;
            text1.enabled = false;
            text2.enabled = false;

            FAED.InvokeDelay(() => { openWindow = false; }, 0.2f);
        }

        if (Input.GetKeyDown(KeyCode.Return) && openWindow)
        {
            SceneManager.LoadScene("Intro");
        }
    }
}
