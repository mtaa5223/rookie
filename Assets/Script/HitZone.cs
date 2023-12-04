using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Photon.Pun;
public class HitZone : MonoBehaviour
{
    [SerializeField] private float hitTime;
    [SerializeField] private float damage;

    private Coroutine hitCoroutine;

    PhotonView pv;
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }
    void Update()
    {

    }

    IEnumerator PlayerHit(Health playerHealth)
    {

        playerHealth.GetDamage(damage);
        yield return new WaitForSeconds(hitTime);

        hitCoroutine = StartCoroutine(PlayerHit(playerHealth));


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerHealth>() != null)
            {
                hitCoroutine = StartCoroutine(PlayerHit(other.GetComponent<PlayerHealth>()));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine(hitCoroutine);
        }
    }
}
