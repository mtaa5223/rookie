using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DroneDead : MonoBehaviour
{
    private Rigidbody rigidbody;

    PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        pv.RPC("dead", RpcTarget.All);
    }
    [PunRPC]

    public void dead()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-10f, 10f), Random.Range(-1f, 1f)), ForceMode.Impulse);
    }
}