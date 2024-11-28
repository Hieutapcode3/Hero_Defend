using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject playerGround; 
    private Vector3 originalPosition;
    private int level = 1;
    private void Start() {
        // OnMouseUp();
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
        GroundTile nearestTile = GroundManager.Instance.GetTileAtPosition(transform.position);
        if (nearestTile != null && !nearestTile.IsOccupied())
        {
            SnapToTile(nearestTile);
        }
        else if (nearestTile != null && nearestTile.IsOccupied())
        {
            PlayerController occupyingPlayer = nearestTile.CurrentPlayer;

            if (occupyingPlayer.level == level)
            {
                Debug.Log("Wtf");
                occupyingPlayer.level++;
                Destroy(gameObject);
            }
            else
            {
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
}
