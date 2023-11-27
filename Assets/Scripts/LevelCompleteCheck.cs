using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteCheck : MonoBehaviour
{
    public enum level
    {
        Underground,
        Sub_Tera_1,
        Sub_Tera_2,
        Sub_Tera_3,
        In_Tera_1,
        In_Tera_2
    }
    public level Level;
    private InteractionPoint interactionPoint;
    private BoxCollider box;
    [SerializeField]
    private bool isStart = false;
    // Start is called before the first frame update
    private void Awake() 
    {
        interactionPoint = this.GetComponent<InteractionPoint>();
        box = this.GetComponent<BoxCollider>();
        box.enabled = false;
        interactionPoint.enabled = false;
    }
    void OnEnable()
    {
        isStart = false;
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
                    if(isStart == false)
                    {
                        StartCoroutine(if_Dialouge_Start("if_Complete_Underground"));
                    }
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
            case level.In_Tera_1:
            {
                if(LevelManager.Instance.level3ClearCheckPoints[0] == true)
                {
                    box.enabled = true;
                    interactionPoint.enabled = true;
                }
                break;
            }
            case level.In_Tera_2:
            {
                if(LevelManager.Instance.isLevel3Clear)
                {
                    box.enabled = true;
                    interactionPoint.enabled = true;
                    if(isStart == false)
                    {
                        StartCoroutine(if_Dialouge_Start("if_Level3_AllClear"));
                    }
                }
                break;
            }
        }
    }
    IEnumerator if_interaction_Start()
    {
        isStart = true;
        GameManager.Instance.interactionManager.if_UnderGround_Clear();
        yield return new WaitForEndOfFrame();
    }

    IEnumerator if_Dialouge_Start(string logue_name)
    {
        isStart = true;
        GameManager.Instance.interactionManager.if_Clear_Dialogue(logue_name);
        yield return new WaitForEndOfFrame();
    }
}
