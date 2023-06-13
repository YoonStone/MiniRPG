using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 0 : 배경음, 1 : 발자국, 2 : 공격, 3 : 플레이어, 4 : 아이템
public enum Effect
{
    LevelUp = 5,
    EffectUp = 6,
    EffectDown = 7,
    GetItem = 8
}

public enum AttackAudio
{
    Sword,
    SwordSpin,
    Bow
}

public enum PlayerAudio
{
    GetHit,
    Dead
}

public enum ItemAudio
{
    Equip,
    Food,
    BuySell
}

public class AudioManager : MonoBehaviour
{
    [HideInInspector] public AudioSource[] audios;

    public AudioClip[] clip_BGM;
    public AudioClip[] clip_Walk;
    public AudioClip[] clip_Attack;
    public AudioClip[] clip_Player;
    public AudioClip[] clip_Item;

    public static AudioManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audios = GetComponents<AudioSource>();
        AudioCtrl_BGM(false);
    }

    public void AudioCtrl_BGM(bool isDungeon)
    {
        // 던전일 때와 아닐 때 BGM 랜덤 재생
        int randomClip = isDungeon ? Random.Range(2, 4) : Random.Range(0, 2);
        audios[0].clip = clip_BGM[randomClip];
        audios[0].Play();
    }

    public void AudioCtrl_Walk(bool isWalking)
    {
        // 걷는 중이고, 효과음이 재생되지 않을 때만 재생
        if (isWalking && !audios[1].isPlaying)
        {
            audios[1].Play();
        }
        // 걷는 중이 아니면 재생 중지
        else if (!isWalking)
        {
            audios[1].Stop();
        }
    }

    public void AudioCtrl_Effect(AttackAudio attackAudio)
    {
        audios[2].clip = clip_Attack[(int)attackAudio];
        audios[2].Play();
    }

    public void AudioCtrl_Effect(PlayerAudio playerAudio)
    {
        audios[3].clip = clip_Player[(int)playerAudio];
        audios[3].Play();
    }

    public void AudioCtrl_Effect(ItemAudio item)
    {
        audios[4].clip = clip_Item[(int)item];
        audios[4].Play();
    }

    public void AudioCtrl_Effect(Effect effect)
    {
        audios[(int)effect].Play();
    }    
}
