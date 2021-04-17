using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]

public class EnemyPatrol : MonoBehaviour
{
    public float forceStrength;     // How fast we move
    public float stopDistance;      // How close we get before moving to next patrol point
    public GameObject[] patrolPoints;  // List of patrol points we will go between

    private int currentPoint = 0;       // Index of the current point we're moving towards
    private Rigidbody rb;   

   void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // How far away are we from the target
        float distance = (patrolPoints[currentPoint].transform.position - (Vector3)transform.position).magnitude;

        // If we are closer to our target than our minimum distance
        if (distance <= stopDistance)
        {
            // Update to the next target
            currentPoint = currentPoint + 1;

            // Loop back to the start
            if (currentPoint >= patrolPoints.Length)
            {
                currentPoint = 0;
            }
        }

        // Move in the correct direction with the set force strength
        Vector3 direction = (patrolPoints[currentPoint].transform.position - (Vector3)transform.position).normalized;
        rb.AddForce(direction * forceStrength);
    }
}