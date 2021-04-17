using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script Updates the ammo counter Ui, it is primarily called from the ShootScript
//Toggling the reloadingDisplay is handled in ShootScript where UpdateUI is being called
public class AmmoUpdater : MonoBehaviour
{
    public GameObject player;
    public Text curAmmo;
    public Text maxAmmo;
    public Text reloadingDisplay;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        reloadingDisplay.enabled = false;
    }
    public void UpdateUI() { //This is the main function that updates the Ammo UI
        curAmmo.text = player.GetComponent<ShootScript>().CurAmmocount.ToString();
        maxAmmo.text = player.GetComponent<ShootScript>().MaxAmmocount.ToString();
    }
    
}
