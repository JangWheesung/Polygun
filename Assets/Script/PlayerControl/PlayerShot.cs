using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FD.Dev;
using Cinemachine;

public class PlayerShot : MonoBehaviour
{
    private AudioSource shotSound;
    public Animator anim;
    private BulletUi bulletUi;
    public CinemachineVirtualCamera cv;

    [Header("Speed")]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float shootSpeed;
    [SerializeField] private float reloadSpeed;

    [Header("GunPos")]
    [SerializeField] private Transform idlePos;
    [SerializeField] private Transform shotPos;

    [Header("Guns")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject fireEmpact;

    [Header("Magazine")]
    public int maxMagazine;
    public int magazine { get; set; }
    public int bullets;

    bool shootingDelay;
    bool reloading;
    public bool canReshoot;

    void Awake()
    {
        shotSound = gameObject.GetComponent<AudioSource>();
        anim = gameObject.GetComponent<Animator>();
        bulletUi = FindObjectOfType<BulletUi>();

        bullets = maxMagazine;
        canReshoot = true;
    }

    void Update()
    {
        Aiming();
        Shoot();
        Reload();
    }

    void Shoot()
    {
        if (Input.GetButton("Fire1") && bullets > 0 && !shootingDelay && canReshoot)
        {
            GunRaycast.Instance.GunRay();

            bullets--;

            shootingDelay = true;
            StartCoroutine(ShootingDelay(shootSpeed));
        }

        if (Input.GetButton("Fire1") && PlayerMove.Instance.isShooting) fireEmpact.SetActive(true);
        else fireEmpact.SetActive(false);
    }

    void Aiming()
    {
        if (Input.GetButtonDown("Fire1") && bullets > 0 && !PlayerMove.Instance.isSitting)
        {
            anim.SetBool("Shoot2", true);
            Debug.Log(22);

            shotSound.volume = 1;

            canReshoot = true;
            PlayerMove.Instance.isShooting = true;
        }
        else if (Input.GetButtonUp("Fire1") && !PlayerMove.Instance.isSitting && PlayerMove.Instance.isShooting)
        {
            anim.SetBool("Shoot2", false);

            shotSound.volume = 0.5f;

            PlayerMove.Instance.isShooting = false;
        }
    }

    void Reload()
    {
        if (bullets <= 0 && !reloading)
        {
            Reloading();
            if(bulletUi.enabled) anim.SetBool("Shoot2", false);
        }

        if (Input.GetKeyDown(KeyCode.R) && bullets != 50 && !PlayerMove.Instance.isShooting)
        {
            bulletUi.enabled = false;
            bullets = 0;
        }

        if (bullets > 0) reloading = false;
        if (bullets == 1) bullets = 0;
    }

    void Reloading()
    {
        reloading = true;
        canReshoot = false;
        shotSound.volume = 0.5f;
        PlayerMove.Instance.isShooting = false;

        anim.SetTrigger("DoReload");
        fireEmpact.SetActive(false);

        FAED.InvokeDelay(() => {
            bullets = maxMagazine;
            bulletUi.enabled = true;
        }, reloadSpeed);
    }

    IEnumerator ShootingDelay(float time)
    {
        yield return new WaitForSeconds(time);
        shotSound.Play();
        shootingDelay = false;
    }
}
