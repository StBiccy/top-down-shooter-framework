using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class VendingMachnie : MonoBehaviour, IInteractable
{
    enum type
    {
        health,
        pistolAmmo,
        shotgunAmmo,
    }

    [SerializeField] private type machineType;
    [SerializeField] private ushort pointCost;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text costText;

    private void Awake()
    {
        switch (machineType)
        {
            case type.health:
                typeText.SetText("Heal");
                break;
            case type.pistolAmmo:
                typeText.SetText("Pistol Ammo");
                break;
            case type.shotgunAmmo:
                typeText.SetText("Shotgun Ammo");
                break;
        }

        costText.SetText(pointCost + " points");
    }

    // Start is called before the first frame update
    public void Interact(GameObject other)
    {
        if (other.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController refrence = other.GetComponent<PlayerController>();
            if (refrence.GetPoints() >= pointCost)
            {
                refrence.RemovePoints(pointCost);

                switch (machineType)
                {
                    case type.health:
                        refrence.Heal();
                        break;
                    case type.pistolAmmo:
                        refrence.AddPistolMag();
                        break;
                    case type.shotgunAmmo:
                        refrence.AddShotgunMag();
                        break;
                }
            }
        }
    }

}
