using UnityEngine;
using Eitrum.Engine.Extensions;

public class GameManager : MonoBehaviour {

    #region Variables

    private static Transform parent;
    private static float roundTimer = 0f;

    public static event System.Action OnRoundStart;
    public static event System.Action<Player> OnRoundEnd;
    public static event System.Action OnRestart;

    #endregion

    #region Properties

    public static Transform Parent => parent;

    private static Player PlayerPrefab => GameSettings.PlayerPrefab;

    #endregion

    #region Unity Methods

    void Awake() {
        parent = transform.Find("Parent");
    }

    void Update() {
        if (roundTimer > 0f) {
            roundTimer -= Time.deltaTime;
            if (roundTimer <= 0f) {
                OnRoundEnd?.Invoke(null);
            }
        }
    }

    #endregion

    public static void Restart() {
        Parent.DestroyAllChildren();
        roundTimer = GameSettings.RoundDuration;

        OnRestart?.Invoke();
    }
}
