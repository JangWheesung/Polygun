using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class RunningGauge : MonoBehaviour
{
    public static RunningGauge Instance;
    public CinemachineVirtualCamera cv;
    private PlayerHead playerHead;
    private Slider runningBar;

    private Vector3 nowPos;
    public bool canRunning;

    void Awake()
    {
        playerHead = FindObjectOfType<PlayerHead>();
        runningBar = gameObject.GetComponent<Slider>();

        Instance = this;
        canRunning = true;
        nowPos = transform.position;
    }

    void Update()
    {
        Running();

        GaugeRecharge();

        GaugeFullset();
    }

    void Running()
    {
        if (Input.GetKey(KeyCode.LeftShift) && canRunning && !PlayerMove.Instance.isShooting && PlayerMove.Instance.isMoving && !PlayerMove.Instance.isSitting)
        {
            PlayerMove.Instance.runSpeed = 1.3f;

            CameraShake(1.5f, 3f);

            StartCoroutine(ValueChange(-0.01f, 0.1f));
        }
        else
        {
            PlayerMove.Instance.runSpeed = 1f;

            if (PlayerMove.Instance.isShooting)
            {
                CameraShake(0.5f, 5f);
            }
            else if(PlayerMove.Instance.isMoving)
            {
                CameraShake(1f, 1f);
            }
            else CameraShake(0.7f, 0.7f);

            StartCoroutine(ValueChange(0.005f, 0.1f));
        }
    }

    void GaugeRecharge()
    {
        if (runningBar.value <= 0)
        {
            canRunning = false;
            for (; runningBar.value >= 10;)
            {
                StartCoroutine(ValueChange(0.01f, 0.01f));
            }
        }
    }

    void GaugeFullset()
    {
        if (runningBar.value >= 10)
        {
            canRunning = true;
            transform.position = new Vector3(-1110, transform.position.y, transform.position.z);
        }
        else transform.position = nowPos;
    }

    void CameraShake(float reach, float speed)
    {
        cv.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = reach;
        cv.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = speed;
    }

    IEnumerator ValueChange(float amount, float speed)
    {
        runningBar.value += amount;
        yield return new WaitForSeconds(speed);
    }
}
