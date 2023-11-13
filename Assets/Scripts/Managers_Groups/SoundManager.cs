using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if(!_instance)
            {
                GameObject container = GameObject.FindFirstObjectByType<GameManager>().gameObject;
                _instance = container.GetComponent<SoundManager>();
            }
            return _instance;
        }
    }
    public LevelManager.Level level
    {
        get
        {
            return LevelManager.Instance.level;
        }
    }
    public TextMeshProUGUI Master_Volume_Value_TMP;
    public TextMeshProUGUI SFX_Volume_Value_TMP;
    public TextMeshProUGUI BGM_Volume_Value_TMP;

    public AudioSource audioSource;
    [SerializeField]
    private AudioMixer m_AudioMixer;


    private void Awake() 
    {
        
    }
    public void SetMasterVolume(float sliderValue)
    {
        if(sliderValue <= -40f)
        {
            m_AudioMixer.SetFloat("Master_mixer", -80);
            Master_Volume_Value_TMP.text = "0";
        }
        else
        {
            m_AudioMixer.SetFloat("Master_mixer", sliderValue);
            Master_Volume_Value_TMP.text = Math.Round(((sliderValue /40f ) * 100f) + 100).ToString();
        }
    }

    public void SetBGMVolume(float sliderValue)
    {
        if(sliderValue <= -40f)
        {
            m_AudioMixer.SetFloat("BGM_mixer", -80);
            BGM_Volume_Value_TMP.text = "0";
        }
        else
        {
            m_AudioMixer.SetFloat("BGM_mixer", sliderValue);
            BGM_Volume_Value_TMP.text = Math.Round(((sliderValue /40f ) * 100f) + 100).ToString();
        }

    }
    public void SetSFXVolume(float sliderValue)
    {
        if(sliderValue <= -40f)
        {
            m_AudioMixer.SetFloat("SFX_mixer", -80);
            SFX_Volume_Value_TMP.text = "0";
        }
        else
        {
            m_AudioMixer.SetFloat("SFX_mixer", sliderValue);
            SFX_Volume_Value_TMP.text = Math.Round(((sliderValue /40f ) * 100f) + 100).ToString();
        }
    }

}
