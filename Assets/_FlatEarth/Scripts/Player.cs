using Rewired;
using UnityEngine;

public sealed class Player : MonoBehaviour {
    public int playerId = 0;
    public string playerName;
    [HideInInspector] public bool canMove;
    [SerializeField] private GameObject namePlatePrefab;
    private Transform namePlateTransform;
    private Transform cameraTransform;

    private Vector3 namePlateOffset = new Vector3(-2.5f, 1, 0);

    private Rewired.Player player;
    private Vector3 moveVector;


    private int bounceCount = 0;

    private Coroutine attack;

#pragma warning disable
    [Header("Components")]
    public Rigidbody rb;
#pragma warning enable

    void Start() {
        player = ReInput.players.GetPlayer(playerId);
        namePlateTransform = Instantiate(namePlatePrefab).transform;
        namePlateTransform.GetComponent<NamePlate>().SetName(playerName);
        cameraTransform = Camera.main.transform;
    }

    private void OnDestroy() {
        if (namePlateTransform)
            Destroy(namePlateTransform.gameObject);
    }

    void FixedUpdate() {
        if (canMove) {
            moveVector.x = player.GetAxis("Move Horizontal");
            moveVector.z = player.GetAxis("Move Vertical");

            rb.AddForce(moveVector.normalized * PlayerMovementSettings.MoveSpeed * Time.fixedDeltaTime, ForceMode.Force);

            if (transform.localPosition.y < -5f)
                GameManager.KillPlayer(this);
        }
    }

    private void Update() {
        namePlateTransform.position = transform.position + namePlateOffset;
        namePlateTransform.LookAt(namePlateTransform.position - cameraTransform.position, cameraTransform.up);


        if(Rewired.ReInput.players.GetPlayer(playerId).GetButtonDown("Identify"))
        Eitrum.Engine.Core.Timer.Animate(0.1f, Animate, ref attack);
    }

    private void Animate(float t) {
        var scale = 2f + Eitrum.Mathematics.EiEase.Cubic.In(1f-t) * 0.4f;
        this.transform.GetChild(0).localScale = new Vector3(scale, scale, scale);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.rigidbody) {
            Vector3 velocity = collision.rigidbody.velocity;
            Vector3 direction = (rb.position
                - collision.rigidbody.position).normalized;
            Vector3 force = direction
                * Mathf.Clamp(velocity.magnitude,
                    PlayerMovementSettings.MinBounceForce,
                    PlayerMovementSettings.MaxBounceForce);

            var toAdd = force * Mathf.Pow(PlayerMovementSettings.BounceMultiplier, bounceCount);
            rb.AddForce(toAdd,
                ForceMode.Impulse);

            var playerController = Rewired.ReInput.players.GetPlayer(playerId);
            playerController.SetVibration(0, Mathf.Max(0.2f, toAdd.magnitude / (PlayerMovementSettings.MaxBounceForce / 2f)), 0.1f);

            bounceCount++;
        }
    }
}
