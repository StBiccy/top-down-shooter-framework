using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    enum gun
    { 
        pistol,
        shotGun,
    }

    [Header("Player Values")]
    [SerializeField] private short baseHitPoints;
    [SerializeField] private float speed = 5;
    private short hitPoints;
    private Vector2 direction;
    private Vector2 mouseWorldPos;
    private Vector2 mouseLocalPos;
    private Rigidbody2D rb;

    [Header("Guns")]
    [SerializeField] private gun gunSelect;
    [SerializeField] private IWeapon currentGun;

    [Header("Knife")]
    [SerializeField] private GameObject knifeObject;
    [SerializeField, Range(0,2)] private float knifeCD;
    private bool knifeWait = false;

    [Header("UI")]
    [SerializeField] private GameObject gunText;
    [SerializeField] private GameObject ammoText;
    [SerializeField] private GameObject hitPointText;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 

        hitPoints = baseHitPoints;

        if (gunSelect == gun.shotGun)
        {
            currentGun = GetComponent<ShotGun>() as IWeapon;
            gunText.GetComponent<TMP_Text>().SetText("Shot Gun");
        }
        else if (gunSelect == gun.pistol)
        {
            currentGun = GetComponent<Pistol>() as IWeapon;
            gunText.GetComponent<TMP_Text>().SetText("Pistol");
        }

    }

    private void Start()
    {
        AmmoTextUpdate();
        HealthTextUpdate();
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
                gunText.GetComponent<TMP_Text>().SetText("Pistol");
            }
            else if ((int)gunSelect == 1)
            {
                currentGun = GetComponent<ShotGun>();
                gunText.GetComponent<TMP_Text>().SetText("Shot Gun");
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

    private void AmmoTextUpdate()
    {
        ammoText.GetComponent<TMP_Text>().SetText(currentGun.GetMagBullets() + " / " + currentGun.GetBullets());
    }

    private void HealthTextUpdate()
    {
        hitPointText.GetComponent<TMP_Text>().SetText("HP: " + hitPoints + " / " + baseHitPoints);
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
