using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hyb.Utils;
public class AudioManager : ManualSingletonMono<AudioManager>
{
    [Header("------------Audio------------")]
    public AudioSource audioSource;
    public AudioClip drop;
    public AudioClip failGame;
    public AudioClip merge;
    public AudioClip coin;

    public override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayAudioFailGame()
    {
        audioSource.PlayOneShot(failGame);
    }public void PlayAudioDrop()
    {
        audioSource.PlayOneShot(drop);
    }
    public void PlayAudioMergePlayer()
    {
        audioSource.PlayOneShot(merge);
    }
    public void PlayAudioCollisionCoin()
    {
        audioSource.PlayOneShot(coin,0.5f);
    }

}
