using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Spawnpoint
{
    public LevelManager.Level level;
    public Vector3 spawnpositionVec3;
}

public class SpawnManager : MonoBehaviour
{
    static GameObject container;
    static SpawnManager _instance;
    public static SpawnManager Instance
    {
        get
        {
            if(!_instance)
            {
                container = GameObject.FindFirstObjectByType<SpawnManager>().gameObject;
                _instance = container.GetComponent<SpawnManager>();
                //DontDestroyOnLoad(container); TODO : GameManager에서 이미 호출하고 있으니 제대로 작동하는지 체크할 것
            }
            return _instance;
        }
    }
    [Header("스폰시킬 플레이어 프리펩")]
    public GameObject player_Prepab;

    [Header("스폰포인트 리스트")]
    [SerializeField]
    public Spawnpoint[] spawnpoints;
    public Player_Controll SpawnPlayer(Spawnpoint spawnpoint)
    {
        Debug.Log("플레이어 생성");
        var playerobject = Instantiate(player_Prepab,spawnpoint.spawnpositionVec3, Quaternion.identity);
        playerobject.gameObject.name = "Player";
        DontDestroyOnLoad(playerobject);
        return playerobject.GetComponent<Player_Controll>();
    }
}
