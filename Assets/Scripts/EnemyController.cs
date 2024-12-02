using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private TextMeshPro healthTxt;
    public bool isBoss = false;
    private BoxCollider2D col;
    private bool isIdleAnimating = true;
    private Coroutine idleCoroutine;
    private Vector3 initialPosition;
    IEnumerator Start()
    {
        transform.localScale = Vector3.one * 2;
        StartAnim();
        col = GetComponent<BoxCollider2D>();
        yield return new WaitForSeconds(0.5f);
        initialPosition = transform.position; 
        UpdateHealthTxt();
        idleCoroutine = StartCoroutine(IdleAnimation());
    }

    public void SetHealth(int health)
    {
        this.health = health;
        UpdateHealthTxt();
    }

    private void StartAnim(){
        transform.DOMoveY(transform.position.y - 0.5f,0.5f).SetEase(Ease.Linear);
        transform.DOScale(1,0.5f).SetEase(Ease.Linear);
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
            }
            else
            {
                EnemyManager.Instance.RemoveEnemy(this);
            }
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
        transform.position = initialPosition;

        Vector3 targetPosition = initialPosition - new Vector3(0, 0.5f, 0);
        float elapsedTime = 0f;
        float moveDuration = 0.5f;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        initialPosition = targetPosition; 

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
        Vector3 startPosition = initialPosition; 
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
