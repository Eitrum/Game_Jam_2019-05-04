using Rewired;
using UnityEngine;

public sealed class Player : MonoBehaviour
{
    private readonly int playerId = 0;

    private Rewired.Player player;
    private Vector3 moveVector;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;

    [Header("Settings")]
    [SerializeField] private PlayerMovementSettings movementSettings;

    void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
    }

    void Update()
    {
        moveVector.x = player.GetAxis("Move Horizontal");
        moveVector.z = player.GetAxis("Move Vertical");

        rb.AddForce(moveVector.normalized * movementSettings.moveSpeed * Time.deltaTime, ForceMode.Force);
    }
}
