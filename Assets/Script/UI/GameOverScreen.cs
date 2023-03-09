using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    VisualElement backGround;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        UIDocument ui = FindObjectOfType<UIDocument>();
        VisualElement root = ui.rootVisualElement;

        backGround = root.Q("Content");
        Button reStart = root.Q<Button>("ReStart");
        Button menu = root.Q<Button>("Menu");

        reStart.RegisterCallback<ClickEvent>(e =>
        {
            Debug.Log("Restart");
            SceneManager.LoadScene("Text");
        });

        menu.RegisterCallback<ClickEvent>(e =>
        {
            Debug.Log("Menu");
            SceneManager.LoadScene("Intro");
        });
    }

    private void Update()
    {
        if (PlayerHp.Instance.hp <= 0)
        {
            backGround.AddToClassList("die");

            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }
}
