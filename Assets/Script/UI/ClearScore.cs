using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FD.Dev;
using UnityEngine.SceneManagement;

public class ClearScore : MonoBehaviour
{
    public static ClearScore Instance;

    public int currentScore;
    public int maxScore;
    bool end;

    [SerializeField] private GameObject endingScreen;
    [SerializeField] private Image endingScreeImage;
    [SerializeField] private GameObject gameName;
    [SerializeField] private Text thankText;
    [SerializeField] private bool survive;
    private Text scoreText;

    private void Awake()
    {
        Instance = this;

        EnemyAi[] enemys = FindObjectsOfType<EnemyAi>();
        maxScore = enemys.Length;
        currentScore = 0;

        scoreText = gameObject.GetComponent<Text>();
        gameName.SetActive(false);
    }

    void Update()
    {
        if (survive)
        {
            scoreText.text = ((int)Time.time).ToString();
        }
        else
        {
            scoreText.text = $"{currentScore}/{maxScore}";

            EndingScreen();
        }
    }

    void EndingScreen()
    {
        if (currentScore == maxScore && !end)
        {
            end = true;

            FAED.InvokeDelay(() => 
            {
                endingScreeImage.DOFade(0.8f, 2).OnComplete(() => 
                { 
                    gameName.SetActive(true);
                    Invoke("Drow", 2);
                }); 
            }, 1);
        }
    }

    void Drow()
    {
        endingScreen.transform.DOMoveY(1850, 25).OnComplete(() => 
        {
            FAED.InvokeDelay(() =>
            {
                thankText.DOColor(new Color(0, 0, 0, 1), 1);

                endingScreeImage.DOFade(1, 1).OnComplete(() => 
                {
                    FAED.InvokeDelay(() => { SceneManager.LoadScene("Intro"); }, 1);
                });
            }, 1);
        });
    }
}
