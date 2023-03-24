using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Test2 : MonoBehaviour
{
    public Transform target_Transform;
    [SerializeField]
    private Rigidbody rb;
    private NavMeshPath path;
    private float Speed = 0.0f;
    public float MovementSpeed = 5;

    void Start()
    {
        path = new NavMeshPath();
        //target_Transform = gameObject.GetComponent<Player_Controll>().transform;
    }

    void FixedUpdate()
    {
        this.UpdateFollwingPath();
        //this.UpdateSight();
    }
    private void UpdateFollwingPath()
    {
        this.UpdateFollwingPath_Navigate();
        //this.UpdateFollwingPath_Follow();
    }

    private void UpdateFollwingPath_Follow()
    {
       
    }
    private void UpdateSight()
    {
        this.UpdateSight_FindPlayer();
    }

    private void UpdateSight_FindPlayer()
    {
        throw new NotImplementedException();
    }

    public void UpdateFollwingPath_Navigate()
    {
       Speed += Time.deltaTime;
       if(Speed > 1.0f)
       {
            Speed -= 1.0f;
            NavMesh.CalculatePath(transform.position, target_Transform.position, NavMesh.AllAreas, path);
       }
       for(int i = 0; i < path.corners.Length -1; i++)
       {
            Debug.DrawLine(path.corners[i], path.corners[i+1], Color.red);
       }
       if(path.status == NavMeshPathStatus.PathPartial)
       {
         //TODO 정지
       }
       else if(path.status == NavMeshPathStatus.PathComplete)
       {
        //TODO 움직이게 하기
       }
       else if(path.status == NavMeshPathStatus.PathInvalid)
       {
            //TODO 저장된 마지막 경로로
       }
    }
}
