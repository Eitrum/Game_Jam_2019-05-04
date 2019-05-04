using UnityEngine;
using Eitrum.Engine.Extensions;

public class GameManager : MonoBehaviour {

    private static Transform parent;
    public static Transform Parent => parent;

    private static Player PlayerPrefab => GameSettings.PlayerPrefab;

    public static event System.Action OnRestart;

    void Awake() {
        parent = transform;
    }

    public static void Restart() {
        Parent.DestroyAllChildren();

        OnRestart?.Invoke();
    }
}
