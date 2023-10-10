using System;
using UnityEngine;

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


    AudioSource audioSource;
}
