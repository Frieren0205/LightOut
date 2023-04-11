using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_Camera : MonoBehaviour
{
    public Player_Controll player;
    [SerializeField]
    private Transform playerTransform;

    public Vector3 offset;
    public float followedspeed = 0.15f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<Player_Controll>();
        playerTransform = player.transform;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        LimitedCameraMovement();   
    }

    private void LimitedCameraMovement()
    {
        Vector3 camera_position = playerTransform.position + offset;
        Vector3 lerp_position = Vector3.Lerp(transform.position, camera_position, followedspeed);
        transform.position = lerp_position;
    }
}
