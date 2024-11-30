using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField] private GameObject playerGround; 
    [SerializeField] private GameObject enemyGround; 
    [SerializeField] private int columns = 6; 
    private List<GroundTile> groundTilesPlayer = new List<GroundTile>();
    private List<GroundTile> groundTilesEnemy = new List<GroundTile>();
    private Dictionary<Vector2Int, GroundTile> gridMapPLayer = new Dictionary<Vector2Int, GroundTile>();
    private Dictionary<Vector2Int, GroundTile> gridMapEnemy = new Dictionary<Vector2Int, GroundTile>();
    public static GroundManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        int currentRow = 0, currentColumn = 0;
        foreach (Transform ground in playerGround.transform)
        {
            GroundTile tile = ground.gameObject.AddComponent<GroundTile>();
            tile.SetPosition(new Vector2Int(currentColumn, currentRow));
            gridMapPLayer[tile.Position] = tile;
            groundTilesPlayer.Add(tile);
            currentColumn++;
            if (currentColumn >= columns)
            {
                currentColumn = 0;
                currentRow++;
            }
        }
        currentColumn = 0 ;
        currentRow = 0; 
        foreach (Transform ground in enemyGround.transform)
        {
            GroundTile tile = ground.gameObject.AddComponent<GroundTile>();
            tile.SetPosition(new Vector2Int(currentColumn, currentRow));
            gridMapEnemy[tile.Position] = tile;
            groundTilesEnemy.Add(tile);
            currentColumn++;
            if (currentColumn >= columns)
            {
                currentColumn = 0;
                currentRow++;
            }
        }
    }
    public GroundTile GetRandomUnoccupiedTile()
    {
        List<GroundTile> unoccupiedTiles = groundTilesPlayer.FindAll(tile => !tile.IsOccupied());

        if (unoccupiedTiles.Count > 0)
        {
            return unoccupiedTiles[Random.Range(0, unoccupiedTiles.Count)];
        }

        return null;
    }
    public GroundTile GetTileAtPosition(Vector2 position)
    {
        float closestDistance = float.MaxValue;
        GroundTile closestTile = null;

        foreach (var tile in groundTilesPlayer)
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
    public List<GroundTile> GetFirstRowTilesOfEnemyGround()
    {
        List<GroundTile> firstRowTiles = new List<GroundTile>();

        foreach (var tile in groundTilesEnemy)
        {
            if (tile.Position.y == 0) 
            {
                firstRowTiles.Add(tile);
            }
        }

        return firstRowTiles;
    }
    










}
