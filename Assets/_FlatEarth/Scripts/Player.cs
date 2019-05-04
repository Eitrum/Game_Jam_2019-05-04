using Rewired;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerId = 0;

    private Rewired.Player player;
    private Vector3 moveVector;

    void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
    }

    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        moveVector.x = player.GetAxis("Move Horizontal");
        moveVector.y = player.GetAxis("Move Vertical");
        Debug.Log(moveVector);
    }
}
