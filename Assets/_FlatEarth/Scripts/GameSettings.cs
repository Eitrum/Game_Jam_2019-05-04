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
    [SerializeField] private Player playerPrefab = null;

    [Header("Round Settings")]
    [SerializeField] private float roundDuration = 180f;

    #endregion

    #region Public Properties

    public static Player PlayerPrefab => Instance.playerPrefab;
    public static float RoundDuration => Instance.roundDuration;

    #endregion
}