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

    [SerializeField] private Player playerPrefab = null;

    #endregion

    #region Public Properties

    public static Player PlayerPrefab => Instance.playerPrefab;

    #endregion
}