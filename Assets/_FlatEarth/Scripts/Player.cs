using Rewired;
using UnityEngine;

public sealed class Player : MonoBehaviour
{
    private readonly int playerId = 0;

    private Rewired.Player player;
    private Vector3 moveVector;

    public bool canMove;

#pragma warning disable
    [Header("Components")]
    [SerializeField] private Rigidbody rb;

    [Header("Settings")]
    [SerializeField] private PlayerMovementSettings movementSettings;
#pragma warning enable

    void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
    }

    void Update()
    {
        moveVector.x = player.GetAxis("Move Horizontal");
        moveVector.z = player.GetAxis("Move Vertical");

        if (canMove)
        {
            rb.AddForce(moveVector.normalized * movementSettings.moveSpeed * Time.deltaTime, ForceMode.Force);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody)
        {

            Vector3 velocity = collision.rigidbody.velocity;
            Vector3 direction = (rb.position - collision.rigidbody.position).normalized;
            Vector3 force = direction * Mathf.Clamp(velocity.magnitude, 2.5f, 5f);
            rb.AddForce(force, 
                ForceMode.Impulse);
        }
    }
}
