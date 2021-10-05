using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Variables")] 
    [SerializeField] float _currentSpeed = 4;
    [Header("Waypoints")] [SerializeField] Transform[] waypoints;
    int waypointIndex = 0;

    void Update()
    {
        if (waypoints.Length > 0)
        {
            if (waypointIndex >= waypoints.Length)
                waypointIndex = 0;

            Vector3 targetPosition = new Vector3(waypoints[waypointIndex].position.x, waypoints[waypointIndex].position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _currentSpeed * Time.deltaTime);

            if (transform.position == targetPosition)
                waypointIndex += 1;
        }    
    }
}
