using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private float hp = 100;
    private float currentHp;

    [SerializeField] private float fieldDamage = 0.1f;

    public void PlayerHit(float damage)
    {
        currentHp -= damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DamageField"))
        {
            StartCoroutine(InDamageField());
        }
    }

    public IEnumerator InDamageField()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerHit(fieldDamage);

        StartCoroutine(InDamageField());
    }
}
