using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class EnemyController : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private TextMeshPro healthTxt;
    public bool isBoss = false;
    void Start()
    {
        UpdateHealthTxt();
    }
    public void SetHealth(int health){
        this.health = health;
        UpdateHealthTxt();
    }
    public int GetHealth(){
        return health;
    }
    public void TakeDamage(int damage){
        health -= damage;
        if(health <=0 ){
            EnemyManager.Instance.IncreaseScore();
            if(isBoss){
                EnemyManager.Instance._isBossComing = false;
                EnemyManager.Instance._isBossAlive = false;
                PlayerManager.Instance.UpdateMainLevelTxt();
            }else
                EnemyManager.Instance.RemoveEnemy(this);
            Destroy(gameObject);
        }
        UpdateHealthTxt();
    }
    private void UpdateHealthTxt(){
        healthTxt.text = health.ToString();
    }
    public IEnumerator MoveMent()
    {
        Vector3 startPosition = this.transform.position;
        Vector3 targetPosition = startPosition - new Vector3(0,0.5f,0);
        float elapsedTime = 0f;
        float moveDuration = 0.5f; 

        while (elapsedTime < moveDuration)
        {
            this.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        this.transform.position = targetPosition;
    }



}
