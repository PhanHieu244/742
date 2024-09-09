using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour {

    public GameObject pos1 = null, pos2 =null;
    public GameObject coinPrefab = null;
    public int maxCoin = 10;


    private void Start()
    {
        SpawnCoin(false);
    }

    public void SpawnCoin(bool destroyOtherCoin)
    {
        if (destroyOtherCoin)
        {
            foreach (var item in GameObject.FindGameObjectsWithTag("Gold"))
            {
                Destroy(item);
            }
        }
        if (GameManager.gameMode == 2)
        {
            for (int i = 0; i < Mathf.RoundToInt(maxCoin * GameManager.Instance.coinFrequency); i++)
            {
                Instantiate(coinPrefab, new Vector3(Random.Range(pos1.transform.position.x, pos2.transform.position.x), Random.Range(pos1.transform.position.y, pos2.transform.position.y), pos1.transform.position.z), Quaternion.identity, transform.parent);
            }
        }
    }

}
