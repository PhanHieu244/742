using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SgLib;
public class MovingTarget : MonoBehaviour {
    public static event System.Action RespawnKnife;

    public GameObject pos1, pos2, target;

    Vector3 targetPosition = Vector3.zero;
    // Use this for initialization  

    private void OnEnable()
    {
        
        //PlayerController.KnifeHitTheLog += OnKnifeHitLog;
        PlayerController.KnifeStuck += OnKnifeStuck;
        PlayerController.KnifeFallOutEvent += OnKnifeFallOutEvent;
    }

    private void OnDisable()
    {
        
        //PlayerController.KnifeHitTheLog -= OnKnifeHitLog;
        PlayerController.KnifeStuck -= OnKnifeStuck;
        PlayerController.KnifeFallOutEvent -= OnKnifeFallOutEvent;
    }


    /*-----------------------------------*/

    private void OnKnifeFallOutEvent()
    {
        OnKnifeHitLog();
    }

    private void OnKnifeHitLog()
    {
        targetPosition = Vector3.Lerp(pos1.transform.position, pos2.transform.position, UnityEngine.Random.value);
        StartCoroutine(MoveToTargetPostion());
    }

    private IEnumerator MoveToTargetPostion()
    {
        yield return new WaitForSeconds(1.5f);
        Vector3 thisTarget = targetPosition;
        float distance = (targetPosition - target.transform.position).magnitude;
        float movedDistance = 0;
        while (movedDistance< distance&& thisTarget==targetPosition)
        {
            Vector3 movingVector = (targetPosition - target.transform.position) * Time.deltaTime * GameManager.Instance.movingTargetSpeed;
            target.transform.position += movingVector;
            movedDistance += movingVector.magnitude;
            yield return null;
        }
    }

    private void OnKnifeStuck(GameObject obj, GameObject knife)
    {
        if (obj.Equals(target))
        {
            AddScore(knife);
            knife.GetComponentInChildren<PlayerController>().isAlive = false;
            StartCoroutine(DestroyAndRespawn(knife));
        }
    }

    private void AddScore(GameObject knife)
    {
        ScoreManager.Instance.AddScore(Mathf.RoundToInt(2/Mathf.Clamp((knife.GetComponentInChildren<KnifeHead>().transform.position - target.transform.position).magnitude,0.5f,4f)));
        ScoreManager.Instance.UpdateHighScore(ScoreManager.Instance.Score);
    }

    private IEnumerator DestroyAndRespawn(GameObject knife)
    {
        OnKnifeHitLog();
        yield return new WaitForSeconds(1.5f);
        Destroy(knife);
        yield return new WaitForSeconds(0.5f);
        if (RespawnKnife!=null)
        RespawnKnife();
    }

}
