using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour {

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
        AddForceToGrass(arg2.GetComponentInChildren<PlayerController>().transform.position);
    }

    public GameObject grassTip;
    public float grassSoftness = 6;
    void AddForceToGrass(Vector3 position)
    {
        if (transform.parent.GetComponent<GrassManager>().enableGrass)
        {
            grassTip.GetComponent<Rigidbody>().AddForce((grassTip.transform.position - position) * grassSoftness * 12, ForceMode.Impulse);
        }
    }
}
