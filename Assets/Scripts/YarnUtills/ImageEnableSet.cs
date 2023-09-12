using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;

public class ImageEnableSet : MonoBehaviour
{
    private Image img => this.gameObject.GetComponent<Image>();
    [YarnCommand("EnableSet")]
    private void EnableSet()
    {
        img.enabled = true;
    }
    [YarnCommand("DisableSet")]
    private void DisableSet()
    {
        img.enabled = false;
    }
}
