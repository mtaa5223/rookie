using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using System; //For Serializable
using Photon.Pun.Demo.PunBasics;
using System.Linq;
using TMPro;
/*using static Unity.VisualScripting.Round<TInput, TOutput>;*/

public class GameManager : MonoBehaviour
{
    //GameManager Setting
    public static GameManager instance;
    PhotonView pv;




    [Header("Player Hp Bar")]
    public Image[] playerHpBar;

    public TextMeshProUGUI[] text;

    [Header("Credits")]
    [SerializeField] GameObject roundWin;
    [SerializeField] GameObject roundLose;
    [SerializeField] GameObject victory;
    [SerializeField] GameObject defeat;
    public GameObject credit;

    [SerializeField] private GameObject hitZoneManager;
    private GameObject currenthitZoneManager;



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
    public bool gameEnd;
    private float sceneChangeTime = 0f;

    private void Start()
    {
        currenthitZoneManager = Instantiate(hitZoneManager);

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
            gameEnd = true;
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
        gameEnd = true;
        StartCoroutine(superIdol());
    }

    IEnumerator superIdol()
    {
        if (gameEnd)
        {
            credit.SetActive(true);
            Time.timeScale = 0.1f;

            yield return new WaitForSeconds(0.3f);

            Time.timeScale = 1f;
            PhotonNetwork.LoadLevel(2);
        }
        playerDie = true;
        credit.SetActive(true);
        Time.timeScale = 0.1f;

        yield return new WaitForSeconds(0.3f);

        // 히트 존 재생성
        Destroy(currenthitZoneManager);
        currenthitZoneManager = Instantiate(hitZoneManager);

        // 위치 재설정
        pv.RPC("photonmna", RpcTarget.All);
        Time.timeScale = 1f;

        playerDie = false;
        playerHpBar[0].fillAmount = 1f;
        playerHpBar[1].fillAmount = 1f;
        text[0].text = "20 / 20";
        text[1].text = "20 / 20";

        credit.SetActive(false);
        redOcean.SetActive(false);

        Transform players = IngamePhotonManager.instance.players;
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("잠시후 재생성됩니다");
            players.GetChild(0).transform.position = new Vector3(-11, 6, 14);
            players.GetChild(1).transform.position = new Vector3(-10, 3, 0);
        }
        PlayerHealth.Instance.ResetHp();
    }
    [PunRPC]

    public void photonmna()
    {
        IngamePhotonManager.instance.ResetPlace();

    }
    public GameObject redOcean;
}