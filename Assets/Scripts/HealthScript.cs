using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This scripted handles anything that has health, with Tag based conditions for if the gameobject is a player or an ememy
//Make sure any gameobject this script is attatched to has either a "Player" or "Enemy" Tag
public class HealthScript : MonoBehaviour
{
    public float currentArmor;
    public float maxArmor = 30f;
    public float currentHealth;
    public float maxHealth = 100f;
    public float regenAmount = 3f;
    public float regenSpeed = 1f;
    public float regenDelay = 3f;
    public GameObject HealthUI;
    

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;
        if (this.tag == "Player") {
            HealthUI = GameObject.Find("HealthDisplayContainer");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
        if (currentHealth == maxHealth) {
            CancelInvoke("Regeneration");
        }
        //Damage Testing Code
        //if (Input.GetKeyDown(KeyCode.E)) {
        //    TakeDamage(50);
        //}
    }

    public void TakeDamage(float Damage) {

        for (int i = 0; i < Damage; i++) {
            if (currentArmor > 0)
            {
                currentArmor -= 1;
            }
            else
            {
                currentHealth -= 1;
            }
        }
        
        
        CancelInvoke("Regeneration");
        InvokeRepeating("Regeneration", regenDelay, regenSpeed);
        if (currentHealth <= 0 && (this.tag != "Player")) {
            this.gameObject.SetActive(false);
        }
        PlayerUpdateHealth();
    }

    public void Regeneration() {
        currentHealth += regenAmount;
        PlayerUpdateHealth();
    }

    public void Armorrefill() {
        currentArmor = maxArmor;
        PlayerUpdateHealth();
    }

    public void PlayerUpdateHealth() {
        if (this.tag == "Player") {
            HealthUI.GetComponent<HealthUpdater>().UpdateHealth(currentHealth, currentArmor);
        }
        
    }
}
