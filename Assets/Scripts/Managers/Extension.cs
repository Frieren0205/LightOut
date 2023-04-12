using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject gameObject)where T : Component
    {
        if(gameObject.TryGetComponent<T>(out T t))
        {
            return t;
        }
        else
        {
            return gameObject.AddComponent<T>();
        }
    }
}
