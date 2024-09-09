using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    private void Start()
    {
        GetComponent<MeshCollider>().isTrigger = true;
        PlayerController.RespawnKnifeEvent += OnRespawn;
        PlayerController.KnifeFallOutEvent += OnKnifeFallOutEvent;
    }

    private void OnDestroy()
    {
        PlayerController.RespawnKnifeEvent -= OnRespawn;
        PlayerController.KnifeFallOutEvent -= OnKnifeFallOutEvent;
    }

    //Turn on collision if the knife collide with ground not by knife head
    private void OnKnifeFallOutEvent()
    {
        GetComponent<MeshCollider>().isTrigger = false;
    }

    //Turn off collision when the knife respawn
    private void OnRespawn()
    {
        GetComponent<MeshCollider>().isTrigger = true;
    }
}
