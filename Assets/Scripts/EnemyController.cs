using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private TextMeshPro healthTxt;
    public bool isBoss = false;
    private BoxCollider2D col;
    private bool isIdleAnimating = true;
    private Coroutine idleCoroutine; 

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        UpdateHealthTxt();
        idleCoroutine = StartCoroutine(IdleAnimation()); 
    }

    public void SetHealth(int health)
    {
        this.health = health;
        UpdateHealthTxt();
    }

    public int GetHealth()
    {
        return health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            EnemyManager.Instance.IncreaseScore();
            col.enabled = false;
            if (isBoss)
            {
                EnemyManager.Instance.isBossComing = false;
                EnemyManager.Instance.isBossAlive = false;
                PlayerManager.Instance.UpdateMainLevelTxt();
                EnemyManager.Instance.luckytime = true;
                EnemyManager.Instance.posStartLucky = EnemyManager.Instance.clickAmount;
                EnemyManager.Instance.StartLuckyTime();
            }
            else
                EnemyManager.Instance.RemoveEnemy(this);
            Destroy(gameObject);
        }
        UpdateHealthTxt();
    }

    private void UpdateHealthTxt()
    {
        healthTxt.text = health.ToString();
    }

    public IEnumerator MoveMent()
    {
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
        }

        Vector3 startPosition = this.transform.position;
        Vector3 targetPosition = startPosition - new Vector3(0, 0.5f, 0);
        float elapsedTime = 0f;
        float moveDuration = 0.5f;

        while (elapsedTime < moveDuration)
        {
            this.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.transform.position = targetPosition;
        idleCoroutine = StartCoroutine(IdleAnimation());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("end game");
            GameManager.Instance.isLose = true;
            GameManager.Instance.ActiveLosePanel();
        }
    }

    private IEnumerator IdleAnimation()
    {
        Vector3 startPosition = transform.position;
        float amplitude = 0.05f;
        float frequency = 3f;

        while (isIdleAnimating)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
            yield return null;
        }
    }
}
