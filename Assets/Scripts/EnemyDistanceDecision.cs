using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDistanceDecision : MonoBehaviour
{
    public float distanceForDecision;   // How close must the player be to change behaviour
    public Transform target;            // The thing we want to measure distance from

    private EnemyPatrol patrolBehaviour;
    private EnemyChase chaseBehaviour;

    void Awake()
    {
        patrolBehaviour = GetComponent<EnemyPatrol>();
        chaseBehaviour = GetComponent<EnemyChase>();

        // Turn off the chase behaviour to start with
        chaseBehaviour.enabled = false;
    }

    void Update()
    {
        // How far away are we from the target
        float distance = ((Vector3)target.position - (Vector3)transform.position).magnitude;

        // If we are closer to our target than our minimum distance
        if (distance <= distanceForDecision)
        {
            // Disable patrol and enable chasing
            patrolBehaviour.enabled = false;
            chaseBehaviour.enabled = true;
        }
        else
        {
            // Enable patrol and disable chasing
            patrolBehaviour.enabled = true;
            chaseBehaviour.enabled = false;
        }

    }
}
