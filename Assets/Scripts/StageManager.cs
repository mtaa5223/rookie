using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private GameObject damageField;
    private bool upField = false;
    [SerializeField] private float fieldSpeed;

    [SerializeField] private GameObject[] doorOpenPoints;
    private int openCount;
    private float openLength = 0;

    private bool openDoor = false;
    [SerializeField] private float openSpeed;

    [Serializable]
    private struct Door
    {
        public GameObject leftDoor;
        public GameObject rightDoor;
    }

    [SerializeField] private Door[] door; 

    private void Start()
    {
        StartCoroutine(damageFieldStart());
    }

    private void Update()
    {
        if (upField)
        {
            damageField.transform.position += new Vector3(0, fieldSpeed * Time.deltaTime, 0);

            if (openCount < doorOpenPoints.Length)
            {
                if (damageField.transform.GetChild(0).position.y >= doorOpenPoints[openCount].transform.position.y)
                {
                    openDoor = true;
                }
            }
        }

        if (openDoor)
        {
            openLength += openSpeed * Time.deltaTime;
            door[openCount].leftDoor.transform.position += new Vector3(openSpeed * Time.deltaTime, 0, 0);
            door[openCount].rightDoor.transform.position += new Vector3(-openSpeed * Time.deltaTime, 0, 0);

            if (openLength >= 10)
            {
                openDoor = false;
                openCount++; 
            }
        }
    }

    public IEnumerator damageFieldStart()
    {
        yield return new WaitForSeconds(0.0f);

        upField = true;
    }
}
