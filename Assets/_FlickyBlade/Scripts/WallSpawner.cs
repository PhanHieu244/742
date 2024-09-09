using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour {


    public GameObject wallPrefab;
    public Vector3 spawnOffset = new Vector3(0, 30, 0);
    bool hasSpawned = false;
    private void OnEnable()
    {
        PlayerController.KnifeStuck += OnKnifeStuck;
        PlayerController.RespawnKnifeEvent += OnKnifeRespawned;
    }
    private void OnDisable()
    {
        PlayerController.KnifeStuck -= OnKnifeStuck;
        PlayerController.RespawnKnifeEvent -= OnKnifeRespawned;

    }

    private void OnKnifeRespawned()
    {
        hasSpawned = false;
    }

    private void OnKnifeStuck(GameObject arg1, GameObject arg2)
    {
        if (!hasSpawned && arg2.GetComponentInChildren<PlayerController>().transform.position.y> transform.position.y)
        {
            SpawnNewWall();
        }
    }

    private void SpawnNewWall()
    {
        GameObject newWall = Instantiate(wallPrefab, transform.position + spawnOffset, Quaternion.identity, GameMode3Manager.Instance.transform) as GameObject ;
        newWall.GetComponent<WallParent>().defaultWall = false;
        hasSpawned = true;
    }

}
