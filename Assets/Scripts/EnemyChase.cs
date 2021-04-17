using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public float forceStrength;     // How fast we move
    public Transform target;        // The thing we want to chase

    private Rigidbody rb;   

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Move in the correct direction with the set force strength
        Vector3 direction = ((Vector3)target.position - (Vector3)transform.position).normalized;
        rb.AddForce(direction * forceStrength);
    }
}
