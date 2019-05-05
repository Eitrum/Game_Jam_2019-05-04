using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eitrum.Engine.Core;
using Eitrum;
using Eitrum.Engine;

public class MoonPlacer : EiComponent
{
    public Transform[] moons;
    public float minMagnitude = 2f;
    public float maxMagnitude = 2f;

    EiLLNode<IThreadedUpdate> threadedUpdate;

    [Header("Moon Settings")]
    public int moonCount = 0;
    public Mesh moonMesh;
    public Material moonMaterial;
    [Header("Scale")]
    public float minScale = 0.1f;
    public float maxScale = 0.3f;
    [Header("Distance")]
    public float minDistance = 1.05f;
    public float maxDistance = 1.3f;
    [Header("Rotation Speed")]
    public float minSpeed = 1f;
    public float maxSpeed = 15f;

    // Cache
    private Vector3 lastPosition;
    private Matrix4x4[] matrixes;
    private float[] distances;
    private Quaternion[] rotations;
    private Quaternion[] speed;
    private Quaternion[] currentRotations;
    private Vector3[] scales;
    
    [ContextMenu("RandomizePos")]
    private void RandomizePos() {
        for (int i = 0; i < moons.Length; i++)
        {
            moons[i].position = transform.position + Random.Range(minMagnitude, maxMagnitude) * Random.onUnitSphere;
            moons[i].rotation = Random.rotation;
        }
    }

    void Awake() {
        rotations = new Quaternion[moonCount];
        speed = new Quaternion[moonCount];
        currentRotations = new Quaternion[moonCount];
        scales = new Vector3[moonCount];
        distances =new float[moonCount];
        matrixes = new Matrix4x4[moonCount];

        for (int i = 0; i < moonCount; i++) {
            rotations[i] = Random.rotationUniform;
            var scale = Random.Range(minScale, maxScale);
            scales[i] = new Vector3(scale, scale, scale);
            distances[i] = Random.Range(minDistance, maxDistance);
            speed[i] = Quaternion.Euler(
                Random.Range(minSpeed, maxSpeed), 
                Random.Range(minSpeed, maxSpeed), 
                Random.Range(minSpeed, maxSpeed));

            currentRotations[i] = Random.rotationUniform;
        }
    }

    void OnEnable() {
        SubscribePreUpdate();
        SubscribeThreadedUpdate();
        SubscribeLateUpdate();
    }

    void OnDisable() {
        UnsubscribePreUpdate();
        UnsubscribeThreadedUpdate();
        UnsubscribeLateUpdate();
    }

    public override void PreUpdateComponent(float time) {
        lastPosition = this.transform.localPosition;
    }

    public override void LateUpdateComponent(float time) {
        Graphics.DrawMeshInstanced(moonMesh, 0, moonMaterial, matrixes);
    }

    public override void ThreadedUpdateComponent(float time) {
        var position = lastPosition;
        for (int i = 0; i < moonCount; i++) {
            currentRotations[i] *= Quaternion.Slerp(Quaternion.identity, speed[i], time);
            var tPosition = position + currentRotations[i] * new Vector3(0, 0, distances[i]);
            matrixes[i] = Matrix4x4.TRS(tPosition, rotations[i], scales[i]);
        }
    }
}
