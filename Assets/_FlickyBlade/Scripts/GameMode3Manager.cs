using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode3Manager : MonoBehaviour {
    public static GameMode3Manager Instance { get; private set; }
    public GameObject gameMode3Camera;
    // Use this for initialization
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
    }

    private void OnEnable()
    {
        GameManager.NewKnifeHasBeenSpawned += OnRespawn;
    }

    private void OnDisable()
    {
        GameManager.NewKnifeHasBeenSpawned -= OnRespawn;
    }

    private void OnRespawn(GameObject knife)
    {
        gameMode3Camera.GetComponent<CameraController>().playerTransform = knife.GetComponentInChildren<PlayerController>().transform;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
