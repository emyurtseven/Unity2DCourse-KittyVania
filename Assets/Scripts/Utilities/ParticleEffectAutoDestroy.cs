using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectAutoDestroy : MonoBehaviour
{

    float timer = 2f;

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= 2 * Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
