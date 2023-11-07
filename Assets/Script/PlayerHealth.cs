using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    [SerializeField] public float playerHp = 20;
    public int playerNum = 0; // This should be set based on the PhotonView ID.
    PhotonView pv;
    public Transform players;
    Image hpbar;

    private TextMeshProUGUI text;

    private void Start()
    {
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
            text = GameObject.Find("HpText_1").GetComponent<TextMeshProUGUI>();
        }
        else if (pv.ViewID == 2001) // Assuming 2nd player has PhotonView ID 1002.
        {
            playerNum = 1;
            text = GameObject.Find("HpText_2").GetComponent<TextMeshProUGUI>();
        }
        hpbar = GameManager.instance.playerHpBar[playerNum];
    }
    private void Update()
    {
        Debug.Log(playerHp);
    }
    public void GetDamage(float _damage)
    {
        if (!GameManager.instance.playerDie)
        {
            playerHp -= _damage;

            hpbar.fillAmount = Mathf.Clamp(hpbar.fillAmount - _damage * 0.05f, 0f, 1f);
            pv.RPC("TextRpc", RpcTarget.All);
            //text.text = playerHp.ToString() + " / 20";

            if (playerHp <= 0)
            {
                GameManager.instance.PlayerDie(playerNum);
            }
        }
    }
    [PunRPC]
    public void TextRpc()
    {
        text.text = playerHp.ToString() + " / 20";
    }
    public void ResetHp()
    {
        pv.RPC("Hp", RpcTarget.All);
    }
    [PunRPC]
    public void Hp()
    {
        playerHp = 20;
        text.text = playerHp.ToString() + " / 20";
    }
}
