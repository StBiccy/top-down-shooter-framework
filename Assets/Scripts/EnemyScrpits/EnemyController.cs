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
    [SerializeField] private uint pointsWorth;
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

    public void Hit(short damage, IDamageable.hitType type, GameObject deltBy)
    {
        if (type == IDamageable.hitType.knife )
        {
            if (canKnife)
            {
                hitPoints -= damage;
                canKnife = false;
                if (hitPoints <= 0)
                {
                    deltBy.GetComponent<PlayerController>().AddPoints(pointsWorth*2);
                    Destroy(gameObject);
                }
                StartCoroutine(KnifeHitImmunity());
            }
        }
        else
        {
            hitPoints -= damage;
            if (hitPoints <= 0)
            {
                deltBy.GetComponent<PlayerController>().AddPoints(pointsWorth);
                Destroy(gameObject);
            }
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
                healthDebugText.SetActive(false);
            }
        }
    }


    private IEnumerator KnifeHitImmunity()
    {
        yield return new WaitForSeconds(knifeImmunityTime);
        canKnife = true;
    }

    public void SetPTransform(Transform transform)
    {
        player = transform;
    }
}
