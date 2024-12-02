using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hyb.Utils;

public class GameManager : ManualSingletonMono<GameManager>
{
    private bool isMuted;
    private bool isVibrationEnabled; 

    [SerializeField] private GameObject losePanel;
    [SerializeField] private Transform posSpawnPlayerForBuy;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle vibrationToggle; 

    public bool canDrag;
    public bool isLose;
    [SerializeField] private int coinAmount = -1;
    [SerializeField] private Text coinTxt;
    private const string CoinKey = "CoinAmount";
    private const string MusicKey = "MusicMuted";
    private const string VibrationKey = "VibrationEnabled";

    public override void Awake()
    {
        base.Awake();
        LoadSettings(); 

        if (coinTxt != null)
            LoadCoins();
    }
    private void Start()
    {
        Time.timeScale = 1;
        canDrag = true;
        isLose = false;

        if (coinTxt != null)
            UpdateCoinTxt();
        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);

        if (vibrationToggle != null)
            vibrationToggle.onValueChanged.AddListener(OnVibrationToggleChanged);

        UpdateToggleUI();
    }

    private void Update()
    {
        // if (!canDrag)
        // {
        //     StartCoroutine(ChangeStateDrag());
        // }

    }

    public void VibrateDevice()
    {
        if (isVibrationEnabled)
        {
            Handheld.Vibrate(); 
            Debug.Log("Vibrate");
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(MusicKey, isMuted ? 1 : 0);
        PlayerPrefs.SetInt(VibrationKey, isVibrationEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        isMuted = PlayerPrefs.GetInt(MusicKey, 0) == 1;
        isVibrationEnabled = PlayerPrefs.GetInt(VibrationKey, 1) == 1;
    }

    private void UpdateToggleUI()
    {
        if (musicToggle != null)
            musicToggle.isOn = !isMuted;

        if (vibrationToggle != null)
            vibrationToggle.isOn = isVibrationEnabled;
    }

    public void OnMusicToggleChanged(bool value)
    {
        isMuted = !value;
        SaveSettings();
        AudioManager.Instance.audioSource.mute = isMuted; 
    }

    public void OnVibrationToggleChanged(bool value)
    {
        isVibrationEnabled = value;
        SaveSettings();
    }

    public void UpdateCoinTxt()
    {
        coinAmount++;
        coinTxt.text = coinAmount.ToString();
        SaveCoins();
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinKey, coinAmount);
        PlayerPrefs.Save();
    }
    public IEnumerator ChangeStateDrag()
    {
        yield return new WaitForSeconds(.5f);
        canDrag = true;
    }
    private void LoadCoins()
    {
        coinAmount = PlayerPrefs.GetInt(CoinKey, 0);
        coinTxt.text = coinAmount.ToString();
    }

    public void BuySkill_1()
    {
        if (coinAmount >= 100)
        {
            PlayerManager.Instance.PLayersAttack();
            coinAmount -= 100;
            SaveCoins();
            LoadCoins();
        }
    }
    public void PlayGame()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    public void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToStartGame()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    public void BuyPlayer()
    {
        if (coinAmount >= 50)
        {
            PlayerManager.Instance.SpawnPlayer(posSpawnPlayerForBuy, false);
            coinAmount -= 50;
            SaveCoins();
            LoadCoins();
        }
    }
    public void BuySkill_2(){
        if(coinAmount >= 150){
            EnemyManager.Instance.EnemyTakeDamage();
            coinAmount -= 150;
            SaveCoins();
            LoadCoins();
        }
    }
     public void ActiveLosePanel()
    {
        StartCoroutine(LoseCroutine());
    }

    IEnumerator LoseCroutine()
    {
        VibrateDevice();
        AudioManager.Instance.PlayAudioFailGame();
        yield return new WaitForSeconds(1f);
        losePanel.SetActive(true);
        Time.timeScale = 0;
    }
}
