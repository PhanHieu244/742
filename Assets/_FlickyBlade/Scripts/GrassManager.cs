using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassManager : MonoBehaviour {

    public int gameMode = 0;
    public bool enableGrass = true;
	// Use this for initialization
	void Start () {
        GameManager.GameModeIsGoingToChange += OnGameModeIsGoingToChange;
	}
    private void OnDestroy()
    {
        GameManager.GameModeIsGoingToChange -= OnGameModeIsGoingToChange;
    }

    private void OnGameModeIsGoingToChange(int obj)
    {
        if (obj!=gameMode)
        {
            SkinnedMeshRenderer[] mesh = transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var item in mesh)
            {
                item.gameObject.SetActive(false);
                enableGrass = false;
            }
        }
        else
        {
            SkinnedMeshRenderer[] mesh = transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var item in mesh)
            {
                item.gameObject.SetActive(true);
                enableGrass = true;
            }
        }
    }
}
