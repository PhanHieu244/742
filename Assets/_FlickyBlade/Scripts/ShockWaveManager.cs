using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveManager : MonoBehaviour {

    private void OnEnable()
    {
        PlayerController.KnifeStuck += OnKnifeStuck;
    }

    
    private void OnDisable()
    {
        PlayerController.KnifeStuck -= OnKnifeStuck;
    }

    private void OnKnifeStuck(GameObject arg1, GameObject arg2)
    {
        if (GameManager.Instance.shockWaveEffect)
            CreateShockWave(arg2.GetComponentInChildren<PlayerController>().transform.position);
    }
    public LayerMask layerMask = 0;
    public GameObject shockWavePlane;
    public float shockWaveDuration = 1;
    public float maxShockWaveRadius = 6;
    public Color32 shockWaveColorTint = Color.black;
    public float shockWaveOffsetFromSurfaceAmount = 1;
    private void CreateShockWave(Vector3 position)
    {
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(GetComponent<Camera>().WorldToScreenPoint(position));

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
        {
            Vector3 offsetVector = new Vector3(hit.normal.x, hit.normal.y, hit.normal.z*shockWaveOffsetFromSurfaceAmount);
            GameObject gameObjectTemp = Instantiate(shockWavePlane, position + offsetVector, Quaternion.identity)as GameObject ;
            gameObjectTemp.transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            gameObjectTemp.transform.localScale = Vector3.zero;
            gameObjectTemp.GetComponent<ShockWavePlane>().shockWaveDuration = shockWaveDuration;
            gameObjectTemp.GetComponent<ShockWavePlane>().maxShockWaveRadius = maxShockWaveRadius;
            gameObjectTemp.GetComponent<ShockWavePlane>().shockWaveColor = shockWaveColorTint;
            Destroy(gameObjectTemp, shockWaveDuration);
        }
    }
}
