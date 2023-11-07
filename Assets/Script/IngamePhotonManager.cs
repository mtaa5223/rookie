using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IngamePhotonManager : MonoBehaviour
{
    public Transform players;
    public bool playerCheck;
    public static IngamePhotonManager instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    } 
    private void Start()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", transform.position, Quaternion.identity);
        players = GameObject.Find("Players").transform;
        player.transform.parent = players;
        if (player.GetComponent<PhotonView>().IsMine)
        {
            int index = player.transform.name.IndexOf('(');
            player.transform.name = player.transform.name.Substring(0, index);
        }
    }

    private void Update()
    {
        if (GameObject.Find("Player(Clone)") && !playerCheck)
        {
            playerCheck = true;
            GameObject playerClone = GameObject.Find("Player(Clone)");

            playerClone.transform.parent = players;
            playerClone.GetComponentInChildren<Camera>().enabled = false;
        }
    }
}
