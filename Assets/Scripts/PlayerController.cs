using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject playerGround;
    [SerializeField] private TextMeshPro myLevel;
    private Vector3 originalPosition;
    private int level = 1;
    private void Awake()
    {
        playerGround = GameObject.Find("PlayerGround");
    }

    private IEnumerator Start() {
        //CheckPos();
        yield return new WaitForSeconds(0.1f);
        GroundTile nearestTile = GroundManager.Instance.GetTileAtPosition(transform.position);
        SnapToTile(nearestTile);
    }
    void OnMouseDown()
    {
        originalPosition = transform.position;
    }

    void OnMouseDrag()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0f;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
    }

    void OnMouseUp()
    {
        CheckPos();
    }
    private void CheckPos()
    {
        GroundTile nearestTile = GroundManager.Instance.GetTileAtPosition(transform.position);
        GroundTile oldTile = GroundManager.Instance.GetTileAtPosition(originalPosition);
        if (nearestTile != null && !nearestTile.IsOccupied())
        {
            oldTile.RemovePlayer();
            SnapToTile(nearestTile);
        }
        else if (nearestTile != null && nearestTile.IsOccupied())
        {
            PlayerController occupyingPlayer = nearestTile.CurrentPlayer;

            if (occupyingPlayer.level == level)
            {
                if (occupyingPlayer != this)
                {
                    Debug.Log("Wtf");
                    occupyingPlayer.level++;
                    occupyingPlayer.UpdateMyLevel();
                    oldTile.RemovePlayer();
                    Destroy(gameObject);
                }
                else
                    transform.position = originalPosition;
            }
            else
            {
                nearestTile.RemovePlayer();
                oldTile.RemovePlayer();
                nearestTile.SetPlayer(this);
                oldTile.SetPlayer(occupyingPlayer);
                Vector3 tempPosition = occupyingPlayer.transform.position;
                occupyingPlayer.transform.position = originalPosition;
                transform.position = tempPosition;
            }
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    private void SnapToTile(GroundTile tile)
    {
        tile.SetPlayer(this);
        transform.position = tile.transform.position;
    }
    public void UpdateMyLevel()
    {
        myLevel.text = level.ToString();
    }
}
