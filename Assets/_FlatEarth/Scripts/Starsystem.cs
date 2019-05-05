using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eitrum;
using Eitrum.Engine;
using Eitrum.Engine.Core;

public class Starsystem : EiComponent
{

    public int stars = 1000;
    public Mesh mesh;
    public Material material;

    [Header("Area")]
    public Bounds bounds;

    [Header("Scale")]
    public float minScale = 0.02f;
    public float maxScale = 0.1f;

    [Header("Offset Rotation")]
    public Vector3 offsetRotation = new Vector3(90, 0, 0);

    private Matrix4x4[] matrices;

    void Start() {
        matrices = new Matrix4x4[stars];

        Quaternion offset = Quaternion.Euler(offsetRotation);

        for (int i = 0; i < stars; i++) {
            var scale = Random.Range(minScale, maxScale);

            var min = bounds.min;
            var max = bounds.max;
            var position = new Vector3(
                Random.Range(min.x, max.x), 
                Random.Range(min.y, max.y), 
                Random.Range(min.z, max.z));

            matrices[i] = Matrix4x4.TRS(position, offset, new Vector3(scale, scale, scale));
        }
    }

    void OnEnable() {
       // SubscribeThreadedUpdate();
        SubscribeLateUpdate();
    }

    void OnDisable() {
       // UnsubscribeThreadedUpdate();
        UnsubscribeLateUpdate();
    }

    public override void ThreadedUpdateComponent(float time) {
        
    }

    public override void LateUpdateComponent(float time) {
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices);
    }
}
