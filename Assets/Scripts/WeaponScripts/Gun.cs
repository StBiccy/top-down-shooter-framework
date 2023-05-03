using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour, IWeapon
{
    [Header("Firing Parameters")]
    [SerializeField] protected float distance;
    [SerializeField] protected float spread;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float reloadSpeed;
    [SerializeField] protected byte numProjectiles;
    [SerializeField] protected LayerMask HitLayers;
    private bool fireCD = false;
    private bool reloading = false;

    [Header("Bullet Values")]
    [SerializeField] protected short damage;
    [SerializeField] protected byte magSize;

    [Header("Current Ammo")]
    [SerializeField]protected uint bullets;
    [SerializeField]protected byte magBullets;

    [Header("Debug")]
    [SerializeField] protected bool drawRange;
    [SerializeField] protected bool drawBulletLines;
    [SerializeField] protected bool drawHitPoint;
    [SerializeField, Range(0, 10)] protected float hitObjectWaitTime;
    [SerializeField] protected GameObject hitObject;
    // Start is called before the first frame update
    protected void Start()
    {
        if (magBullets > magSize)
        {
            magBullets = magSize;
        }
    }

    public void FireGun()
    {
        if (magBullets > 0 && !fireCD && !reloading)
        {
            --magBullets;
            for (int i = 0; i < numProjectiles; i++)
            {
                Vector3 fireDir = Quaternion.Euler(0, 0, transform.rotation.z + UnityEngine.Random.Range(-spread, spread)) * transform.right;

                RaycastHit2D hit = Physics2D.Raycast(transform.position, fireDir, distance, HitLayers);

                #region debug
                if (hit && hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    hit.collider.GetComponent<IDamageable>().Hit(damage, IDamageable.hitType.gun);
                }

                if (drawHitPoint)
                {
                    GameObject debug = Instantiate(hitObject, hit.point, Quaternion.identity);
                    debug.GetComponent<DebugObject>().waitTime= hitObjectWaitTime;
                }
                if (drawBulletLines)
                {
                    Debug.DrawRay(transform.position, fireDir * distance, Color.green, 10);
                }
                #endregion
            }
            fireCD = true;
            StartCoroutine(FireCoolDown());
            
        }
        
    }

    public float ReloadGun()
    {
        if (magBullets >= magSize || bullets <= 0)
        {
            return 0;
        }
        else if (bullets >= magSize)
        {
            bullets -= (uint) magSize- magBullets;
            magBullets = magSize;
            StartCoroutine(Relaod());
            return reloadSpeed;
        }
        else
        {
            magBullets += (byte)bullets;
            bullets = 0;
            StartCoroutine(Relaod());
            return reloadSpeed;
        }
    }

    protected void OnDrawGizmos()
    {
        if (drawRange)
        {
            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, transform.rotation.z + spread) * transform.right * distance);
            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, transform.rotation.z - spread) * transform.right * distance);
        }
    }

    private IEnumerator FireCoolDown()
    {
        yield return new WaitForSeconds(fireRate);
        fireCD = false;
    }

    private IEnumerator Relaod()
    {
        yield return new WaitForSeconds(reloadSpeed);
        reloading = false;
    }

    public uint GetBullets() { return bullets;}

    public ushort GetMagBullets() { return magBullets; }
}

