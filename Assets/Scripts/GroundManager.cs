using System.Collections.Generic;
using UnityEngine;
using Hyb.Utils;

public class GroundManager : ManualSingletonMono<GroundManager>
{
    [SerializeField] private GameObject playerGround; 
    [SerializeField] private int columns = 6; 
    private List<GroundTile> groundTiles = new List<GroundTile>();
    private Dictionary<Vector2Int, GroundTile> gridMap = new Dictionary<Vector2Int, GroundTile>();

    void Start()
    {
        int currentRow = 0, currentColumn = 0;
        
        foreach (Transform ground in playerGround.transform)
        {
            GroundTile tile = ground.gameObject.AddComponent<GroundTile>();
            tile.SetPosition(new Vector2Int(currentColumn, currentRow));
            gridMap[tile.Position] = tile;
            groundTiles.Add(tile);
            currentColumn++;
            if (currentColumn >= columns)
            {
                currentColumn = 0;
                currentRow++;
            }
        }
    }
    public GroundTile GetTileAtPosition(Vector2 position)
    {
        float closestDistance = float.MaxValue;
        GroundTile closestTile = null;

        foreach (var tile in groundTiles)
        {
            float distance = Vector2.Distance(tile.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tile;
            }
        }

        return closestDistance < 1f ? closestTile : null;
    }
}
