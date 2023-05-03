using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void FireGun();
    float ReloadGun();
    ushort GetMagBullets();
    uint GetBullets();
}
