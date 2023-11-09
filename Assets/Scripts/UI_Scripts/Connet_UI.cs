using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connet_UI : MonoBehaviour
{
    private GameManager gameManager;

    void OnEnable()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }
    public void Connect_toTitleScene()
    {
        gameManager.uIManager.toTitleScene();
    }
    public void Connect_toGameExit()
    {
        gameManager.uIManager.toGameExit();
    }
}
