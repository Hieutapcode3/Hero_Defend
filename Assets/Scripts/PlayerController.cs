using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject playerGround;
    [SerializeField] private TextMeshPro myLevel;
    [SerializeField] private SpriteRenderer myWeapon;
    [SerializeField] private List<Sprite> weaponsSprite;
    private Vector3 originalPosition;
    private int level = 1;
    private int bulletCount;
    private Animator anim;
    private bool isMouseDown;
    private int damage;
    private void Awake()
    {
        playerGround = GameObject.Find("PlayerGround");
        anim = GetComponent<Animator>();
    }

    private void Start() {
        SetDamage();
    }
    void OnMouseDown()
    {
        if (!GameManager.Instance.canDrag) return; 
        originalPosition = transform.position;
        isMouseDown = true;
        transform.localScale = Vector3.one * 0.9f;
    }

    void OnMouseDrag()
    {
        if (!GameManager.Instance.canDrag ) return; 
        if(isMouseDown )
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0f;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
        }
        
    }

    void OnMouseUp()
    {
        if (!GameManager.Instance.canDrag) return;
        CheckPos();
    }
    private void CheckPos()
    {
        GroundTile nearestTile = GroundManager.Instance.GetTileAtPosition(transform.position);
        GroundTile oldTile = GroundManager.Instance.GetTileAtPosition(originalPosition);

        if (nearestTile == null || nearestTile == oldTile)
        {
            transform.position = originalPosition;
            transform.localScale = Vector3.one * 0.75f;
            return;
        }

        if (!nearestTile.IsOccupied())
        {
            oldTile.RemovePlayer();
            SnapToTile(nearestTile);
        }
        else if (nearestTile.IsOccupied())
        {
            PlayerController occupyingPlayer = nearestTile.CurrentPlayer;
            if (occupyingPlayer.level == level && occupyingPlayer != this)
            {
                occupyingPlayer.level++;
                occupyingPlayer.SetDamage();
                occupyingPlayer.UpdateMyLevel();
                oldTile.RemovePlayer();
                PlayerManager.Instance.RemovePlayer(this);
                GameManager.Instance.canDrag = false;
                Destroy(gameObject);
            }
            else if (occupyingPlayer.level != level)
            {
                Debug.Log("!= level");
                nearestTile.RemovePlayer();
                oldTile.RemovePlayer();
                nearestTile.SetPlayer(this);
                oldTile.SetPlayer(occupyingPlayer);
                Vector3 tempPosition = occupyingPlayer.transform.position;
                occupyingPlayer.transform.position = originalPosition;
                transform.position = tempPosition;

            }
        }
        transform.localScale = Vector3.one * 0.75f;
        GameManager.Instance.canDrag = false;
        if (isMouseDown)
        {
            PlayerManager.Instance.PLayersAttack(); 
            if(EnemyManager.Instance._isBossAlive){
                if(EnemyManager.Instance.CheckNullBoss()){
                    EnemyManager.Instance.SpawnBossEnemy();
                }else
                    EnemyManager.Instance.MoveBoss();
            }
            EnemyManager.Instance.MoveEnemies();
            EnemyManager.Instance.MoveChest();
            EnemyManager.Instance.IncreaseClick();

            
        }
        isMouseDown = false;
    }

    private void SnapToTile(GroundTile tile)
    {
        tile.SetPlayer(this);
        transform.position = tile.transform.position;
    }
    public void SetMyLevel(int mylevel){
        this.level = mylevel;
    }
    public void UpdateMyLevel()
    {
        myLevel.text = level.ToString();
        if(level < weaponsSprite.Count){
            myWeapon.sprite = weaponsSprite[level - 1];
        }else
            myWeapon.sprite = weaponsSprite[weaponsSprite.Count - 1];

    }
    public IEnumerator Attack(){
        bulletCount = level % 5 ;
        if(bulletCount == 0)
            bulletCount = 1;
        anim.SetTrigger("Attack");
        for(int i = 0;i < bulletCount;i++){
            GameObject bullet =  ObjectPool.instance.SpawnFromPool("Bullet",this.transform.position,Quaternion.identity);
            bullet.GetComponent<Bullet>().SetDamage(damage);
            yield return new WaitForSeconds(0.05f);
        }
    }
    public void SetDamage(){
        if (level < 5)
            damage = level;
        else if (level == 5)
            damage = 30;
        else
            damage  = Mathf.RoundToInt(damage * 1.3f / bulletCount);
    }
    
}
