using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medckit : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private int heeling;

    private AudioSource itemSound;

    private PlayerHp playerHp;

    private void Awake()
    {
        playerHp = FindObjectOfType<PlayerHp>();
        itemSound = GameObject.FindWithTag("Sound").GetComponent<AudioSource>();
    }

    void Update()
    {
        Heel();
    }

    void Heel()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, 0.5f, playerMask);
        if (collider.Length > 0 && playerHp.hp < 100)
        {
            if (playerHp.hp >= (playerHp.maxHp - heeling)) playerHp.hp = playerHp.maxHp;
            else playerHp.hp += heeling;

            itemSound.Play();
            gameObject.SetActive(false);
        }
    }
}
