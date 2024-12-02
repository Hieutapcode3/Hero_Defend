using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hyb.Utils;
using TMPro;
using UnityEngine.UI;
public class PlayerManager : ManualSingletonMono<PlayerManager>
{
    [SerializeField] private GameObject playerPref;
    private List<PlayerController> players;
    private int mainLevel = 0;
    [SerializeField] private Text mainLevelTxt;
    [SerializeField] private Image weaponImg;
    [SerializeField] private List<Sprite> weapons;
    public override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        players = new List<PlayerController>(gameObject.GetComponentsInChildren<PlayerController>());
        UpdateMainLevelTxt();
    }
    public void PLayersAttack(){
        foreach(PlayerController player in players){
            player.StartCoroutine(player.Attack());
        }
    }
    public void RemovePlayer(PlayerController player){
        players.Remove(player);
    }
    public void SpawnPlayer(Transform spawnPos,bool isGoldChest){
        // GameManager.Instance.canDrag = false;
        GroundTile randomTile = GroundManager.Instance.GetRandomUnoccupiedTile();
        if (randomTile != null)
        {
            GameObject newPlayerObj = Instantiate(playerPref,spawnPos.position,Quaternion.identity);
            newPlayerObj.transform.SetParent(this.transform);
            PlayerController newPlayer = newPlayerObj.GetComponent<PlayerController>();
            if(!isGoldChest)
                newPlayer.SetMyLevel(mainLevel);
            else
                newPlayer.SetMyLevel(mainLevel + 1);
            newPlayer.UpdateMyLevel();
            players.Add(newPlayer);
            StartCoroutine(MovePlayerToTile(newPlayer.gameObject, randomTile));
            randomTile.SetPlayer(newPlayer);
        }
    }
    private IEnumerator MovePlayerToTile(GameObject player, GroundTile targetTile)
    {
        Vector3 startPosition = player.transform.position;
        Vector3 targetPosition = targetTile.transform.position;
        float elapsedTime = 0f;
        float moveDuration = 0.2f; 

        while (elapsedTime < moveDuration)
        {
            player.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        player.transform.position = targetPosition;
    }
    public void UpdateMainLevelTxt(){
        mainLevel++;
        mainLevelTxt.text = mainLevel.ToString();
        if(mainLevel < weapons.Count){
            weaponImg.sprite = weapons[mainLevel - 1];
        }else
            weaponImg.sprite = weapons[weapons.Count - 1];
    }

}
