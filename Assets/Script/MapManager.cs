using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject playerHitZone;
    [SerializeField] private float hitZoneSpeed;
    [SerializeField] private float startTime;

    private bool isStart;

    void Start()
    {
        StartCoroutine(StartHitZone());
    }

    void Update()
    {
        if (isStart)
        {
            playerHitZone.transform.Translate(new Vector3(0, hitZoneSpeed * Time.deltaTime, 0));
        }
    }

    IEnumerator StartHitZone()
    {
        yield return new WaitForSeconds(startTime);

        isStart = true;
    }
}
