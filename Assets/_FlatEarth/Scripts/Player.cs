using Rewired;
using UnityEngine;
using FMODUnity;

public sealed class Player : MonoBehaviour {
    public int playerId = 0;
    public string playerName;
    [HideInInspector] public bool canMove;
    [SerializeField] private GameObject namePlatePrefab;
    private Transform namePlateTransform;
    private Transform cameraTransform;
    [SerializeField] private ParticleSystem impactParticle;

    private Vector3 namePlateOffset = new Vector3(-2.5f, 1, 0);

    private Rewired.Player player;
    private Vector3 moveVector;

    private Player aiFocus;

    private int bounceCount = 0;

    public float defaultScale = 2f;

    private Coroutine attack;

    private bool isRolling;
    private bool isRollingPrev;


    //FMOD

    // Planet hit
    public string planetHitEv = "event:/Soundtrack/Planets/planetHit";
    public FMOD.Studio.EventInstance planetHit;

    // Planet roll
    public string planetRollEv = "event:/Soundtrack/Planets/planetRoll";
    public FMOD.Studio.EventInstance planetRoll;

    //FMOD

#pragma warning disable
    [Header("Components")]
    public Rigidbody rb;
#pragma warning enable

    void Start() {
        if(playerId >=0)
            player = ReInput.players.GetPlayer(playerId);
        namePlateTransform = Instantiate(namePlatePrefab).transform;
        namePlateTransform.GetComponent<NamePlate>().SetName(playerName);
        cameraTransform = Camera.main.transform;
        planetHit = FMODUnity.RuntimeManager.CreateInstance(planetHitEv);
        planetRoll = FMODUnity.RuntimeManager.CreateInstance(planetRollEv);
    }

    private void OnDestroy() {
        if (namePlateTransform)
            Destroy(namePlateTransform.gameObject);
        planetRoll.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void FixedUpdate() {
        if (canMove) {
            if (playerId == -1) {
                var direction = CalculateAI();
                moveVector.x = direction.x;
                moveVector.z = direction.z;
            }
            else {
                moveVector.x = player.GetAxis("Move Horizontal");
                moveVector.z = player.GetAxis("Move Vertical");
            }
            rb.AddForce(moveVector.normalized * (PlayerMovementSettings.MoveSpeed * Time.fixedDeltaTime * rb.mass), ForceMode.Force);


            if (transform.localPosition.y < -5f)
                GameManager.KillPlayer(this);
        }
    }

    private Vector3 CalculateAI() {
        if(!aiFocus)
        aiFocus = GameManager.GetFocusToOtherPlayer(this);

        var direction = -this.transform.localPosition;

        if (aiFocus) {
            direction = (aiFocus.transform.localPosition - this.transform.localPosition) / 2f;
        }

        return direction;
    }

    private void Update() {
        namePlateTransform.position = transform.position + namePlateOffset;
        namePlateTransform.LookAt(namePlateTransform.position - cameraTransform.position, cameraTransform.up);


        if (playerId >= 0 && Rewired.ReInput.players.GetPlayer(playerId).GetButtonDown("Identify"))
            Eitrum.Engine.Core.Timer.Animate(0.1f, Animate, ref attack);

        isRolling = rb.velocity.magnitude > 0.1f;

        if (isRolling != isRollingPrev)
        {
            isRollingPrev = isRolling;
            if (isRolling)
            {
                planetRoll.start();
            }
            else
            {
                planetRoll.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
    }

    private void Animate(float t) {
        var scale = defaultScale + Eitrum.Mathematics.EiEase.Cubic.In(1f - t) * defaultScale*0.2f;
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
                    PlayerMovementSettings.MaxBounceForce)
                * rb.mass;

            var toAdd = force * Mathf.Pow(PlayerMovementSettings.BounceMultiplier, bounceCount);
            rb.AddForce(toAdd,
                ForceMode.Impulse);
                
            float normalized = Mathf.Clamp01((force.magnitude - PlayerMovementSettings.MinBounceForce) / (PlayerMovementSettings.MaxBounceForce - PlayerMovementSettings.MinBounceForce));
            planetHit.setParameterByName("impactForce", normalized);
            planetHit.start();

            if (playerId >= 0) {
                var playerController = Rewired.ReInput.players.GetPlayer(playerId);
                playerController.SetVibration(0, Mathf.Max(0.2f, toAdd.magnitude / (PlayerMovementSettings.MaxBounceForce / 4f)), 0.2f);

                bounceCount++;
            }
            else {
                bounceCount += 3;
            }
            impactParticle.Play();
        }
    }
}
