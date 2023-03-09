using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FD.Dev;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private new Rigidbody rigidbody;
    private Rigidbody playerRigidbody;
    bool isflying;

    private void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        JumpRader();
    }

    void JumpRader()
    {
        if (PlayerMove.Instance.isGround)
        {
            transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        }
        else
        {
            transform.position = player.transform.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!PlayerMove.Instance.isGround && !PlayerMove.Instance.wasJump)
        {
            isflying = false;
            PlayerMove.Instance.isGround = true;
        }
    }
}
