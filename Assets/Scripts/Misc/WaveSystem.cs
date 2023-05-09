using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform player;

    [SerializeField] private GameObject[] spawners;
    [SerializeField] private float spawnInterval;
    private uint wave = 0;

    [SerializeField] private TMP_Text waveText;
    private uint leftToSpawn;
    private uint enemiesInScene;

    private void Start()
    {
        NextWave();
    }

    void NextWave()
    {
        ++wave;

        leftToSpawn = wave;
        enemiesInScene= leftToSpawn;

        waveText.SetText("Wave: " + wave);

        StartCoroutine(spawnCD());
    }

    void spawn()
    {
        --leftToSpawn;


        var spawn = Instantiate(enemy, spawners[Random.Range(0, spawners.Length)].transform.position,Quaternion.identity);

        spawn.GetComponent<EnemyController>().SetPTransform(player);
        spawn.AddComponent<WaveInformant>().waveSystem = this;
        
        if(leftToSpawn != 0)
        {
            StartCoroutine(spawnCD());
        }
    }

    private IEnumerator spawnCD()
    {
        yield return new WaitForSeconds(spawnInterval);
        spawn();
    }



    public void EnemyDeath()
    {
        if(--enemiesInScene <= 0)
        {
            NextWave();
        }
    }

}
