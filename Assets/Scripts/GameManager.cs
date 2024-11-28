using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hyb.Utils;

public class GameManager : ManualSingletonMono<GameManager>
{
    [SerializeField] private Sprite _musicImg;
    [SerializeField] private Sprite _muteImg;
    [SerializeField] private List<Text> _levelIndex;
    public bool isMuted;
    [SerializeField] private List<Button> _musicButtons;
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Text timeCountDown;
    [SerializeField] private int timeSum = 60;
    [SerializeField] private GameObject winGamePanel;
    public int goldCount;
    [SerializeField] private int goldNum;
    private float _remainingTime; 

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        goldCount = 0;
        Time.timeScale = 1;
        _remainingTime = timeSum; 

        if (_levelIndex.Count != 0)
        {
            foreach (Text txt in _levelIndex)
                txt.text = "Level " + (SceneManager.GetActiveScene().buildIndex - 1).ToString();
        }

        isMuted = PlayerPrefs.GetInt("isMuted", 0) == 1;
        if (_musicButtons.Count != 0)
        {
            foreach (Button btn in _musicButtons)
            {
                btn.image.sprite = isMuted ? _muteImg : _musicImg;
                btn.onClick.AddListener(OnMusicButtonClick);
            }
        }
        if(timeCountDown!= null)
            StartCoroutine(TimeCountdownCoroutine());
    }

    private IEnumerator TimeCountdownCoroutine()
    {
        while (_remainingTime > 0)
        {
            UpdateTimeDisplay();
            _remainingTime -= Time.deltaTime;
            yield return null;
        }
        ActiveLosePanel();
    }

    private void UpdateTimeDisplay()
    {
        int minutes = Mathf.FloorToInt(_remainingTime / 60); 
        int seconds = Mathf.FloorToInt(_remainingTime % 60); 
        timeCountDown.text = string.Format("{0:0} : {1:00}", minutes, seconds);
    }

    public void OnMusicButtonClick()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("isMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        foreach (Button btn in _musicButtons)
        {
            btn.image.sprite = isMuted ? _muteImg : _musicImg;
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

    public void ActiveSuccessPanel()
    {
        if(SceneManager.GetActiveScene().buildIndex != SceneManager.sceneCountInBuildSettings - 1){
            StartCoroutine(SuccessCroutine());
        }
        else{
            StartCoroutine(WinCroutine());
        }
    }

    IEnumerator SuccessCroutine()
    {
        yield return new WaitForSeconds(1f);
        // AudioManager.Instance.PlayAudioSuccessGame();
        successPanel.SetActive(true);
        Time.timeScale = 0;
    }
    IEnumerator WinCroutine()
    {
        yield return new WaitForSeconds(1f);
        // AudioManager.Instance.PlayAudioSuccessGame();
        winGamePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ActiveLosePanel()
    {
        StartCoroutine(LoseCroutine());
    }

    IEnumerator LoseCroutine()
    {
        yield return new WaitForSeconds(1f);
        // AudioManager.Instance.PlayAudioFailGame();
        losePanel.SetActive(true);
        Time.timeScale = 0;
    }
    public int GetGoldNum(){
        return goldNum;
    }
}
