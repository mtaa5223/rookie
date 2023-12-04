using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBoom : MonoBehaviour
{
    private float timer = 0f;

    private bool hited = false;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 0.1f)
        {
            hited = true;
            GetComponent<SphereCollider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hited && other.CompareTag("Player"))
        {
            other.transform.GetComponent<Health>().GetDamage(3f);
            StartCoroutine(KnockBack(other.transform));
            hited = true;
        }
    }

    IEnumerator KnockBack(Transform player)
    {
        float power = 100f;
        transform.LookAt(player);
        while (power > 1f)
        {
            Vector3 moveAngle = transform.forward;
            PlayerKnockBack(player, power, new Vector3(moveAngle.x, 0, 0));
            PlayerKnockBack(player, power, new Vector3(0, moveAngle.y, 0));
            PlayerKnockBack(player, power, new Vector3(0, 0, moveAngle.z));
            power -= 200f * Time.deltaTime;
            yield return null;
        }
    }

    private void PlayerKnockBack(Transform player, float power, Vector3 moveAngle)
    {
        RaycastHit hit;
        if (!Physics.Raycast(player.position, moveAngle, power * Time.deltaTime))
        {
            player.position += moveAngle * power * Time.deltaTime;
        }
    }
}