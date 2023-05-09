using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveInformant : MonoBehaviour
{
    public WaveSystem waveSystem;

    private void OnDestroy()
    {
        waveSystem.EnemyDeath();
    }
}
