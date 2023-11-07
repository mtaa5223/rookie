using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float lineDeleteTime = 0f;

    void Update()
    {
        lineDeleteTime += Time.deltaTime;
        if (lineDeleteTime > 0.01f)
        {
            Destroy(gameObject);
        }
    }
} 
