using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class DroneHealth : Health
{
    [SerializeField] public float droneHp = 3;
    [SerializeField] private GameObject boom;
    [SerializeField] private GameObject deadDrone;


    PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }
    public override void GetDamage(float _damage)
    {
        if (!GameManager.instance.playerDie && !GameManager.instance.gameEnd)
        {

            droneHp -= _damage;
            if (droneHp <= 0)
            {
                pv.RPC("dronehps", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    public void dronehps()
    {
        GameObject _deadDrone = Instantiate(deadDrone);
        _deadDrone.transform.position = transform.position;

        GameObject _boom = Instantiate(boom);
        _boom.transform.position = transform.position;

        _boom.transform.parent = _deadDrone.transform;

        gameObject.SetActive(false);
    }
}