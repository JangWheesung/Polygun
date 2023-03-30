using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletUi : MonoBehaviour
{
    private PlayerShot playerShot;
    private Text ammunitionText;

    void Awake()
    {
        playerShot = FindObjectOfType<PlayerShot>();
        ammunitionText = gameObject.GetComponent<Text>();
    }

    void Update()
    {
        ammunitionText.text = $"{playerShot.nowWeapos.bullets}/{playerShot.nowWeapos.maxMagazine}";
    }
}
