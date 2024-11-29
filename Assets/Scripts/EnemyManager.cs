using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hyb.Utils;

public class EnemyManager : ManualSingletonMono<EnemyManager>
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private int maxEnemiesToSpawn = 3;
    [SerializeField] private GameObject silver_chest;
    [SerializeField] private GameObject gold_chest;
    private List<GameObject> chests = new List<GameObject>();
    private List<EnemyController> enemies = new List<EnemyController>();
    public int _score = 0;
    private int _clickAmount = -1;

    void Start()
    {
        SpawnEnemiesOnFirstRow();
    }

    public void SpawnEnemiesOnFirstRow()
    {
        List<GroundTile> firstRowTiles = GroundManager.Instance.GetFirstRowTilesOfEnemyGround();
        List<GroundTile> availableTiles = new List<GroundTile>(firstRowTiles.FindAll(tile => !tile.IsOccupied()));

        int enemiesToSpawn = Random.Range(1, maxEnemiesToSpawn + 1);
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (availableTiles.Count == 0) break;
            int randomIndex = Random.Range(0, availableTiles.Count);
            GroundTile tile = availableTiles[randomIndex];
            availableTiles.RemoveAt(randomIndex);
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, 3)];
            GameObject newEnemy = Instantiate(enemyPrefab, tile.transform.position, Quaternion.identity);
            newEnemy.transform.SetParent(this.transform);
            EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                int health = CalculateHealth();
                enemyController.SetHealth(health);
            }
            tile.SetEnemy();  
            enemies.Add(newEnemy.GetComponent<EnemyController>());
        }
    }

    private int CalculateHealth()
    {
        if (_score <= 1)
        {
            return 1;
        }
        else if (_score <= 2)
        {
            return Random.Range(1, 3);
        }
        else
        {
            int minHealth = _score / 3;
            int maxHealth = Mathf.Max(minHealth + 1, _score / 2);
            return Random.Range(minHealth, maxHealth + 1);
        }
    }

    public void IncreaseClick()
    {
        _clickAmount += 1;
        if (_clickAmount % 2 == 0)
        {
            SpawnChest();
        }
    }

    private void SpawnChest()
    {
        List<GroundTile> firstRowTiles = GroundManager.Instance.GetFirstRowTilesOfEnemyGround();
        List<GroundTile> availableTiles = new List<GroundTile>(firstRowTiles.FindAll(tile => !tile.IsOccupied()));

        if (availableTiles.Count > 0)
        {
            GroundTile randomTile = availableTiles[Random.Range(0, availableTiles.Count)];
            GameObject chestPrefab = Random.value < 0.6f ? silver_chest : gold_chest;
            GameObject newChest = Instantiate(chestPrefab, randomTile.transform.position, Quaternion.identity);
            randomTile.SetChest();  
            chests.Add(newChest);
        }
    }

    public void MoveEnemies()
    {
        List<GroundTile> firstRowTiles = GroundManager.Instance.GetFirstRowTilesOfEnemyGround();
        foreach(GroundTile groundTile in firstRowTiles){
            if(groundTile.IsOccupied()){
                groundTile.RemoveChest();
                groundTile.RemoveEnemy();
            }
        }
        if (enemies.Count != 0)
        {
            foreach (EnemyController enemy in enemies)
            {
                enemy.StartCoroutine(enemy.MoveMent());
            }
        }
        SpawnEnemiesOnFirstRow();
    }

    public void MoveChest()
    {
        if (chests.Count != 0)
        {
            foreach (GameObject chest in chests)
            {
                StartCoroutine(MoveMent(chest.transform));
            }
        }
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        GroundTile tile = GroundManager.Instance.GetTileAtPosition(enemy.transform.position);
        if (tile != null) tile.RemoveEnemy();
        enemies.Remove(enemy);
    }
    public void RemoveChest(GameObject chest)
    {
        GroundTile tile = GroundManager.Instance.GetTileAtPosition(chest.transform.position);
        if (tile != null) tile.RemoveChest();
        chests.Remove(chest);
        Destroy(chest);
    }


    public IEnumerator MoveMent(Transform obj)
    {
        if (obj == null) yield break;  
        
        Vector3 startPosition = obj.position;
        Vector3 targetPosition = startPosition - new Vector3(0, 0.5f, 0);
        float elapsedTime = 0f;
        float moveDuration = 0.5f;

        while (elapsedTime < moveDuration)
        {
            if (obj == null) yield break;  

            obj.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (obj != null)  
        {
            obj.position = targetPosition;
        }
    }

}
