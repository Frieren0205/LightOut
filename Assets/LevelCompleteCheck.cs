using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelCompleteCheck : MonoBehaviour
{
    static LevelCompleteCheck _instance;
    public static LevelCompleteCheck Instance
    {
        get
        {
            if(!_instance)
            {
                GameObject container = FindFirstObjectByType<LevelCompleteCheck>().gameObject;
                _instance = container.GetComponent<LevelCompleteCheck>();
            }
            return _instance;
        }
    }
    public Enemy_Test2[] target_enemy;
    private InteractionPoint interactionPoint;
    // Start is called before the first frame update
    private void OnEnable() 
    {
        interactionPoint.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(target_enemy.Any())
        {
            interactionPoint.enabled = true;
        }
    }
}
