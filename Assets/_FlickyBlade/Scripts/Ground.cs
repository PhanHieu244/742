using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

    public GameObject knifeHeadGround = null;
    
    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
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
        GetComponent<BoxCollider>().isTrigger = false;
    }

    //Turn off collision when the knife respawn
    private void OnRespawn()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

}
