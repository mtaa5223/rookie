using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MainMenuPhotonManager : MonoBehaviourPunCallbacks
{
    InputField m_InputField;
    Text m_textConnectLog;
    Text m_textPlayerList;
    Button StartButton;
    PhotonView PV;

    void Start()
    {
       
        Screen.SetResolution(1920, 1080, false);

        m_InputField = GameObject.Find("Canvas/InputField").GetComponent<InputField>();
        m_textPlayerList = GameObject.Find("Canvas/TextPlayerList").GetComponent<Text>();
        m_textConnectLog = GameObject.Find("Canvas/TextConnectLog").GetComponent<Text>();
        StartButton = GameObject.Find("Canvas/StartButton").GetComponent<Button>();
        
        PV = GetComponent<PhotonView>();        
        
        m_textConnectLog.text = "���ӷα�\n";
        StartButton.interactable = false;
    }
    

    public override void OnConnectedToMaster()
    {
       
      

        PhotonNetwork.LocalPlayer.NickName = m_InputField.text;
        PhotonNetwork.JoinOrCreateRoom("Room1", null, null);

    }
    public override void OnJoinedRoom()
    {
        updatePlayer();
        m_textConnectLog.text += m_InputField.text;
        m_textConnectLog.text += " ���� �濡 �����Ͽ����ϴ�.\n";
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
            PV.RPC("StartButtonActivate", RpcTarget.All);
        }
    }
    [PunRPC]
    public void StartButtonActivate()
    {
        StartButton.interactable = true;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        updatePlayer();
        m_textConnectLog.text += newPlayer.NickName;
        m_textConnectLog.text += " ���� �����Ͽ����ϴ�.\n";
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        updatePlayer();
        m_textConnectLog.text += otherPlayer.NickName;
        m_textConnectLog.text += " ���� �����Ͽ����ϴ�.\n";
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public void GameReady()
    {
        PV.RPC("GameStart", RpcTarget.All); //RPC �Լ� ȣ�� 
    }
 
    [PunRPC]
    public void GameStart()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    void updatePlayer()
    {
        m_textPlayerList.text = "������";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            m_textPlayerList.text += "\n";
            m_textPlayerList.text += PhotonNetwork.PlayerList[i].NickName;
        }
    }

}