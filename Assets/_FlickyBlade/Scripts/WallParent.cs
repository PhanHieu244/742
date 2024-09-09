using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallParent : MonoBehaviour {

    public bool defaultWall = false;
    public GameObject scoreFlagPrefab = null;
    public GameObject scoreFlag = null;
    public Vector3 scoreFlagPosition = new Vector3(0, 0, 0);
    public CoinSpawner coinSpawner = null;

	void Start () {
        PlayerController.RespawnKnifeEvent += OnRespawn;
	}

    private void OnDestroy()
    {
        PlayerController.RespawnKnifeEvent -= OnRespawn;
    }

    private void OnRespawn()
    {
        if (!defaultWall)
        {
            Destroy(gameObject);
        }
        else if(scoreFlag==null)
        {
            GameObject newFlag = Instantiate(scoreFlagPrefab, scoreFlagPosition, Quaternion.identity, transform);
            scoreFlag = newFlag;
        }
        if (defaultWall&&GameManager.gameMode==2)
        {
            if (coinSpawner != null)
                coinSpawner.SpawnCoin(true);
        }
    }

    
}
