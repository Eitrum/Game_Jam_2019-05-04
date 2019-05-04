using Rewired;
using UnityEngine;

public sealed class Player : MonoBehaviour
{
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

#pragma warning disable
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
#pragma warning enable

    void Start()
    {
        player = ReInput.players.GetPlayer(playerId);
        namePlateTransform = Instantiate(namePlatePrefab).transform;
        namePlateTransform.GetComponent<NamePlate>().SetName(playerName);
        cameraTransform = Camera.main.transform;
    }

    private void OnDestroy()
    {
        Destroy(namePlateTransform.gameObject);
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            moveVector.x = player.GetAxis("Move Horizontal");
            moveVector.z = player.GetAxis("Move Vertical");

            rb.AddForce(moveVector.normalized * PlayerMovementSettings.MoveSpeed * Time.fixedDeltaTime, ForceMode.Force);

            if (transform.localPosition.y < -5f)
                GameManager.KillPlayer(this);
        }
    }

    private void Update()
    {
        namePlateTransform.position = transform.position + namePlateOffset;
        namePlateTransform.LookAt(namePlateTransform.position - cameraTransform.position, cameraTransform.up);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody)
        {
            Vector3 velocity = collision.rigidbody.velocity;
            Vector3 direction = (rb.position 
                - collision.rigidbody.position).normalized;
            Vector3 force = direction 
                * Mathf.Clamp(velocity.magnitude, 
                    PlayerMovementSettings.MinBounceForce, 
                    PlayerMovementSettings.MaxBounceForce);
            rb.AddForce(force * Mathf.Pow(PlayerMovementSettings.BounceMultiplier, bounceCount),
                ForceMode.Impulse);
            bounceCount++;
        }
    }
}
