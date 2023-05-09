using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    enum hitType
    {
        gun,
        knife,
        enemy,
    }

    void Hit(short damage, hitType type, GameObject deltBy);
}
