using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Test2 : MonoBehaviour
{
    public Transform target_Transform;
    private Rigidbody rb;
    private EnemySight sight; 
    private NavMeshPath path;
    private float PathRefreshTime = 0.0f;
    public float MovementSpeed = 5;

    public Vector3[] WayPoints;
    private int currentWayPoints = 0;
    private float WayPointsArrivalDistance = 1.5f;

    void Start()
    {
        path = new NavMeshPath();
        sight = this.gameObject.GetComponentInChildren<EnemySight>();
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
       // 갱신 주기 
       PathRefreshTime += Time.deltaTime;
       if(PathRefreshTime > 1.0f)
       {
            PathRefreshTime -= 1.0f;
            // 경로 재계산
            NavMesh.CalculatePath(transform.position, target_Transform.position, NavMesh.AllAreas, path);
            if(path.status == NavMeshPathStatus.PathComplete)
            {
                WayPoints = path.corners;
            }
            else 
                WayPoints = null;
                currentWayPoints = 0;
       }
       if(WayPoints != null)
       {
        /*
            float TonextCorner = Vector3.Distance(transform.position, WayPoints[currentWayPoints]);
            if(TonextCorner <= )
            {
                currentWayPoints++;
            }*/
       }
       for(int i = 0; i < path.corners.Length -1; i++)
       {
            Debug.DrawLine(path.corners[i], path.corners[i+1], Color.red);
       }
    }
}
