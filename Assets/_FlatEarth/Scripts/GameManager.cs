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
    
    #endregion

    #region Unity Methods

    void Awake() {
        parent = transform.Find("Parent");
        Restart();
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
        SpawnPlayers();
        OnRestart?.Invoke();
    }

    public static void SpawnPlayers() {
        int count = Rewired.ReInput.players.allPlayerCount;
        Vector3 spawn = Vector3.up;
        var forward = Parent.forward * GameSettings.SpawnDistance;
        float steps = 360f / count;
        
        for (int i = 0; i < count; i++) {
            var position = spawn + Quaternion.Euler(0, steps * i, 0) * forward;
            var go = Instantiate(GameSettings.PlayerPrefab(i), position, Quaternion.identity, Parent);
            go.GetComponent<Player>().playerId = Rewired.ReInput.players.AllPlayers[i].id;
        }

    }
}
