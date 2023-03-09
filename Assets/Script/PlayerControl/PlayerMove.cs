using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FD.Dev;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance;

    private new Rigidbody rigidbody;
    [SerializeField] private CapsuleCollider bodyCollider;

    [Header("Speed")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float turnSpeed;
    public float runSpeed;
    private float slowSpeed;

    [Header("State")]
    public bool isGround;
    public bool isMoving;
    public bool isShooting;
    public bool isSitting;
    public bool wasJump;
    bool canSit;
    bool ceiling;

    [Header("Other")]
    float xRotation = 0.0f;
    public float yRotationSize;

    private RaycastHit hit;

    float xBlend;
    float zBlend;
    float sitTime;

    void Awake()
    {
        Instance = this;

        rigidbody = gameObject.GetComponent<Rigidbody>();

        runSpeed = 1f;
        slowSpeed = 1f;
    }

    void Update()
    {
        Brain();
        CeilingRaycast();
    }

    void Brain()
    {
        MouseRotation();
        if (!isSitting)
        {
            Move();
            if (!ceiling) Jump();
            if (!isShooting)
            {
                slowSpeed = 1f;
            }
            else slowSpeed = 0.3f;
        }

        //if (isGround && !isShooting)
        //{
        //    IsSitting();
        //}
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        float x2 = -z;
        float z2 = x;

        Vector3 moveX = transform.right * (x);
        Vector3 moveZ = transform.forward * (z);
        Vector3 pos = (moveX + moveZ).normalized * moveSpeed * runSpeed * slowSpeed;

        if (x != 0 || z != 0) isMoving = true;
        else isMoving = false;

        rigidbody.MovePosition(transform.position + pos);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Vector3 velo = rigidbody.velocity;
            velo.y = jumpSpeed;
            rigidbody.velocity = velo;

            isGround = false;
            wasJump = true;
            FAED.InvokeDelay(() => { wasJump = false; }, 0.3f);
        }
    }

    void IsSitting()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            canSit = true;
            sitTime = 0;
        }

        if (Input.GetKey(KeyCode.C) && canSit)
        {
            isSitting = true;

            bodyCollider.center = new Vector3(0, 1.2f, 0);
            bodyCollider.height = 1;

            sitTime += Time.deltaTime;
            if (sitTime > 7)
            {
                canSit = false;
                isSitting = false;
            }
        }
        else
        {
            canSit = false;
            isSitting = false;

            bodyCollider.center = new Vector3(0, 1.65f, 0);
            bodyCollider.height = 1.86f;
        }
    }

    void CeilingRaycast()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);

        if (Physics.Raycast(pos, Vector3.up, out hit, 3))
        {
            ceiling = true;
            Debug.DrawRay(pos, Vector3.up * hit.distance, Color.red);
        }
        else
        {
            ceiling = false;
            Debug.DrawRay(pos, Vector3.up * 1, Color.red);
        }
    }

    void MouseRotation()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        yRotationSize = Input.GetAxis("Mouse X") * turnSpeed;
        float yRotation = transform.eulerAngles.y + yRotationSize;

        float xRotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        xRotation = Mathf.Clamp(xRotation + xRotationSize, -40, 30);

        transform.eulerAngles = new Vector3(transform.rotation.x, yRotation, transform.rotation.z);
    }
}
