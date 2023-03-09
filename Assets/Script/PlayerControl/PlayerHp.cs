using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    public static PlayerHp Instance;

    CapsuleCollider capsuleCollider;
    PlayerMove playerMove;
    PlayerShot playerShot;
    PlayerHead playerHead;
    new Rigidbody rigidbody;

    [SerializeField] private Slider sliderHp;
    [SerializeField] private Image sliderHpFill;
    [SerializeField] private Text textHp;

    public float hp;
    public float maxHp;
    public bool hit;
    bool die;

    void Awake()
    {
        Instance = this;

        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        playerMove = gameObject.GetComponent<PlayerMove>();
        playerShot = FindObjectOfType<PlayerShot>();
        playerHead = FindObjectOfType<PlayerHead>();
        rigidbody = gameObject.GetComponent<Rigidbody>();

        maxHp = hp;
    }

    void Update()
    {
        SliderHpbar();
        Die();
    }

    void SliderHpbar()
    {
        sliderHp.maxValue = maxHp;
        sliderHp.value = hp;
        textHp.text = $"{Mathf.Clamp(hp, 0, 100)}/{maxHp}";

        sliderHpFill.color = hp switch
        {
            > 40 => Color.cyan,
            > 10 => Color.yellow,
            _ => Color.red
        };

        textHp.color = hp switch
        {
            > 10 => Color.white,
            _ => Color.red
        };
    }

    void Die()
    {
        if (hp <= 0 && !die)
        {
            die = true;
            playerMove.enabled = false;
            playerShot.anim.SetBool("Shoot2", false);
            playerShot.enabled = false;
            playerHead.enabled = false;

            rigidbody.constraints = RigidbodyConstraints.None;
            capsuleCollider.material = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Bullet"))
        {
            --hp;
            hit = true;
        }
    }
}
