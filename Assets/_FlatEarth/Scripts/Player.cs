using Rewired;
using UnityEngine;

public sealed class Player : MonoBehaviour
{
    public int playerId = 0;

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
    }

    void FixedUpdate()
    {
        moveVector.x = player.GetAxis("Move Horizontal");
        moveVector.z = player.GetAxis("Move Vertical");

        rb.AddForce(moveVector.normalized * PlayerMovementSettings.MoveSpeed * Time.fixedDeltaTime, ForceMode.Force);

        if (transform.localPosition.y < -5f)
            GameManager.KillPlayer(this);
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
