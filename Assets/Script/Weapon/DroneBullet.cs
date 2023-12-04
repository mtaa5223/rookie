using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBullet : MonoBehaviour
{
    [SerializeField] private GameObject boom;

    [HideInInspector] public Transform target;

    private float time = 0f;

    void Update()
    {
        time += Time.deltaTime;
        if (time >= 5f)
        {
            Destroy(gameObject);
        }
        else if (time <= 0.7f)
        {
            // 플레이어 추격
            Quaternion currentAngle = transform.rotation;
            transform.LookAt(target);
            transform.rotation = Quaternion.Slerp(currentAngle, transform.rotation, 100f * Time.deltaTime);
        }

        transform.position += transform.forward * Time.deltaTime * 20f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject _boom = Instantiate(boom);
        _boom.transform.position = transform.position;
        Destroy(gameObject);
    }
}