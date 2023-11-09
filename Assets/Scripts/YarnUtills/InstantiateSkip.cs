using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateSkip : MonoBehaviour
{
    private GameManager gameManager;
    public void OnClick()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gameManager.NextSceneLoad();
    }
}
