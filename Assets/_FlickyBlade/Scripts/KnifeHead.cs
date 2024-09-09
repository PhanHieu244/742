using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KnifeHead : MonoBehaviour {

    Rigidbody rb;
    public PlayerController knifeBody;
    
    public Collider knifeHeadTargetCollider = null;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        PlayerController.DragKnife += OnDragKnife;
        PlayerController.KnifeStuck += OnKnifeStuck;
    }

    private void OnDisable()
    {
        PlayerController.DragKnife -= OnDragKnife;
        PlayerController.KnifeStuck -= OnKnifeStuck;
    }

    private void OnKnifeStuck(GameObject target, GameObject knife)
    {
        rb.isKinematic = true;
    }

    private void OnDragKnife()
    {
        rb.isKinematic = false;
    }

    //check collisition with ground
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag.Equals("Ground"))
        {
            knifeHeadTargetCollider = null;
                    }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag.Equals("Ground"))
        {
            knifeHeadTargetCollider = other;
        }
    }
}
