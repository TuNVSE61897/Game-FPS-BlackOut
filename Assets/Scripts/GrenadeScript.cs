using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Handles behavoir of the grenade once instantiated, spawning of grenades is handled by ShootScript
public class GrenadeScript : MonoBehaviour
{
    public float fuseTimer = 3;
    public float blastRadius = 5;
    public float blastForce = 300;
    public float damage = 30;
   // public float throwForce = 20;
    public Collider[] closeObjects;
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * throwForce);
        Invoke("Explode", 3);
    }

    private void Explode() {
        closeObjects = Physics.OverlapSphere(this.transform.position, blastRadius);
        foreach (Collider col in closeObjects) {
            if (col.tag == "Player") {
                Debug.Log(col.name + " hit by grenade");
                col.GetComponent<HealthScript>().TakeDamage(damage);
            } else if (col.tag == "Enemy") {
                col.GetComponentInParent<HealthScript>().TakeDamage(damage);
            }
            if (col.GetComponent<Rigidbody>() != null) {
                Debug.Log("Force apllied to " + col.name);
                col.attachedRigidbody.AddExplosionForce(blastForce, this.transform.position, blastRadius, 1.0f);
                //col.attachedRigidbody.AddForce(blastForce)
            }
            

        }
        Destroy(this.gameObject);
    }
}
