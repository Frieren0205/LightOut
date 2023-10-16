using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelCompleteCheck : MonoBehaviour
{
    private InteractionPoint interactionPoint;
    private BoxCollider box;
    // Start is called before the first frame update
    private void OnEnable() 
    {
        interactionPoint = this.GetComponent<InteractionPoint>();
        box = this.GetComponent<BoxCollider>();
        box.enabled = false;
        interactionPoint.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(LevelManager.Instance.isLevelClear[0])
        {
            box.enabled = true;
            interactionPoint.enabled = true;
        }
    }
}
