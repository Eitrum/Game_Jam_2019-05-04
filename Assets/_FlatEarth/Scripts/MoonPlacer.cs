using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonPlacer : MonoBehaviour
{
    public Transform[] moons;
    public float minMagnitude = 2f;
    public float maxMagnitude = 2f;

    [ContextMenu("RandomizePos")]
    private void RandomizePos() {
        for (int i = 0; i < moons.Length; i++)
        {
            moons[i].position = transform.position + Random.Range(minMagnitude, maxMagnitude) * Random.onUnitSphere;
            moons[i].rotation = Random.rotation;
        }
    }
}
