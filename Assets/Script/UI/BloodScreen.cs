using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodScreen : MonoBehaviour
{
    private Image image;

    void Awake()
    {
        image = gameObject.GetComponent<Image>();
    }

    void Update()
    {
        if (PlayerHp.Instance.hit)
        {
            PlayerHp.Instance.hit = false;

            Color hitColor = image.color;
            hitColor.a = 0.8f;
            image.color = hitColor;
        }

        StartCoroutine(ScreenDesappear(0.1f));
    }

    IEnumerator ScreenDesappear(float time)
    {
        Color hitColor = image.color;
        hitColor.a -= 0.01f;
        image.color = hitColor;
        yield return new WaitForSeconds(time);
    }
}
