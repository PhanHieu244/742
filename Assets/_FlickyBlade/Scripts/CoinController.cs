using UnityEngine;
using System.Collections;
using SgLib;
using System;

public class CoinController : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float animationDuration = 1f;
    public float outOfScreenDistance = 100f;
    public float coinMovingAcceleration = 2f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            CoinManager.Instance.AddCoins(1);
            SoundManager.Instance.PlaySound(SoundManager.Instance.coin);
            Destroy(gameObject, 10f);
            StartCoroutine(StartGetCoinAnimation());
        }
    }

    private IEnumerator StartGetCoinAnimation()
    {
        transform.position = transform.position + new Vector3(0, 0, 5);
        yield return null;
        float secondHasPassed = 0;
        while (secondHasPassed < animationDuration)
        {
            rotationSpeed += 40f;
            transform.localScale += Vector3.one*1.5f*Time.deltaTime;
            secondHasPassed += Time.deltaTime;
            yield return null;
        }
        float distanceHasMoved = 0;
        float v = 0;
        while (distanceHasMoved<outOfScreenDistance)
        {
            v += coinMovingAcceleration * Time.deltaTime;
            
            transform.position +=  new Vector3(0, v*Time.deltaTime, 0);
            distanceHasMoved +=  v * Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    public float randomRotationSpeed = 1f;

    private void Start()
    {
        randomRotationSpeed = UnityEngine.Random.Range(0.8f, 1f);
    }

    private void Update()
    {
        transform.eulerAngles += new Vector3(0, rotationSpeed*randomRotationSpeed * Time.deltaTime, 0);
    }

    
}
