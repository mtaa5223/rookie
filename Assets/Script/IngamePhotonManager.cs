using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IngamePhotonManager : MonoBehaviour
{
    public Transform players;
    public bool playerCheck;
    public GameObject player;
    public GameObject playerClone;
    public static IngamePhotonManager instance = null;
    public Transform[] spawnpoints;
    public Text text;

    PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (null == instance)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Vector3 spawnPosition = spawnpoints[0].position;
        player = PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartCoroutine(GameStart());
        }

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
            playerClone = GameObject.Find("Player(Clone)");
            playerClone.transform.parent = players;
            playerClone.GetComponentInChildren<Camera>().enabled = false;
        }
    }

    IEnumerator GameStart()
    {
        text.text = "5초후 게임이 시작됩니다";
        yield return new WaitForSeconds(3);

        for (int i = 5; i > 0; i--)
        {
            text.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        text.text = "게임 시작";
        yield return new WaitForSeconds(1);
        text.text = "";
    }

    [PunRPC]
    public void ResetPlace()
    {
        pv.RPC("rawd", RpcTarget.All);
    }

    [PunRPC]
    public void rawd()
    {
        Vector3 playerPosition = player.transform.position;
        player.transform.position = playerClone.transform.position;
        playerClone.transform.position = playerPosition;
    }
}