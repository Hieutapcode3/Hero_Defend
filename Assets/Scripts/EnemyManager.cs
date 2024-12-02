using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hyb.Utils;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class EnemyManager : ManualSingletonMono<EnemyManager>
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private int maxEnemiesToSpawn = 3;
    [SerializeField] private GameObject silver_chest;
    [SerializeField] private GameObject gold_chest;
    [SerializeField] private GameObject coinPref;
    [SerializeField] private Text scoreTxt;
    [SerializeField] private GameObject damageTxt;

    private List<GameObject> coins = new List<GameObject>();
    private List<GameObject> chests = new List<GameObject>();
    private List<EnemyController> enemies = new List<EnemyController>();
    private int _nextBigEnemyScoreThreshold = 20;
    private int _nextBossEnemyScoreThreshold = 50;
    private EnemyController boss;
    public int clickAmount = -1;
    public int score = 0;
    public bool isBossAlive = false; 
    public bool isBossComing = false;
    public bool luckytime;
    public int posStartLucky = 0;

    public override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        SpawnEnemiesOnFirstRow();
        UpdateScoreTxt();
    }

    public void SpawnEnemiesOnFirstRow()
    {
        if (isBossAlive) return;

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
            GameObject newEnemy = Instantiate(enemyPrefab, tile.transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
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
        if (score <= 1)
        {
            return 1;
        }
        else if (score <= 2)
        {
            return Random.Range(1, 3);
        }
        else
        {
            int minHealth = score / 3;
            int maxHealth = Mathf.Max(minHealth + 1, score / 2);
            return Random.Range(minHealth, maxHealth + 1);
        }
    }

    private void SpawnBigEnemy()
    {
        List<GroundTile> availableTiles = GroundManager.Instance.GetFirstRowTilesOfEnemyGround().FindAll(tile => !tile.IsOccupied());
        
        if (availableTiles.Count > 0)
        {
            GroundTile tile = availableTiles[Random.Range(0, availableTiles.Count)];
            GameObject bigEnemyPrefab = enemyPrefabs[3]; 
            GameObject bigEnemy = Instantiate(bigEnemyPrefab, tile.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            bigEnemy.transform.SetParent(this.transform);
            EnemyController bigEnemyController = bigEnemy.GetComponent<EnemyController>();
            
            if (bigEnemyController != null)
            {
                int bigEnemyHealth = Mathf.RoundToInt(score * 1.2f);
                bigEnemyController.SetHealth(bigEnemyHealth);
            }
            
            tile.SetEnemy();
            enemies.Add(bigEnemy.GetComponent<EnemyController>());
        }
    }
    public void EnemyTakeDamage()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            TextMeshPro txt = Instantiate(damageTxt, enemies[i].transform.position,Quaternion.identity).GetComponent<TextMeshPro>();
            txt.text = "-" +  score.ToString();
            enemies[i].TakeDamage(score);
        }

        if (boss != null)
        {
            boss.TakeDamage(score);
        }
    }

    public bool CheckNullBoss(){
        return boss == null;
    }
    public void SpawnBossEnemy()
    {
        List<GroundTile> availableTiles = GroundManager.Instance.GetFirstRowTilesOfEnemyGround().FindAll(tile => !tile.IsOccupied());
        if (availableTiles.Count == 0) return;

        GroundTile tile = availableTiles[Random.Range(0, availableTiles.Count)];
        GameObject bossPrefab;
        if (score < 200)
        {
            bossPrefab = enemyPrefabs[Random.Range(4, 6)];
        }
        else
        {
            bossPrefab = enemyPrefabs[Random.Range(6, enemyPrefabs.Count)];
        }

        GameObject bossEnemy = Instantiate(bossPrefab, tile.transform.position + Vector3.up * .5f, Quaternion.identity);
        bossEnemy.transform.SetParent(this.transform);
        EnemyController bossController = bossEnemy.GetComponent<EnemyController>();
        bossController.isBoss = true;
        if (bossController != null)
        {
            int bossHealth = score * (score / 50) * 2;
            bossController.SetHealth(bossHealth);
            boss = bossController;
        }
        isBossAlive = true;
        _nextBossEnemyScoreThreshold += 50;
    }


    public void IncreaseScore()
    {
        score++;
        UpdateScoreTxt();
        if (score >= _nextBossEnemyScoreThreshold)
        {
            isBossComing = true;
            if(isBossAlive && isBossComing){
                Debug.Log("spawn Boss");
                SpawnBossEnemy();
                _nextBossEnemyScoreThreshold += 50;
            }
        }
        else if (score >= _nextBigEnemyScoreThreshold && !isBossComing)
        {
            SpawnBigEnemy();
            _nextBigEnemyScoreThreshold += 20;
        }
    }
    private void CheckEmptyEnemyAndChest(){
        if(chests.Count==0 && enemies.Count ==0 && isBossComing){
            isBossAlive = true;
        }
    }
    public void MoveBoss(){
        boss.StartCoroutine(boss.MoveMent());
    }
    public void IncreaseClick()
    {
        if(!isBossComing){
            clickAmount += 1;
            if (clickAmount % 2 == 0 && !luckytime)
            {
                SpawnChest();
            }
        }
    }
    private void SpawnChest()
    {
        List<GroundTile> firstRowTiles = GroundManager.Instance.GetFirstRowTilesOfEnemyGround();
        List<GroundTile> availableTiles = new List<GroundTile>(firstRowTiles.FindAll(tile => !tile.IsOccupied()));

        if (availableTiles.Count > 0)
        {
            GroundTile randomTile = availableTiles[Random.Range(0, availableTiles.Count)];
            GameObject chestPrefab = Random.value < 0.7f ? silver_chest : gold_chest;
            GameObject newChest = Instantiate(chestPrefab, randomTile.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            newChest.transform.localScale = Vector3.one * 2;
            StartAnimChest(newChest);
            randomTile.SetChest();
            chests.Add(newChest);
        }
    }
    private void StartAnimChest(GameObject obj){
        obj.transform.DOMoveY(obj.transform.position.y - 0.5f,0.5f).SetEase(Ease.Linear);
        obj.transform.DOScale(.9f,0.5f).SetEase(Ease.Linear);
    }
    private void StartAnimCoin(GameObject obj){
        obj.transform.DOMoveY(obj.transform.position.y - 0.5f,0.5f).SetEase(Ease.Linear);
        obj.transform.DOScale(.15f,0.5f).SetEase(Ease.Linear);
    }

    public void MoveEnemies()
    {
        List<GroundTile> firstRowTiles = GroundManager.Instance.GetFirstRowTilesOfEnemyGround();
        foreach (GroundTile groundTile in firstRowTiles)
        {
            if (groundTile.IsOccupied())
            {
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
        if(!isBossComing && !luckytime)
            SpawnEnemiesOnFirstRow();
    }

    public void MoveChest()
    {
        if (chests.Count != 0)
        {
            foreach (GameObject chest in chests)
            {
                if(chest!=null)
                    StartCoroutine(MoveMent(chest.transform));
            }
        }
    }
    public void StartLuckyTime()
    {
        if (luckytime)
        {
            if (posStartLucky >= clickAmount - 1)
            {
                List<GroundTile> firstRowTiles = GroundManager.Instance.GetFirstRowTilesOfEnemyGround();
                List<GroundTile> shuffledTiles = new List<GroundTile>(firstRowTiles);
                for (int i = shuffledTiles.Count - 1; i > 0; i--)
                {
                    int randomIndex = Random.Range(0, i + 1);
                    GroundTile temp = shuffledTiles[i];
                    shuffledTiles[i] = shuffledTiles[randomIndex];
                    shuffledTiles[randomIndex] = temp;
                }
                List<GroundTile> chestTiles = shuffledTiles.GetRange(0, 2);
                List<GroundTile> coinTiles = shuffledTiles.GetRange(2, shuffledTiles.Count - 2);
                List<GroundTile> availableChestTiles = new List<GroundTile>(chestTiles.FindAll(tile => !tile.IsOccupied()));
                for (int i = 0; i < 2 && availableChestTiles.Count > 0; i++)
                {
                    GroundTile randomTile = availableChestTiles[Random.Range(0, availableChestTiles.Count)];
                    GameObject chestPrefab = Random.value < 0.6f ? silver_chest : gold_chest;
                    GameObject newChest = Instantiate(chestPrefab, randomTile.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                    newChest.transform.localScale = Vector3.one * 2;
                    StartAnimChest(newChest);
                    randomTile.SetChest();
                    chests.Add(newChest);
                    availableChestTiles.Remove(randomTile);
                }
                List<GroundTile> availableCoinTiles = new List<GroundTile>(coinTiles.FindAll(tile => !tile.IsOccupied()));
                for (int i = 0; i < 4 && availableCoinTiles.Count > 0; i++)
                {
                    GroundTile randomTile = availableCoinTiles[Random.Range(0, availableCoinTiles.Count)];
                    GameObject coin = Instantiate(coinPref, randomTile.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                    coin.transform.localScale = Vector3.one * .2f;
                    StartAnimCoin(coin);
                    coins.Add(coin);
                    availableCoinTiles.Remove(randomTile);
                }
            }
            else
            {
                luckytime = false;
            }
        }
    }


    public void MoveCoins()
    {
        if (coins.Count != 0)
        {
            foreach (GameObject coin in coins)
            {
                if(coin != null)
                    StartCoroutine(MoveMent(coin.transform));
            }
        }
    }
    public void RemoveCoin(GameObject coin)
    {
        coins.Remove(coin);
    }
    public void RemoveEnemy(EnemyController enemy)
    {
        GroundTile tile = GroundManager.Instance.GetTileAtPosition(enemy.transform.position);
        if (tile != null) tile.RemoveEnemy();
        enemies.Remove(enemy);
        CheckEmptyEnemyAndChest();
    }

    public void RemoveChest(GameObject chest)
    {
        GroundTile tile = GroundManager.Instance.GetTileAtPosition(chest.transform.position);
        if (tile != null) tile.RemoveChest();
        chests.Remove(chest);
        Destroy(chest);
        CheckEmptyEnemyAndChest();
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
    private void UpdateScoreTxt(){
        scoreTxt.text = "SCORE: " + score.ToString();
    }

}
