using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugObject : MonoBehaviour
{
    public float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(wait());
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
