using UnityEngine;

public class GroundTile : MonoBehaviour
{
    public Vector2Int Position { get; private set; }
    public PlayerController CurrentPlayer { get; private set; }

    public void SetPosition(Vector2Int position)
    {
        Position = position;
    }

    public void SetPlayer(PlayerController player)
    {
        CurrentPlayer = player;
    }

    public void RemovePlayer()
    {
        CurrentPlayer = null;
    }

    public bool IsOccupied()
    {
        return CurrentPlayer != null;
    }
}
