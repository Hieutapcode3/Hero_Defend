using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int damage;
    private Coroutine disableCoroutine;

    void OnEnable()
    {
        disableCoroutine = StartCoroutine(DisableAfterTime(3f));
    }

    void OnDisable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private void  OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
        }
        else if (col.gameObject.CompareTag("Chest_1"))
        {
            PlayerManager.Instance.SpawnPlayer(col.gameObject.transform,false);
            EnemyManager.Instance.RemoveChest(col.gameObject);
            Destroy(col.gameObject);
        }else if (col.gameObject.CompareTag("Chest_2")){
            PlayerManager.Instance.SpawnPlayer(col.gameObject.transform,true);
            EnemyManager.Instance.RemoveChest(col.gameObject);
            Destroy(col.gameObject);
        }
        this.gameObject.SetActive(false);
    }
    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false); 
    }
}
