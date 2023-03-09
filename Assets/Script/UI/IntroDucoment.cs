using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class IntroDucoment : MonoBehaviour
{
    [SerializeField] private GameObject introCamera;
    [SerializeField] private GameObject enemyObject;
    private bool enemyWindow;
    private int enemyNumber = 1;

    TextElement hp;
    TextElement attack;
    TextElement range;

    private void OnEnable()
    {
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        UIDocument ui = FindObjectOfType<UIDocument>();
        VisualElement root = ui.rootVisualElement;

        VisualElement mainDocument = root.Q("MainDocument");
        VisualElement enemyDocument = root.Q("EnemyList");

        Button play = root.Q<Button>("Play");
        Button howToPlay = root.Q<Button>("HowToPlay");
        Button enemys = root.Q<Button>("Enemys");
        Button exit = root.Q<Button>("Exit");

        Button black = root.Q<Button>("Black");
        Button red = root.Q<Button>("Red");
        Button blue = root.Q<Button>("Blue");
        Button green = root.Q<Button>("Green");

        Button back = root.Q<Button>("Back");

        hp = root.Q<TextElement>("Hp");
        attack = root.Q<TextElement>("Attack");
        range = root.Q<TextElement>("Range");

        play.RegisterCallback<ClickEvent>((e) => { SceneManager.LoadScene("Text"); });
        howToPlay.RegisterCallback<ClickEvent>((e) => 
        {
            introCamera.transform.DORotate(new Vector3(0, -105, 0), 0.5f);
            mainDocument.AddToClassList("off");
            back.ClearClassList();
        });
        enemys.RegisterCallback<ClickEvent>((e) => 
        {
            introCamera.transform.DORotate(new Vector3(0, 90, 0), 0.5f);
            mainDocument.AddToClassList("off");
            enemyDocument.ClearClassList();
            back.ClearClassList();
            enemyWindow = true;
        });
        exit.RegisterCallback<ClickEvent>((e) => { Application.Quit(); });


        
        black.RegisterCallback<ClickEvent>((e) =>
        {
            enemyObject.transform.DOMoveZ(0, 0.5f).OnComplete(() => { enemyNumber = 1; });
        });
        red.RegisterCallback<ClickEvent>((e) =>
        {
            enemyObject.transform.DOMoveZ(4, 0.5f).OnComplete(() => { enemyNumber = 2; });
        });
        blue.RegisterCallback<ClickEvent>((e) =>
        {
            enemyObject.transform.DOMoveZ(8, 0.5f).OnComplete(() => { enemyNumber = 3; });
        });
        green.RegisterCallback<ClickEvent>((e) =>
        {
            enemyObject.transform.DOMoveZ(12, 0.5f).OnComplete(() => { enemyNumber = 4; });
        });

        back.RegisterCallback<ClickEvent>((e) => 
        {
            enemyObject.transform.DOMoveZ(0, 0.5f).OnComplete(() => { enemyNumber = 1; });
            introCamera.transform.DORotate(new Vector3(0, 0, 0), 0.5f);
            back.AddToClassList("off");
            enemyDocument.AddToClassList("off");
            mainDocument.ClearClassList();
            enemyWindow = false;
        });
    }

    private void Update()
    {
        switch (enemyNumber)
        {
            case 1:
                hp.text = "Hp : Normal";
                attack.text = "Attack : Normal";
                range.text = "Range : Normal";
                break;
            case 2:
                hp.text = "Hp : Low";
                attack.text = "Attack : High";
                range.text = "Range : Normal";
                break;
            case 3:
                hp.text = "Hp : High";
                attack.text = "Attack : Low";
                range.text = "Range : Normal";
                break;
            case 4:
                hp.text = "Hp : Normal";
                attack.text = "Attack : Normal";
                range.text = "Range : High";
                break;
        }
    }
}
