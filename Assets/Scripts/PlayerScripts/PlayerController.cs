using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static IDamageable;

public class PlayerController : MonoBehaviour, IDamageable
{
    enum gun
    {
        pistol,
        shotGun,
    }

    [Header("Player Values")]
    [SerializeField] private short baseHitPoints;
    [SerializeField] private float speed = 5;
    [SerializeField] private uint points;
    private short hitPoints;
    private Vector2 direction;
    private Vector2 mouseWorldPos;
    private Vector2 mouseLocalPos;
    private Rigidbody2D rb;

    [SerializeField] private float immuneTime;
    private bool immune = false;

    private GameObject interactionObject;


    [Header("Guns")]
    [SerializeField] private gun gunSelect;
    [SerializeField] private IWeapon currentGun;

    [Header("Knife")]
    [SerializeField] private GameObject knifeObject;
    [SerializeField, Range(0, 2)] private float knifeCD;
    private bool knifeWait = false;

    [Header("UI")]
    [SerializeField] private TMP_Text gunText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text hitPointText;
    [SerializeField] private TMP_Text pointText;
    [SerializeField] private GameObject deathScreen;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 

        hitPoints = baseHitPoints;

        if (gunSelect == gun.shotGun)
        {
            currentGun = GetComponent<ShotGun>() as IWeapon;
            gunText.SetText("Shot Gun");
        }
        else if (gunSelect == gun.pistol)
        {
            currentGun = GetComponent<Pistol>() as IWeapon;
            gunText.SetText("Pistol");
        }

    }

    private void Start()
    {
        AmmoTextUpdate();
        HealthTextUpdate();
        PointTextUpdate();
    }

    private void FixedUpdate()
    {
        rb.velocity = direction * speed;

        mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseLocalPos);
        Vector2 facingDiretion = mouseWorldPos - (Vector2)transform.position;
        float angle = Mathf.Atan2(facingDiretion.y,facingDiretion.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    public void OnMousePos(InputAction.CallbackContext context)
    {
        mouseLocalPos = context.ReadValue<Vector2>();
        mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseLocalPos);
    }

    public void OnWeaponSwap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ++gunSelect;

            if ((int)gunSelect > 1)
            {
                gunSelect = 0;
                currentGun = GetComponent<Pistol>();
                gunText.SetText("Pistol");
            }
            else if ((int)gunSelect == 1)
            {
                currentGun = GetComponent<ShotGun>();
                gunText.SetText("Shot Gun");
            }
            AmmoTextUpdate();
        }
    }

    public void OnWeaponAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentGun.FireGun();
            AmmoTextUpdate();
        }
    }

    public void OnMeleeAttack(InputAction.CallbackContext context)
    {
        if(context.performed && !knifeWait) 
        {
            knifeObject.SetActive(true);
            knifeWait = true;
            StartCoroutine(KinfeCoolDown()); 
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            StartCoroutine(AmmoUpdateDelay(currentGun.ReloadGun()));
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.performed && interactionObject != null) 
        {
            interactionObject.GetComponent<IInteractable>().Interact(gameObject);
        }
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if(context.performed) 
        {
            Time.timeScale= 1.0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void AddPoints(uint val)
    {
        points += val;
        PointTextUpdate();
    }

    public void RemovePoints(uint val) 
    { 
        points -= val; 
        PointTextUpdate();
    }

    public uint GetPoints() { return points; }

    public void Heal()
    {
        hitPoints = baseHitPoints;
        HealthTextUpdate();
    }

    public void AddShotgunMag()
    {
        GetComponent<ShotGun>().AddMag();
        AmmoTextUpdate();
    }

    public void AddPistolMag()
    {
        GetComponent<Pistol>().AddMag();
        AmmoTextUpdate();
    }

    private void AmmoTextUpdate()
    {
        ammoText.SetText(currentGun.GetMagBullets() + " / " + currentGun.GetBullets());
    }

    private void HealthTextUpdate()
    {
        hitPointText.SetText("HP: " + hitPoints + " / " + baseHitPoints);
    }

    private void PointTextUpdate()
    {
       pointText.SetText("Points: " + points);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            interactionObject = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            interactionObject = null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Hit(1, IDamageable.hitType.enemy, collision.gameObject);
        }
    }

    public void Hit(short damage, hitType type, GameObject deltBy)
    {
        if (!immune)
        {
            hitPoints -= damage;
            HealthTextUpdate();
            if (hitPoints <= 0)
            {
                deathScreen.SetActive(true);
                Time.timeScale = 0.0f;
            }
            else
            {
                immune = true;
                StartCoroutine(DamageImmunity());
            }
        }

    }
    private IEnumerator DamageImmunity()
    {
        yield return new WaitForSeconds(immuneTime);
        immune = false;
    }

    private IEnumerator KinfeCoolDown()
    {
        yield return new WaitForSeconds(knifeCD);
        knifeObject.SetActive(false);
        knifeWait = false;
    }
    private IEnumerator AmmoUpdateDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AmmoTextUpdate();
    }
}
