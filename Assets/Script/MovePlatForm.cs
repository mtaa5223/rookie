using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatForm : MonoBehaviour
{
    [Serializable]
    struct MoveData
    {
        public Transform movePoint;
        public float moveSpeed;
    }

    [SerializeField] MoveData[] moveDatas;
    int dataCount = 0;

    private void Start()
    {
        //Debug.Log(moveDatas.Length);
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, moveDatas[dataCount % moveDatas.Length].movePoint.position,
            moveDatas[dataCount % moveDatas.Length].moveSpeed * Time.deltaTime);

        if (transform.position == moveDatas[dataCount % moveDatas.Length].movePoint.position)
        {
            dataCount++;
        }
    }
}
