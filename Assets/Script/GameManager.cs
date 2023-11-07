using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using System; //For Serializable
using Photon.Pun.Demo.PunBasics;
using System.Linq;
/*using static Unity.VisualScripting.Round<TInput, TOutput>;*/

public class GameManager : MonoBehaviour
{
    //GameManager Setting
    public static GameManager instance;
    PhotonView pv;




    [Header("Player Hp Bar")]
    public Image[] playerHpBar;

    [Header("Credits")]
    [SerializeField] GameObject roundWin;
    [SerializeField] GameObject roundLose;
    [SerializeField] GameObject victory;
    [SerializeField] GameObject defeat;
    public GameObject credit;



    //Score Circles
    [Serializable]
    public struct Circle
    {
        public Image[] circle;
    }
    [Header("Score Circles")]
    public Circle[] circles;
    public Sprite circleOnPoint;

    public int playerScore = 0;

    public bool playerDie = false;
    private float sceneChangeTime = 0f;

    private void Start()
    {

        pv = GetComponent<PhotonView>();
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);    
        }
        else
        {
            Destroy(gameObject);
        }

    }


    private void Update()
    {

        //if (playerDie)
        //{
        //    sceneChangeTime += Time.deltaTime;
        //    credit.GetComponent<Image>().color = new Vector4(1, 1, 1, Mathf.Clamp01(sceneChangeTime * 0.5f));
        //    if (sceneChangeTime >= 0.3f)
        //    {
        //        playerHpBar[0].fillAmount = 1f;
        //        playerHpBar[1].fillAmount = 1f;

        //        credit.SetActive(false);

        //        Transform players = IngamePhotonManager.instance.players;
        //        players.GetChild(0).transform.position = new Vector3(0, 3, 0);
        //        players.GetChild(1).transform.position = new Vector3(-3, 3, 0);

        //        PlayerHealth.Instance.playerHp = 20;

        //        Time.timeScale = 1f;
        //    }
        //}
    }


    public void PlayerDie(int _playerNum)
    {
        if (_playerNum == PlayerHealth.Instance.playerNum)
        {
            int winPlayerNum;
            if (_playerNum == 0)
            {
                winPlayerNum = 1;
            }
            else
            {
                winPlayerNum = 0;
            }
            pv.RPC("Game", RpcTarget.Others);
            pv.RPC("Circle1", RpcTarget.All, winPlayerNum, whatWin++);
            playerDie = true;

        }
    }
    int whatWin;
    public Sprite circleWhite;
    [PunRPC]
    public void Circle1(int winPlayerNum, int whatWIn)
    {
        Debug.Log(winPlayerNum + " " + whatWIn);
        circles[winPlayerNum].circle[whatWIn].sprite = circleWhite;
    }
    [PunRPC]
    public void Game()
    {
        ++playerScore;
        Debug.Log(playerScore);
        if (playerScore < 2)
        {
            credit = roundWin;
            pv.RPC("Lose", RpcTarget.Others);
        }
        else
        {
            credit = victory;
            pv.RPC("Defeat", RpcTarget.Others);
        }
        StartCoroutine(superIdol());
    }
    [PunRPC]
    public void Lose()
    {
        credit = roundLose;
        StartCoroutine(superIdol());
    }
    [PunRPC]
    public void Defeat()
    {
        credit = defeat;
        StartCoroutine(superIdol());
    }

    IEnumerator superIdol()
    {
        playerDie = true;
        credit.SetActive(true);
        Time.timeScale = 0.1f;

        yield return new WaitForSeconds(0.3f);

        Time.timeScale = 1f;

        playerDie = false;
        playerHpBar[0].fillAmount = 1f;
        playerHpBar[1].fillAmount = 1f;

        credit.SetActive(false);

        Transform players = IngamePhotonManager.instance.players;
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("잠시후 재생성됩니다");
            players.GetChild(0).transform.position = new Vector3(5, 3, 0);
            players.GetChild(1).transform.position = new Vector3(-5, 3, 0);
        }
        PlayerHealth.Instance.ResetHp();
    }
}