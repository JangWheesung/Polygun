using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FD.Dev;
using Cinemachine;

[System.Serializable]
public class Weapon
{
    public Texture appearance;
    public int maxMagazine;
    public int magazine;
    public int bullets;
    public float shootSpeed;
    public float reloadSpeed;
}

public class PlayerShot : MonoBehaviour
{
    private AudioSource shotSound;
    public Animator anim;
    private BulletUi bulletUi;
    public CinemachineVirtualCamera cv;

    [Header("Speed")]
    [SerializeField] private float rotateSpeed;

    [Header("GunPos")]
    [SerializeField] private Transform idlePos;
    [SerializeField] private Transform shotPos;

    [Header("Guns")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject fireEmpact;

    [Header("Magazine")]
    [SerializeField] private List<Weapon> weapons = new List<Weapon>();
    public Weapon nowWeapos;
    [SerializeField] private Material gunMaterial;

    bool shootingDelay;
    bool reloading;
    public bool canReshoot;

    void Awake()
    {
        shotSound = gameObject.GetComponent<AudioSource>();
        anim = gameObject.GetComponent<Animator>();
        bulletUi = FindObjectOfType<BulletUi>();

        for(int i = 0; i < weapons.Count; i++)
            weapons[i].bullets = weapons[i].maxMagazine;

        canReshoot = true;
        nowWeapos = weapons[0];
    }

    void Update()
    {
        if(Input.anyKeyDown && !PlayerMove.Instance.isShooting && !reloading) GunChange();

        Aiming(nowWeapos);
        Shoot(nowWeapos);
        Reload(nowWeapos);
    }

    void GunChange()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (Input.GetKeyDown(byKeyCode(i + 1)))
                nowWeapos = weapons[i]; Debug.Log(444);
        }

        gunMaterial.SetTexture(nowWeapos.appearance.name, nowWeapos.appearance);
    }

    KeyCode byKeyCode(int inputNumber)
    {
        KeyCode changeKey = inputNumber switch
        {
            1 => KeyCode.Alpha1,
            2 => KeyCode.Alpha2,
            3 => KeyCode.Alpha3,
            _ => KeyCode.Numlock
        };
        return changeKey;
    }

    void Shoot(Weapon wp)
    {
        if (Input.GetButton("Fire1") && wp.bullets > 0 && !shootingDelay && canReshoot)
        {
            GunRaycast.Instance.GunRay();

            wp.bullets--;

            shootingDelay = true;
            StartCoroutine(ShootingDelay(wp.shootSpeed));
        }

        if (Input.GetButton("Fire1") && PlayerMove.Instance.isShooting) fireEmpact.SetActive(true);
        else fireEmpact.SetActive(false);
    }

    void Aiming(Weapon wp)
    {
        if (Input.GetButtonDown("Fire1") && wp.bullets > 0 && !PlayerMove.Instance.isSitting)
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

    void Reload(Weapon wp)
    {
        if (wp.bullets <= 0 && !reloading)
        {
            Reloading(wp);
            if(bulletUi.enabled) anim.SetBool("Shoot2", false);
        }

        if (Input.GetKeyDown(KeyCode.R) && wp.bullets != 50 && !PlayerMove.Instance.isShooting)
        {
            bulletUi.enabled = false;
            wp.bullets = 0;
        }

        if (wp.bullets > 0) reloading = false;
        if (wp.bullets == 1) wp.bullets = 0;
    }

    void Reloading(Weapon wp)
    {
        reloading = true;
        canReshoot = false;
        shotSound.volume = 0.5f;
        PlayerMove.Instance.isShooting = false;

        anim.SetTrigger("DoReload");
        fireEmpact.SetActive(false);

        FAED.InvokeDelay(() => {
            wp.bullets = wp.maxMagazine;
            bulletUi.enabled = true;
        }, wp.reloadSpeed);
    }

    IEnumerator ShootingDelay(float time)
    {
        yield return new WaitForSeconds(time);
        shotSound.Play();
        shootingDelay = false;
    }
}
