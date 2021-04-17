using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A holder script for all the stats of a gun prefab, all guns are varients of the "Gun" prefab and have these stats at verying amounts
//If a stat is added it must be added to the GetWepStats function in ShootScript 
public class GunStats : MonoBehaviour
{
    public int GunDamage = 1;
    public float FireRate = 0.05f;
    public float Range = 50f;
    public float Hitforce = 100f;
    public Vector3 originalPos;
    public Vector3 aimingPos;
    public float AdsSpeed = 100;
    public int MaxAmmocount = 36;
    public int CurAmmoCount = 36;
    public float reloadSpeed = 1;

    
}
