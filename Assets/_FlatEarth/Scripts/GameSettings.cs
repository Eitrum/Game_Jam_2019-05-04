using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Settings", menuName = "Flat Earth/Game Settings", order = 0)]
public class GameSettings : ScriptableObject {

    #region Singleton

    private static GameSettings instance;
    private static GameSettings Instance {
        get {
            if (!instance) {
                instance = Resources.Load<GameSettings>("Game Settings");
            }
            return instance;
        }
    }

    #endregion

    #region Variables

    [Header("Prefabs")]
    [SerializeField] private Player[] playerPrefabs = null;
    [SerializeField] private float spawnDistance = 3f;

    [Header("Round Settings")]
    [SerializeField] private float roundDuration = 180f;
    [SerializeField] private AnimationCurve platformScaleCurve = AnimationCurve.Linear(0, 1f, 1f, 0f);

    #endregion

    #region Public Properties

    public static Player PlayerPrefab(int index) => Instance.playerPrefabs[index%Instance.playerPrefabs.Length];
    public static float SpawnDistance => Instance.spawnDistance;
    public static float RoundDuration => Instance.roundDuration;
    public static float Scale(float roundTimer) => Instance.platformScaleCurve.Evaluate(1f - roundTimer / RoundDuration);

    #endregion
}