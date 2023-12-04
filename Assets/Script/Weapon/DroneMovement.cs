using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Properties;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    [SerializeField] private DroneShoot droneShoot;

    [SerializeField] private Transform firePos;

    [SerializeField] private Transform[] targets = new Transform[2];

    [SerializeField] private Transform[] movePos;

    PhotonView pv;
    private int movingCount = 0;
    public IngamePhotonManager photon;
    private Transform detectTarget;

    private bool detected = false;

    void Start()
    {
        pv = GetComponent<PhotonView>();



    }


    void Update()
    {
        if (!detected && !(IngamePhotonManager.instance.playerClone is null))
        {
            targets[0] = IngamePhotonManager.instance.player.transform;
            Debug.Log(targets[0].gameObject);
            targets[1] = IngamePhotonManager.instance.playerClone.transform;
            Debug.Log(targets[1].gameObject);

            detected = true;
        }
        if (detected)
        {
            DetectPlayer();
        }

        if (!(detectTarget is null))
        {
            Quaternion currentAngle = transform.rotation;
            transform.LookAt(detectTarget);
            transform.rotation = Quaternion.Slerp(currentAngle, transform.rotation, Time.deltaTime * 5f);
            droneShoot.Shoot(detectTarget);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, movePos[movingCount].position, Time.deltaTime * 5f);
            if (Vector3.Distance(transform.position, movePos[movingCount].position) < 0.1f)
            {
                movingCount++;
                if (movingCount >= movePos.Length)
                {
                    movingCount = 0;
                }
            }
        }
        //transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 5f);
    }

    private void DetectPlayer()
    {
        detectTarget = null;
        for (int i = 0; i < targets.Length; ++i)
        {
            // 방향 설정
            Vector3 currentAngle = transform.eulerAngles;
            transform.LookAt(targets[i].position + new Vector3(0, 0.5f, 0));

            // 레이 발사
            int layerMask = ~(1 << LayerMask.NameToLayer("CastUnable"));
            RaycastHit hit;
            if (detectTarget is null && Physics.Raycast(firePos.position, firePos.forward, out hit))
            {
                Debug.DrawLine(firePos.position, hit.point);

                if (hit.transform.CompareTag("Player"))
                {
                    Debug.Log(hit.transform);
                    detectTarget = hit.transform;
                }
            }

            // 오브젝트 방향 복구
            transform.eulerAngles = currentAngle;
        }
    }
}