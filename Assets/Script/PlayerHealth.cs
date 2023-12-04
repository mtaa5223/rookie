using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerHealth : Health
{
    public static PlayerHealth Instance;
    [SerializeField] public float playerHp = 20;
    public int playerNum = 0;
    public int otherPlayerNum;
    public PhotonView pv;
    public Transform players;
    public Image hpbar;
    public Image otherHpbar;
    public GameObject redOcean;
    private TextMeshProUGUI text;

    private void Start()
    {
        redOcean = GameManager.instance.redOcean;
        if (Instance == null)
        {
            Instance = this;
        }
        pv = GetComponent<PhotonView>();
        players = GameObject.Find("Players").transform;

        // Set playerNum based on PhotonView ID.
        if (pv.ViewID == 1001) // Assuming 1st player has PhotonView ID 1001.
        {
            playerNum = 0;
            otherPlayerNum = 1;
            //text = GameObject.Find("HpText_1").GetComponent<TextMeshProUGUI>();
        }
        else if (pv.ViewID == 2001) // Assuming 2nd player has PhotonView ID 2001.
        {
            playerNum = 1;
            otherPlayerNum = 0;
            //text = GameObject.Find("HpText_2").GetComponent<TextMeshProUGUI>();
        }
        //hpbar = GameManager.instance.playerHpBar[playerNum];
        //otherHpbar = GameManager.instance.playerHpBar[otherPlayerNum];
    }

    //private void Update()
    //{
    //    pv.RPC("OtherPlayerHp", RpcTarget.Others);//, playerHp);
    //}
    //[PunRPC]
    //void OtherPlayerHp()//float hp)
    //{
    //    //Debug.Log(hp);
    //    Debug.Log(otherHpbar.fillAmount);
    //    //otherHpbar.fillAmount = .1f;
    //}
    public override void GetDamage(float _damage)
    {
        if (!GameManager.instance.playerDie)
        {
            if (pv.IsMine)
            {
                if (playerHp > 0)
                {
                    pv.RPC("playerhpup", RpcTarget.All, _damage);
                }
                //text.text = playerHp.ToString() + " / 20";
                if (playerHp < 10)
                {
                    redOcean.SetActive(true);
                }
                if (playerHp <= 0)
                {
                    GameManager.instance.PlayerDie(playerNum);
                }
            }
        }
    }
    [PunRPC]
    public void playerhpup(float _damage)
    {
        playerHp -= _damage;
        GameManager.instance.playerHpBar[pv.ViewID == 1001 ? 0 : 1].fillAmount = playerHp / 20f;
        GameManager.instance.text[pv.ViewID == 1001 ? 0 : 1].text = playerHp.ToString() + " / 20";
    }
    public void ResetHp()
    {
        pv.RPC("Hp", RpcTarget.All);
    }
    [PunRPC]
    public void Hp()
    {
        playerHp = 20;
    }
}
