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
    private Vector3 previousPos;
    private int level = 1;
    private int bulletCount;
    private Animator anim;
    private bool canDrag;
    private void Awake()
    {
        playerGround = GameObject.Find("PlayerGround");
        anim = GetComponent<Animator>();
    }

    private void Start() {
        canDrag = true;
    }
    void OnMouseDown()
    {
        if (!canDrag) return; 
        originalPosition = transform.position;
    }

    void OnMouseDrag()
    {
        if (!canDrag && previousPos != originalPosition) return; 
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0f;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
        
    }

    void OnMouseUp()
    {
        if (!canDrag) return;
        CheckPos();
    }
    private void CheckPos()
    {
        GroundTile nearestTile = GroundManager.Instance.GetTileAtPosition(transform.position);
        GroundTile oldTile = GroundManager.Instance.GetTileAtPosition(originalPosition);

        if (nearestTile == null || nearestTile == oldTile)
        {
            transform.position = originalPosition;
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
                occupyingPlayer.UpdateMyLevel();
                oldTile.RemovePlayer();
                PlayerManager.Instance.RemovePlayer(this);
                Destroy(gameObject);
            }
            else if (occupyingPlayer.level != level)
            {
                nearestTile.RemovePlayer();
                oldTile.RemovePlayer();
                nearestTile.SetPlayer(this);
                oldTile.SetPlayer(occupyingPlayer);

                Vector3 tempPosition = occupyingPlayer.transform.position;
                occupyingPlayer.transform.position = originalPosition;
                transform.position = tempPosition;

            }
            else
            {
                transform.position = originalPosition;
            }
        }
        else
        {
            transform.position = originalPosition;
        }
        canDrag = false;
        previousPos = originalPosition;
        PlayerManager.Instance.PLayersAttack(); 
        EnemyManager.Instance.MoveEnemies();
        EnemyManager.Instance.MoveChest();
        EnemyManager.Instance.IncreaseClick();
        StartCoroutine(ChangeStateDrag());
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
            bullet.GetComponent<Bullet>().SetDamage(level);
            yield return new WaitForSeconds(0.05f);
        }
    }
    private IEnumerator ChangeStateDrag(){
        yield return new WaitForSeconds(1f);
        canDrag = true;
    }
}
