using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class PlayerHead : MonoBehaviour
{
    [SerializeField] private float turnSpeed;
    private PlayerHead playerHead;
    public CinemachineVirtualCamera cv;

    float xRotation = 0.0f;

    private void Awake()
    {
        playerHead = gameObject.GetComponent<PlayerHead>();
    }

    void Update()
    {
        float xRotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        xRotation = Mathf.Clamp(xRotation + xRotationSize, -60, 60);

        if (PlayerHp.Instance.hp < 0) playerHead.enabled = false;

        CameraBrain();
    }

    void CameraBrain()
    {
        transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, 0);

        if (PlayerMove.Instance.runSpeed > 1f) cv.m_Lens.FieldOfView = Mathf.Lerp(cv.m_Lens.FieldOfView, 70, Time.deltaTime * 10);
        else cv.m_Lens.FieldOfView = Mathf.Lerp(cv.m_Lens.FieldOfView, 60, Time.deltaTime * 10);
    }
}
