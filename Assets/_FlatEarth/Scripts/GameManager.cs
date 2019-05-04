using UnityEngine;
using Eitrum.Engine.Extensions;

public class GameManager : MonoBehaviour {

    #region Variables

    private static Transform parent;
    private static Transform platform;
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
        platform = transform.Find("Platform");
        Restart();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            Restart();
        if (roundTimer > 0f) {
            roundTimer -= Time.deltaTime;
            var scale = GameSettings.Scale(roundTimer);
            platform.localScale = new Vector3(scale, 1f, scale) ;
            if (roundTimer <= 0f) {
                OnRoundEnd?.Invoke(null);
            }
        }
    }

    #endregion

    #region Restart

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Flat Earth/Restart", validate = true)]
    public static bool CanRestart() {
        return Application.isPlaying;
    }

    [UnityEditor.MenuItem("Flat Earth/Restart")]
#endif
    public static void Restart() {
        Parent.DestroyAllChildren();
        roundTimer = GameSettings.RoundDuration;
        SpawnPlayers();
        OnRestart?.Invoke();
    }

    #endregion

    #region respawn players

    public static void SpawnPlayers() {
        var oldPlayers = Parent.GetComponentsInChildren<Player>();
        for (int i = 0; i < oldPlayers.Length; i++) {
            oldPlayers[i].transform.Destroy();
        }

        int count = Rewired.ReInput.players.playerCount;
        Vector3 spawn = Vector3.up;
        var forward = Parent.forward * GameSettings.SpawnDistance;
        float steps = 360f / count;
        
        for (int i = 0; i < count; i++) {
            var position = spawn + Quaternion.Euler(0, steps * i, 0) * forward;
            var go = Instantiate(GameSettings.PlayerPrefab(i), position, Quaternion.identity, Parent);
            go.GetComponent<Player>().playerId = Rewired.ReInput.players.Players[i].id;
        }

    }

    #endregion
}
