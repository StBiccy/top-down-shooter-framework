using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Path Finding")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform player;

    [Header("Enemy Values")]
    [SerializeField] private short baseHitPoints = 10;
    [SerializeField] private float knifeImmunityTime;
    private bool canKnife = true;
    private short hitPoints;

    [Header("Debug")]
    [SerializeField] private bool healthDebug;
    [SerializeField] private GameObject healthDebugText;

    void Awake()
    {
        hitPoints = baseHitPoints;
    }

    void Update()
    {
        agent.SetDestination(player.position);
        
        transform.rotation = Quaternion.Euler(0, 0, 0);

        HealthDebug();
    }

    public void Hit(short damage, IDamageable.hitType type)
    {
        if (type == IDamageable.hitType.knife )
        {
            if (canKnife)
            {
                hitPoints -= damage;
                canKnife = false;
                StartCoroutine(KnifeHitImmunity());
            }
        }
        else
        {
            hitPoints -= damage;
        }

        if (hitPoints < 0)
        {
            Destroy(gameObject);
        }
    }

    private void HealthDebug()
    {
        if (healthDebug)
        {
            if (!healthDebugText.activeSelf)
            {
                healthDebugText.SetActive(true);
            }
            healthDebugText.GetComponent<TMP_Text>().SetText(hitPoints + " / " + baseHitPoints);
        }
        else
        {
            if (healthDebugText.activeSelf)
            {
                if (healthDebugText.activeSelf) ;
            }
        }
    }


    private IEnumerator KnifeHitImmunity()
    {
        yield return new WaitForSeconds(knifeImmunityTime);
        canKnife = true;
    }
}