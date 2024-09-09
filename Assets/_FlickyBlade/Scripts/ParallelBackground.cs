using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelBackground : MonoBehaviour {

    public List<GameObject> backgroundList = new List<GameObject>();
    public List<Vector3> backGroundDefaultPos = new List<Vector3>();
    public float parallelBackgroundSpeed = 1f;
    public Vector3 defaultCameraPos = Vector3.zero;
    private void Start()
    {
        defaultCameraPos = GameMode3Manager.Instance.gameMode3Camera.transform.position;
        foreach (var item in backgroundList)
        {
            Vector3 itemPos = item.transform.localPosition;
            backGroundDefaultPos.Add(itemPos);
        }
    }

    void Update () {
        for (int i = 0; i < backgroundList.Count; i++)
        {
            Vector3 backgroundPos = backgroundList[i].transform.localPosition;
            backgroundPos.y = backGroundDefaultPos[i].y - (backgroundList.Count - i-1) * (Mathf.Clamp (GameMode3Manager.Instance.gameMode3Camera.transform.position.y - defaultCameraPos.y,0f,1000f) )* parallelBackgroundSpeed;
            backgroundList[i].transform.localPosition = backgroundPos;
        }
	}
}
