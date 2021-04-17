using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthUpdater : MonoBehaviour
{
    public Text HealthDisplay;
    public Text ArmorDisplay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth(float health, float armor) {
        HealthDisplay.text = health.ToString();
        ArmorDisplay.text = armor.ToString();
    }
}
