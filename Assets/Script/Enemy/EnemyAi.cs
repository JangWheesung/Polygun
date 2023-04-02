using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FD.Dev;

public class EnemyAi : MonoBehaviour
{
    private enum State { move, shot, warning, diying}
    State state = State.move;

    private new Rigidbody rigidbody;
    private Animator animator;
    private CapsuleCollider bodyCollider;
    private BoxCollider groundCollider;
    private AudioSource shotSound;
    private GameObject player;
    private NavMeshAgent agent;

    [SerializeField] private LayerMask playerMask;

    [Header("Stat")]
    public EnemyKind enemyStat;
    private float enemyHp; //체력
    private float roundPerMinute; //연사력
    private float radiusRange; //경계범위
    private float boundaryTime; //사격 후 경계시간
    private const float moveSpeed = 0.03f; //걷는속도(적 종류 다 동일)
    private const float runSpeed = 7f; //뛰는속도(적 종류 다 동일)

    [Header("Guns")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject fireEmpact;

    [Header("Mode")]
    [SerializeField] private bool survive;

    Vector3 destination;
    Quaternion originalRot;
    float zBlend;
    float time = 0;
    bool shootingDelay;
    bool alert;
    bool die;

    void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();
        bodyCollider = gameObject.GetComponent<CapsuleCollider>();
        groundCollider = gameObject.GetComponent<BoxCollider>();
        shotSound = gameObject.GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");

        enemyHp = enemyStat.hp;
        roundPerMinute = enemyStat.roundPerMinute;
        radiusRange = enemyStat.radiusRange;
        boundaryTime = enemyStat.boundaryTime;

        originalRot = transform.rotation;

        if (survive)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = runSpeed;
            destination = agent.destination;
        }
    }

    void Update()
    {
        Brain();
        Life();
    }

    private void Brain()
    {
        Range();

        switch (state)
        {
            case State.move:
                if (survive) Tracking();
                else Move();
                break;
            case State.shot:
                if(PlayerHp.Instance.hp > 0) Shot();
                break;
        }

        Complete();
    }

    private void Move()
    {
        float z = Vector();

        Vector3 moveZ = transform.forward * z;
        Vector3 pos = moveZ.normalized * moveSpeed;

        zBlend = Mathf.Lerp(zBlend, z, Time.deltaTime * 10f);

        rigidbody.MovePosition(transform.position + pos);

        animator.SetFloat("Y", zBlend);
    }

    private int Vector()
    {
        if (true)
        {
            if (state == State.move) time += Time.deltaTime;

            switch (time)
            {
                case > 4:
                    transform.Rotate(0, 180, 0);
                    time = 0;
                    break;
                case >= 3:
                    return 0;
                case < 3:
                    return 1;
            }
        }
        return 0;
    }

    private void Tracking()
    {
        if (Vector3.Distance(destination, player.transform.position) > 5.0f)
        {
            destination = player.transform.position;
            agent.destination = destination;

            animator.SetBool("Run", true);
        }
    }

    private void Range()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, radiusRange, playerMask);
        if (collider.Length >= 1 && state != State.diying)
        {
            PlayerRaycast();
        }
        if (collider.Length == 0 && state == State.shot && !alert)
        {
            if (survive)
            {
                animator.SetBool("Shot", false);
                animator.SetBool("Run", true);
                transform.rotation = originalRot;
                fireEmpact.SetActive(false);
                shotSound.volume = 0.5f;
                state = State.move;
            }
            else
            {
                state = State.warning;
                Warning();
            }
        }
    }

    private void PlayerRaycast()
    {
        RaycastHit playerHit;
        Vector3 ves = player.transform.position - transform.position;
        Quaternion q = Quaternion.LookRotation(ves.normalized);
        transform.rotation = q;

        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        
        if (Physics.Raycast(pos, transform.forward, out playerHit, radiusRange) && playerHit.transform.CompareTag("Player"))
        {
            Debug.DrawRay(pos, transform.forward * playerHit.distance, Color.red);
            state = State.shot;
        }
        else Debug.DrawRay(pos, transform.forward * radiusRange, Color.blue);
    }

    private void Shot()
    {
        animator.SetBool("Run", false);
        animator.SetBool("Shot", true);

        try
        {
            agent.destination = transform.position;
        }
        catch (Exception exp)
        {

        }

        Vector3 ves = player.transform.position - transform.position;

        Quaternion q = Quaternion.LookRotation(ves.normalized);
        transform.rotation = q;
        transform.eulerAngles += new Vector3(0, 41, 0);

        shotSound.volume = 1;

        if (!shootingDelay)
        {
            FAED.Pop("Bullet", firePoint.position, firePoint.rotation);

            shootingDelay = true;
            StartCoroutine(ShootingDelay(1 / roundPerMinute));
        }

        fireEmpact.SetActive(true);
    }

    private void Warning()
    {
        FAED.InvokeDelay(() => { if(state == State.warning) animator.SetBool("Shot", false); }, boundaryTime);
        FAED.InvokeDelay(() => { if (state == State.warning) {
                state = State.move;
                transform.rotation = originalRot;
                fireEmpact.SetActive(false);
                shotSound.volume = 0.5f;
            } }, boundaryTime + 0.2f);
    }

    private void Life()
    {
        if (enemyHp <= 0 && !die)
        {
            die = true;
            StopAllCoroutines();

            state = State.diying;
            animator.SetBool("Die", true);

            fireEmpact.SetActive(false);

            rigidbody.useGravity = false;
            bodyCollider.enabled = false;
            groundCollider.enabled = false;

            ClearScore.Instance.currentScore++;
        }
    }

    public void Behitting()
    {
        --enemyHp;
        float nowHp = enemyHp;

        state = State.shot;
        alert = true;

        FAED.InvokeDelay(() => { if (nowHp == enemyHp) alert = false; }, 2f);
    }

    private void Complete()
    {
        if (PlayerHp.Instance.hp < 0)
        {
            StopAllCoroutines();
            animator.SetBool("Shot", false);
            fireEmpact.SetActive(false);
            state = State.move;
        }
    }

    IEnumerator ShootingDelay(float time)
    {
        yield return new WaitForSeconds(time);
        shotSound.Play();
        shootingDelay = false;
    }
}
