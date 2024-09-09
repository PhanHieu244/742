using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SgLib;
using System;

public class ScoreFlag : MonoBehaviour {

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
        if (GameManager.gameMode==2)
        {
            if (arg2.GetComponentInChildren<PlayerController>().transform.position.y> transform.position.y)
            {
                ScoreManager.Instance.AddScore(1);
                Destroy(gameObject);
            }
        }
    }
}
