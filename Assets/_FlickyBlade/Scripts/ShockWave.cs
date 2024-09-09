using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShockWave : MonoBehaviour {

    private void OnEnable()
    {
        PlayerController.KnifeStuck += OnKnifeStuck;
        GameManager.GameModeChanged += OnGameModeChange;
    }

    private void OnDisable()
    {
        PlayerController.KnifeStuck -= OnKnifeStuck;
        GameManager.GameModeChanged -= OnGameModeChange;
    }

    private void OnGameModeChange()
    {
        shockWave = false;
    }

    private void OnKnifeStuck(GameObject arg1, GameObject arg2)
    {
        CreateShockWave(arg2.GetComponentInChildren<PlayerController>().transform.position);
    }

    public LayerMask layerMask = 0;

    private void CreateShockWave(Vector3 shockWavePositionWorld)
    {
        Vector2 shockWavePosition = GetComponent<Camera>().WorldToViewportPoint(shockWavePositionWorld);
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(GetComponent<Camera>().WorldToScreenPoint(shockWavePositionWorld));
        
        if (Physics.Raycast(ray,out hit,Mathf.Infinity,layerMask,QueryTriggerInteraction.Collide))
        {
            shockWaveNormal = hit.normal;
            shockWaveMaterial.SetVector("_ShockWaveNormal", shockWaveNormal);
            shockWaveMaterial.SetVector("_ScreenRayVector", ray.direction.normalized);
        }
        
        shockWaveMaterial.SetVector("_ExplosionPos", shockWavePosition);
        shockWaveMaterial.SetFloat("_ExplosionRad",0f);
        shockWaveMaterial.SetFloat("_Ring", ring);
        shockWaveMaterial.SetFloat("_ScreenRatio", (float)Screen.height / Screen.width);
        shockWaveMaterial.SetColor("_ShockWaveColorTint", shockWaveColorTint);
        shockWave = true;
        StartCoroutine(EnableShockWave(shockWavePosition,shockWavePositionWorld));
    }

    private IEnumerator EnableShockWave(Vector2 shockeWavePos, Vector3 shockWavePositionWorld)
    {
        yield return null;
        float timeHasPass = 0;
        float shockWaveSpeed = maxShockWaveRadius / shockWaveDuration;
        float shockWaveRad = 0;
        float shockWaveMagnitudeTemp = shockWaveMagnitude;
        shockWaveMaterial.SetFloat("_Magnitude",shockWaveMagnitudeTemp);
        while (timeHasPass < shockWaveDuration)
        {
            Vector2 shockWavePosition = GetComponent<Camera>().WorldToViewportPoint(shockWavePositionWorld);
            shockWaveMaterial.SetVector("_ExplosionPos", shockWavePosition);
            shockWaveRad += shockWaveSpeed * Time.deltaTime;
            shockWaveMaterial.SetFloat("_ExplosionRad",shockWaveRad);
            shockWaveMagnitudeTemp = shockWaveMagnitude*Mathf.Pow((shockWaveDuration - timeHasPass)/shockWaveDuration,4);
            shockWaveMagnitudeTemp = Mathf.Max(0, shockWaveMagnitudeTemp);
            shockWaveMaterial.SetFloat("_Magnitude", shockWaveMagnitudeTemp);
            timeHasPass += Time.deltaTime;
            yield return null;
        }
        shockWave = false;
    }

    public Color32 shockWaveColorTint = Color.black;
    Vector3 shockWaveNormal = Vector3.up;
    public float shockWaveMagnitude = 0.5f;
    bool shockWave = false;
    public float shockWaveDuration = 1f;
    [Range(0,0.9f)]
    public float ring = 0.5f;
    public float maxShockWaveRadius = 1f;
    public Material shockWaveMaterial;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shockWave&&GameManager.Instance.shockWaveEffect)
        {
            Graphics.Blit(source, destination, shockWaveMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
        
    }
}
