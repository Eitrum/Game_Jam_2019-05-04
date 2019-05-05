using UnityEngine;
using Eitrum.Engine.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Eitrum;

public class GameManager : MonoBehaviour {

    #region Variables

    private static Transform parent;
    private static Transform platform;
    private static float roundTimer = 0f;
    private static List<Player> players = new List<Player>();

    public static event System.Action OnRoundStart;
    public static event System.Action<Player> OnRoundEnd;
    public static event System.Action OnRestart;
    public static event System.Action<int> OnCountDown;

    private static Coroutine endGameRestartRoutine;

    public Transform cameraContainer;
    public GameObject logo;

    #endregion

    #region Properties

    public static Transform Parent => parent;

    #endregion

    #region Unity Methods

    void Awake() {
        parent = transform.Find("Parent");
        platform = transform.Find("Platform");
    }

    IEnumerator Start(){
        logo.SetActive(true);
        yield return new WaitForSeconds(3f);
        logo.SetActive(false);
        Restart();
    }

    void Update() {
        for (int i = 0; i < 10; i++) {
            if (Input.GetKeyDown((KeyCode.Alpha0+i))) {
                GameSettings.SetMinimumPlayers(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Restart();
        if (roundTimer > 0f) {
            roundTimer -= Time.deltaTime;
            var scale = GameSettings.Scale(roundTimer);
            platform.localScale = new Vector3(scale, 1f, scale);
            if (roundTimer <= 0f) {
                EndRound(null);
            }
        }

        Vector3 center = Vector3.zero;
        var tempPlayers = players.Where(x => x.playerId >= 0).ToArray();
        for (int i = 0; i < tempPlayers.Length; i++) {
            center += tempPlayers[i].transform.localPosition;
        }
        if (tempPlayers.Length > 0)
            center /= tempPlayers.Length;
        center.y = 0f;

        cameraContainer.localPosition =Vector3.Lerp(cameraContainer.localPosition, center, Time.deltaTime *3f);
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
        Eitrum.Engine.Core.Timer.Stop(endGameRestartRoutine);
        Parent.DestroyAllChildren();
        roundTimer = GameSettings.RoundDuration;
        SpawnPlayers();
        OnRestart?.Invoke();
        int counter = 3;
        Eitrum.Engine.Core.Timer.Repeat(1f, counter, () => {
            counter -= 1;
            OnCountDown.Invoke(counter);
            if (counter == 0) {
                OnRoundStart?.Invoke();
                for (int iPlayer = 0, nPlayer = players.Count; iPlayer < nPlayer; ++iPlayer) {
                    players[iPlayer].canMove = true;
                    players[iPlayer].rb.isKinematic = false;
                }
            }
        });
    }

    #endregion

    #region End Round

    private static void EndRound(Player winner) {
        OnRoundEnd?.Invoke(winner);
        Eitrum.Engine.Core.Timer.Once(3f, Restart, ref endGameRestartRoutine);
    }

    #endregion

    #region players

    public static Player GetFocusToOtherPlayer(Player myPlayer) {
        var otherPlayers = players.Where(x => x != myPlayer && x.transform.localPosition.ToVector3_XZ().magnitude < 10f).ToArray();
        if (otherPlayers.Length == 0)
            return null;
        return otherPlayers[Random.Range(0, otherPlayers.Length)];
    }

    public static void SpawnPlayers() {
        var oldPlayers = Parent.GetComponentsInChildren<Player>();
        for (int i = 0; i < oldPlayers.Length; i++) {
            oldPlayers[i].transform.Destroy();
        }

        players.Clear();
        var controllers = Rewired.ReInput.players.Players.Where(x => x.controllers.Controllers.Any(y=>y.isConnected)).ToArray();
        int playerCount = Mathf.Max(controllers.Length, GameSettings.MinimumPlayers);
        Vector3 spawn = Vector3.up;
        var forward = Parent.forward * GameSettings.SpawnDistance;
        float steps = 360f / playerCount;

        for (int i = 0; i < playerCount; i++) {
            var position = spawn + Quaternion.Euler(0, steps * i, 0) * forward;
            var go = Instantiate(GameSettings.PlayerPrefab(i), position, Quaternion.identity, Parent);
            var player = go.GetComponent<Player>();
            player.playerId = (i <controllers.Length ?controllers[i].id:-1);
            player.rb.isKinematic = true;
            players.Add(player);
        }
    }

    public static void KillPlayer(Player player) {
        if (players.Remove(player)) {
            Destroy(player.gameObject);
            if (players.Count == 0) {
                EndRound(null);
            }
            if (players.Count == 1) {
                EndRound(players[0]);
            }
        }
    }

    #endregion
}
