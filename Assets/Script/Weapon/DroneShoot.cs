using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneShoot : MonoBehaviour
{
    [SerializeField] private Transform firePos;

    [SerializeField] private GameObject bullet;

    [SerializeField] private GameObject readyBullet;

    private float coolTime = 0;

    void Update()
    {
        coolTime += Time.deltaTime;

        if (coolTime <= 1f)
        {
            readyBullet.SetActive(false);
        }
        else
        {
            readyBullet.SetActive(true);
        }
    }

    public void Shoot(Transform _target)
    {
        if (coolTime >= 3f)
        {
            GameObject _bullet = Instantiate(bullet);
            _bullet.transform.position = firePos.position;
            _bullet.transform.eulerAngles = firePos.eulerAngles;
            _bullet.GetComponent<DroneBullet>().target = _target;

            coolTime = 0;
        }
    }
}