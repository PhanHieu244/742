using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWavePlane : MonoBehaviour {

    private void OnEnable()
    {
        GameManager.GameModeChanged += OnGameModeChange;
    }

    private void OnDisable()
    {
        GameManager.GameModeChanged -= OnGameModeChange;
    }
    public float shockWaveDuration = 1;
    public float maxShockWaveRadius = 6;
    public float shockWaveMagnitude = 50;
    private void OnGameModeChange()
    {
        Destroy(gameObject);
    }
    Camera mainCam;
    public Material shockWaveMaterial;
    public Color32 shockWaveColor = Color.black;
    Material shockWaveMat;
    //public Texture2D CapturedScreenshot;
    //public LayerMask grabpassLayer;
    public RenderTexture CapturedScreenshot;
    private void Start()
    {
        CapturedScreenshot = new RenderTexture(Screen.width, Screen.height, 24);
        StartCoroutine(Explode());
        Camera.main.transform.GetChild(1).GetComponent<Camera>().targetTexture = CapturedScreenshot;
        //UpdateSceenShot();
        shockWaveMat = GetComponent<MeshRenderer>().sharedMaterial;
        shockWaveMat.SetTexture("_GrabTexture", CapturedScreenshot);
    }


    void Update()
    {
        //UpdateSceenShot();
        //shockWaveMat.SetTexture("_GrabTexture", CapturedScreenshot);
    }

    // private void UpdateSceenShot()
    // {
    //     mainCam = Camera.main;
    //     LayerMask layerMaskTemp = mainCam.cullingMask;
    //     mainCam.cullingMask = layerMaskTemp;
    //     RenderTexture tempRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
    //     mainCam.targetTexture = tempRT;
    //     CapturedScreenshot = new Texture2D(tempRT.width, tempRT.height, TextureFormat.RGBA32, false);
    //     mainCam.Render();
    //     RenderTexture.active = tempRT;
    //     CapturedScreenshot.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
    //     CapturedScreenshot.Apply();
    //     RenderTexture.active = null;
    //     RenderTexture.ReleaseTemporary(tempRT);
    //     mainCam.targetTexture = null;
    //     mainCam.cullingMask = layerMaskTemp;
    //     mainCam.Render();
    // }

    private IEnumerator Explode()
    {
        shockWaveMaterial.SetColor("_Color",shockWaveColor);
        yield return null;
        float shockWaveMagnitudeTemp = shockWaveMagnitude;
        shockWaveMaterial.SetFloat("_BumpAmt", shockWaveMagnitudeTemp);
        float timeHasPass = 0;
        float shockWaveSpeed = maxShockWaveRadius / shockWaveDuration;
        float shockWaveRad = 0;
        while (timeHasPass < shockWaveDuration)
        {
            shockWaveRad += shockWaveSpeed * Time.deltaTime;
            transform.localScale = new Vector3(shockWaveRad, shockWaveRad, shockWaveRad);
            shockWaveMagnitudeTemp = shockWaveMagnitude * Mathf.Pow((shockWaveDuration - timeHasPass) / shockWaveDuration, 4);
            shockWaveMagnitudeTemp = Mathf.Max(0, shockWaveMagnitudeTemp);
            shockWaveMaterial.SetFloat("_BumpAmt", shockWaveMagnitudeTemp);
            timeHasPass += Time.deltaTime;
            yield return null;
        }
    }
}
