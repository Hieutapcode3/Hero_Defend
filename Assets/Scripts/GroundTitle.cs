using UnityEngine;

public class GroundTile : MonoBehaviour
{
    public Vector2Int Position { get; private set; }
    public PlayerController CurrentPlayer { get; private set; }
    public bool IsOccupiedByEnemy { get; private set; }
    public bool IsOccupiedByChest { get; private set; }

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

    public void SetEnemy()
    {
        IsOccupiedByEnemy = true;
    }

    public void RemoveEnemy()
    {
        IsOccupiedByEnemy = false;
    }
    public void SetChest()
    {
        IsOccupiedByChest = true;
    }

    public void RemoveChest()
    {
        IsOccupiedByChest = false;
    }
    public bool IsOccupied()
    {
        return CurrentPlayer != null || IsOccupiedByEnemy || IsOccupiedByChest;
    }
}
