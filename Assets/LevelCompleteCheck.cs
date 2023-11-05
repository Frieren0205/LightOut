using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelCompleteCheck : MonoBehaviour
{
    public enum level
    {
        Underground,
        Sub_Tera_1,
        Sub_Tera_2,
        Sub_Tera_3,
        In_Tera
    }
    public level Level;
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
        switch(Level)
        {
            case level.Underground:
            {
                if(LevelManager.Instance.isLevel1Clear)
                {
                    box.enabled = true;
                    interactionPoint.enabled = true;
                }
                break;
            }
            case level.Sub_Tera_1:
            {
                if(LevelManager.Instance.level2ClearCheckPoints[0])
                {
                    box.enabled = true;
                    interactionPoint.enabled = true;
                }
                break;
            }
            case level.Sub_Tera_2:
            {
                if(LevelManager.Instance.level2ClearCheckPoints[0] && LevelManager.Instance.level2ClearCheckPoints[1])
                {
                    box.enabled = true;
                    interactionPoint.enabled = true;
                }
                break;
            }
            case level.Sub_Tera_3:
            {
                if(LevelManager.Instance.isLevel2Clear)
                {
                    box.enabled = true;
                    interactionPoint.enabled = true;
                }
                break;
            }
            case level.In_Tera:
            {
                if(LevelManager.Instance.isLevel3Clear)
                {
                    box.enabled = true;
                    interactionPoint.enabled = true;
                }
                break;
            }
        }
    }
}
