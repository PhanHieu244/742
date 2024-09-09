using UnityEngine;
using System.Collections;
using SgLib;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PlayerController : MonoBehaviour
{
    public static event System.Action PlayerDied;
    public static event System.Action DragKnife;
    public static event System.Action RespawnKnifeEvent;
    public static event System.Action KnifeFallOutEvent;
    public static event System.Action KnifeHitTheLog;
    public static event System.Action<GameObject,GameObject> KnifeStuck;


    public Rigidbody knifeHead = null;
    public MeshCollider blade = null;
    public bool isAlive = true;
    bool hasSpawned = false;
    float deltaRotation = 0;
    Vector3 lastRotation = Vector3.zero;
    float rotationCount = 0;
    int scoreOfThisFlip = 0;
    float roationDir = 0;
    bool onAir = false;
    bool stuck = false;
    Vector3 originalDragPoint = Vector3.zero;
    Vector3 dragVector = Vector3.zero;

    Rigidbody rb = null;
    public float knifeBladeAngle = 60;
    void OnEnable()
    {
        GameManager.GameStateChanged += OnGameStateChanged;
        GameManager.TouchStateChange += OnTouchStateChanged;
    }

    void OnDisable()
    {
        GameManager.GameStateChanged -= OnGameStateChanged;
        GameManager.TouchStateChange -= OnTouchStateChanged;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
	
    // Update is called once per frame
    void Update()
    {
        Rotate();
        CountRotate();
    }

    // Listens to changes in game state
    void OnGameStateChanged(GameState newState, GameState oldState)
    {
        if (newState == GameState.Playing)
        {
            // Do whatever necessary when a new game starts
        }      
    }

    // Calls this when the player dies and game over
    public void Die()
    {
        // Fire event
        if (PlayerDied != null)
            PlayerDied();
    }

     
    
    /*------------------------------------------*/


    //drag and rotate
    private void OnTouchStateChanged(GameManager.TouchState obj)
    {
        if (obj.Equals(GameManager.TouchState.Enter))
        {
            originalDragPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (obj.Equals(GameManager.TouchState.Exit))
        {
            if (originalDragPoint.magnitude > 0)
            {
                dragVector = (Camera.main.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.InverseTransformPoint(originalDragPoint));
                originalDragPoint = Vector3.zero;
                if (dragVector.magnitude > 2)
                {
                    if (isAlive)
                    {
                        if (knifeHead.isKinematic)
                        {
                            Drag(dragVector);
                        }
                    }
                }
            }
        }

    }

    private void Drag(Vector3 dragVector)
    {
        if (dragVector.y > 0)
        {
            stuck = false;
            DragKnife();
            transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            if (DragKnife != null)
                DragKnife();
            Vector3 dragVectorTemp = new Vector3(-dragVector.x, dragVector.y, 0);
            dragVectorTemp = dragVectorTemp.normalized * Mathf.Clamp(dragVectorTemp.magnitude, GameManager.Instance.minimumDragForce, 10000);
            rb.AddForce(dragVectorTemp * GameManager.Instance.dragForce/5*rb.mass, ForceMode.Impulse);
            roationDir = dragVectorTemp.x / Mathf.Abs(dragVectorTemp.x);
            onAir = true;
        }

    }

    private void Rotate()
    {
        if (onAir)
        {
            rb.angularVelocity = Vector3.back * roationDir * GameManager.Instance.torque;
            knifeHead.angularVelocity = Vector3.back * roationDir * GameManager.Instance.torque;
            SoundManager.Instance.PlaySound(SoundManager.Instance.knifeSwing);
        }
    }

    //count score
    private void CountRotate()
    {
        if (lastRotation == Vector3.zero)
        {
            lastRotation = transform.localEulerAngles;
        }
        else
        {
            deltaRotation = Mathf.Abs(Mathf.DeltaAngle(transform.localEulerAngles.z, lastRotation.z));
            rotationCount += deltaRotation;
        }
        lastRotation = transform.localEulerAngles;
        scoreOfThisFlip = Mathf.RoundToInt(rotationCount / 360);
    }

    //collision check
    private void OnTriggerEnter(Collider other)
    {
        onAir = false;
        if (other.tag.Equals("Ground"))
        {
            StartCoroutine(WaitToCheckKnifeHead(other));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        onAir = false;
        StartCoroutine(WaitToCheckIfKnifeHasStuck());
        
    }

    private IEnumerator WaitToCheckIfKnifeHasStuck()
    {
        yield return new WaitForFixedUpdate();
        if (!stuck)
            KnifeFallout();
    }

    private IEnumerator WaitToCheckKnifeHead(Collider other)
    {
        yield return new WaitForFixedUpdate();
        if (knifeHead.GetComponent<KnifeHead>().knifeHeadTargetCollider!=null&& knifeHead.GetComponent<KnifeHead>().knifeHeadTargetCollider.Equals(other)&&CheckAngleOfKnife(other))
        {
            if (!stuck) {
                stuck = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.knifeHit);
                AddScore();
                StartCoroutine(ShakeKnife());
                KnifeStuck(other.gameObject, transform.parent.gameObject);
                if (KnifeHitTheLog != null)
                    KnifeHitTheLog();
            }
            
        }
        else
        {
            KnifeFallout();
        }
    }

    private bool CheckAngleOfKnife(Collider other)
    {
        if (Mathf.Abs(Mathf.DeltaAngle(transform.localEulerAngles.z, other.transform.localEulerAngles.z)) < knifeBladeAngle)
            return true;        
        else
            return false;
    }

    private IEnumerator ShakeKnife()
    {
        yield return new WaitForEndOfFrame();

        Vector3 shakeRotation = transform.localEulerAngles;
        transform.localEulerAngles = shakeRotation + new Vector3(5f, 0, 0);
    }

    private void AddScore()
    {
        if (GameManager.gameMode == 0 && scoreOfThisFlip > 0)
            ScoreManager.Instance.AddScore(scoreOfThisFlip);
        rotationCount = 0;
    }

    private void KnifeFallout()
    {
        if (isAlive)
        {
            isAlive = false;
            KnifeFallOutEvent();
            StartCoroutine(RespawnKnife());
            ScoreManager.Instance.Reset();
            GameManager.Instance.QuickChangeState();
        }
    }

    private IEnumerator RespawnKnife()
    {
        yield return new WaitForSeconds(1.5f);
        if (!hasSpawned)
        {
            Destroy(gameObject.transform.parent.gameObject);
            if (RespawnKnifeEvent != null)
                RespawnKnifeEvent();
            hasSpawned = true;
        }

    }
}
