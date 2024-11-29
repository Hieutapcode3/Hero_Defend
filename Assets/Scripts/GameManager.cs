using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hyb.Utils;

public class GameManager : ManualSingletonMono<GameManager>
{
    public bool isMuted;
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject winGamePanel;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Time.timeScale = 1;
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
}
