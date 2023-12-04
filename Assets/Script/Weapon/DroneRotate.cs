using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneRotate : MonoBehaviour
{
    void Update()
    {
        transform.localEulerAngles += new Vector3(0, 10000f * Time.deltaTime, 0);
    }
}
