using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CanvasEnable : MonoBehaviour
{
    PhotonView pv;
    public GameObject Canvas;
    // Start is called before the first frame update
    void Start()
    {   
     pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            Canvas.SetActive(true);
        }
        else
        {
            Canvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
