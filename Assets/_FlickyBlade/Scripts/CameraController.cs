using UnityEngine;
using System.Collections;
using UnityStandardAssets_ImageEffects;
using System;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    private Vector3 velocity = Vector3.zero;
    private Vector3 originalDistance;

    [Header("Camera Follow Smooth-Time")]
    public float smoothTime = 0.1f;

    [Header("Shaking Effect")]
    // How long the camera shaking.
    public float shakeDuration = 0.1f;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.2f;
    public float decreaseFactor = 0.3f;
    [HideInInspector]
    public Vector3 originalPos;

    private BlurOptimized blurComp;
    private float currentShakeDuration;
    private float currentDistance;

    void Start()
    {
        blurComp = GetComponent<BlurOptimized>();
        blurComp.enabled = false;
        originalDistance = transform.position - playerTransform.transform.position;
    }
    private void OnEnable()
    {
        PlayerController.KnifeStuck += OnKnifeStuck;
        GameManager.NewKnifeHasBeenSpawned += OnNewKnifeRespawned;
    }
    private void OnDisable()
    {
        PlayerController.KnifeStuck -= OnKnifeStuck;
        GameManager.NewKnifeHasBeenSpawned -= OnNewKnifeRespawned;
    }

    

    bool moving = false;
    void Update()
    {
        if ((moving||GameManager.gameMode!=2) && GameManager.Instance.GameState == GameState.Playing)
        {
            Vector3 playerPos = playerTransform.position;
            if (GameManager.gameMode == 2)
            {
                playerPos = new Vector3(0, playerTransform.position.y, 0);
                if (Mathf.Abs(playerPos.y - transform.position.y + originalDistance.y)<0.001f )
                {
                    moving = false;
                }
            }
            Vector3 pos = playerPos + originalDistance;
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothTime);
        }
    }

    public void EnableBlur()
    {
        //blurComp.enabled = true;
    }

    public void DisableBlur()
    {
        //blurComp.enabled = false;
    }

    public void FixPosition()
    {
        transform.position = playerTransform.position + originalDistance;
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    public void ShakeCameraVertical()
    {
        StartCoroutine(ShakeVertical());
    }

    public void ShakeCameraHorizontal()
    {
        StartCoroutine(ShakeHorizontal());
    }

    IEnumerator Shake()
    {
        originalPos = transform.position;
        currentShakeDuration = shakeDuration;
        while (currentShakeDuration > 0)
        {
            transform.position = originalPos + UnityEngine.Random.insideUnitSphere * shakeAmount;
            currentShakeDuration -= Time.deltaTime * decreaseFactor;
            yield return null;
        }
        transform.position = originalPos;
    }

    IEnumerator ShakeVertical()
    {
        yield return null;
        originalPos = transform.position;
        currentShakeDuration = shakeDuration/2;
        while (currentShakeDuration > 0)
        {
            transform.position = originalPos + new Vector3(0,UnityEngine.Random.value*shakeAmount,0);
            currentShakeDuration -= Time.deltaTime * decreaseFactor;
            yield return null;
        }
        transform.position = originalPos;
    }

    IEnumerator ShakeHorizontal()
    {
        yield return null;
        originalPos = transform.position;
        currentShakeDuration = shakeDuration / 2;
        while (currentShakeDuration > 0)
        {
            transform.position = originalPos + new Vector3(UnityEngine.Random.value * shakeAmount,0, 0);
            currentShakeDuration -= Time.deltaTime * decreaseFactor;
            yield return null;
        }
        transform.position = originalPos;
    }


    private void OnNewKnifeRespawned(GameObject obj)
    {
        EnableMoving();
    }

    private void OnKnifeStuck(GameObject arg1, GameObject arg2)
    {
        EnableMoving();
    }

    private void EnableMoving()
    {
        if (gameObject.activeInHierarchy)
        {
            moving = true;
        }
    }

}
