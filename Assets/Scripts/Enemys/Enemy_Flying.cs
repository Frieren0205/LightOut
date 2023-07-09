using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Flying : MonoBehaviour
{
    [SerializeField]
    private Player_Controll player;


    private bool isflying;
    [SerializeField]
    private float flydistance;

    private void OnEnable() 
    {
        player = FindObjectOfType<Player_Controll>().GetComponent<Player_Controll>();
    }

    private void FixedUpdate() 
    {
        OnAir();    
    }

    private void OnAir()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        isflying = (Physics.Raycast(ray, out hit, flydistance));
        if(isflying)
        {
            transform.position = new Vector3(transform.position.x, flydistance, transform.position.z);
        }
        else
        {
            Debug.Log("비행중");
        }
    }
     
    private void OnMovement()
    {
        bool isflip = 0 <= (player.transform.position.x - transform.position.x);
    }
}
