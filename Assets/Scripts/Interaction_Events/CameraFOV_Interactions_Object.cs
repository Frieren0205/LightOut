using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof (BoxCollider))]
public class CameraFOV_Interactions_Object : MonoBehaviour
{
    public enum point_type
    {
        start,
        end
    }
    [Header("FOV값 조정 위치 구별")]
    public point_type Point_Type;
    [SerializeField]
    private bool iseventEnd = false;
    [Range(60, 90)]
    public float FOVvalue;
    private CinemachineVirtualCamera virtualCamera;
    private Player_Controll player;

    private void OnValidate()
    {
        BoxCollider box = this.GetComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(0.06873417f, 1,1);
        if(Point_Type == point_type.start) FOVvalue = 60;
        if(Point_Type == point_type.end) FOVvalue = 90;
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.GetComponent<Player_Controll>() && Point_Type == point_type.start)
        {
            player = other.GetComponent<Player_Controll>();
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            StartCoroutine(SCameraFOVSetting());
            GetComponent<BoxCollider>().enabled = false;
        }
        if(other.gameObject.GetComponent<Player_Controll>() && Point_Type == point_type.end)
        {
            player = other.GetComponent<Player_Controll>();
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            StartCoroutine(ECameraFOVSetting());
            GetComponent<BoxCollider>().enabled = false;
        }
    }
    IEnumerator SCameraFOVSetting()
    {
        while(!iseventEnd)
        {
            if(FOVvalue > 90)
            {
                iseventEnd = true;
            }
            FOVvalue = FOVvalue + player.Movement();
            virtualCamera.m_Lens.FieldOfView = FOVvalue;
            yield return new WaitForSecondsRealtime(0.05f);
            if(virtualCamera.m_Lens.FieldOfView < 60)
            {
                GetComponent<BoxCollider>().enabled = true;
                iseventEnd = false;
            }
        }
    }
    IEnumerator ECameraFOVSetting()
    {
        while(!iseventEnd)
        {
            if(FOVvalue <= 60)
            {
                iseventEnd = true;
            }
            FOVvalue = FOVvalue - player.Movement();
            virtualCamera.m_Lens.FieldOfView = FOVvalue;
            yield return new WaitForSecondsRealtime(0.05f);
            if(virtualCamera.m_Lens.FieldOfView > 90)
            {
                GetComponent<BoxCollider>().enabled = true;
                iseventEnd = false;
            }
        }
    }
}
