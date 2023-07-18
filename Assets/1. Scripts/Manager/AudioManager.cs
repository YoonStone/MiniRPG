using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

// 0 : 배경음, 1 : 발자국, 2 : 공격, 3 : 플레이어, 4 : 아이템
public enum SFX
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

    public AudioMixer audioMixer;
    public GameObject setting;
    public Slider[] audioSliders;

    public AudioClip[] clip_BGM;
    public AudioClip[] clip_Walk;
    public AudioClip[] clip_Attack;
    public AudioClip[] clip_Player;
    public AudioClip[] clip_Item;

    DataManager dm;
    public static AudioManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) Destroy(gameObject);
    }

    void Start()
    {
        audios = GetComponents<AudioSource>();
        dm = DataManager.instance;

        AudioCtrl_BGM(false);
    }

    // 설정화면 열기
    public void OpenSetting()
    {
        // 저장되어있는 볼륨 > 슬라이더
        for (int i = 0; i < 2; i++)
        {
            audioSliders[i].value = dm.data.volumes[i];
        }

        setting.SetActive(true);
    }

    // 설정화면 닫기
    public void CloseSetting()
    {
        if(GameManager.instance) GameManager.instance.UIAndMoveCtrl(false);
        setting.SetActive(false);
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

    public void AudioCtrl_SFX(AttackAudio attackAudio)
    {
        audios[2].clip = clip_Attack[(int)attackAudio];
        audios[2].Play();
    }

    public void AudioCtrl_SFX(PlayerAudio playerAudio)
    {
        audios[3].clip = clip_Player[(int)playerAudio];
        audios[3].Play();
    }

    public void AudioCtrl_SFX(ItemAudio item)
    {
        audios[4].clip = clip_Item[(int)item];
        audios[4].Play();
    }

    public void AudioCtrl_SFX(SFX effect)
    {
        audios[(int)effect].Play();
    }

    // 실제 볼륨 조절
    public void SetVolume(int number, float value)
    {
        dm.data.volumes[number] = value;
        audioMixer.SetFloat("Volume" + number, Mathf.Log10(value) * 20);
        print(Mathf.Log10(value) + ", " + Mathf.Log10(value) * 20);
    }

    // 슬라이더 > 볼륨
    public void OnChangeVolume(int number)
    {
        SetVolume(number, audioSliders[number].value);

        if (number == 1 && !isChangeSFX)
        {
            changeCoolTime = 0;
            isChangeSFX = true;
            AudioCtrl_SFX(SFX.GetItem);
        }
    }

    // 효과음 조절 시 미리듣기
    public void SFXPreview()
    {
        AudioCtrl_SFX(SFX.GetItem);
    }

    // 효과음 조절 시 미리듣기가 너무 많이 나오지 않도록 조절 
    public bool isChangeSFX;
    public float changeCoolTime;
    private void Update()
    {
        if(isChangeSFX)
        {
            changeCoolTime += Time.deltaTime;

            if (changeCoolTime >= 0.2f) isChangeSFX = false;
        }
    }
}
