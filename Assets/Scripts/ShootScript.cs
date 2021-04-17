using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The main script that handles all the shooting and gun behaviors.
public class ShootScript : MonoBehaviour
{
    public GameObject Player;
    public GameObject[] allWeps; 
    public GameObject CurrentWep;
    private int GunDamage = 1;
    private float FireRate = 0.05f;
    private float Range = 50f;
    private float Hitforce = 100f;
    private Vector3 originalPos;
    private Vector3 aimingPos;
    private float AdsSpeed;
    private float reloadSpeed;

    public Camera fpsCamera;
    private WaitForSeconds ShotDuration = new WaitForSeconds(0.05f);
    public GameObject startpoint;
    public LineRenderer LaserLine;
    private float nextFire;

    public GameObject AmmoCountContainer;
    public int CurAmmocount = 0;
    public int MaxAmmocount = 0;

    public bool canFire = true;

    public GameObject grenadePrefab;
    private GameObject grenadeStartPoint;
    public float grenadeThrowForce = 700;

    // Start is called before the first frame update
    void Start()
    {
        AmmoCountContainer = GameObject.Find("AmmoDisplayContainer");
        allWeps = GameObject.FindGameObjectsWithTag("Weapon");
        foreach (GameObject weps in allWeps) {
            weps.SetActive(false);
        }
        allWeps[0].SetActive(true);
        LaserLine = GetComponent<LineRenderer>();
        fpsCamera = GetComponentInChildren<Camera>();
        CurrentWep = allWeps[0];
        GetWepStats(CurrentWep);
        

        startpoint = GameObject.Find("FirePoint");
        grenadeStartPoint = GameObject.Find("GrenadeStartPoint");
        CurAmmocount = MaxAmmocount;
    }

    // Update is called once per frame
    void Update()
    {
        //Throw Grenade
        if (Input.GetKeyDown(KeyCode.E)) {
            GameObject proj = Instantiate(grenadePrefab, grenadeStartPoint.transform.position, Quaternion.identity);
            proj.gameObject.GetComponent<Rigidbody>().AddForce(grenadeStartPoint.transform.forward * grenadeThrowForce);
        }
        //ADS
        if (Input.GetMouseButton(1) && canFire)
        {
            CurrentWep.transform.localPosition = Vector3.Lerp(CurrentWep.transform.localPosition, aimingPos, Time.deltaTime * AdsSpeed);
        }
        else {
            CurrentWep.transform.localPosition = Vector3.Lerp(CurrentWep.transform.localPosition, originalPos, Time.deltaTime * AdsSpeed);
        }

        //Firing
        while (Input.GetMouseButton(0) && (Time.time > nextFire) && (CurAmmocount > 0) && canFire) {
            nextFire = Time.time + FireRate;
            StartCoroutine(ShotEffect());
            Vector3 rayorigin = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            LaserLine.SetPosition(0, startpoint.transform.position);
            if (Physics.Raycast(rayorigin, fpsCamera.transform.forward, out hit, Range))
            {
                LaserLine.SetPosition(1, hit.point);
                Vector3 ray = hit.point - rayorigin;
                if (hit.rigidbody != null) {
                    hit.rigidbody.AddForce(ray.normalized * Hitforce);
                }
                GetComponent<DecalController>().SpawnDecal(hit);
                Debug.Log("Hit " + hit.collider.name + " " + hit.collider.tag);
                if (hit.collider.tag == "Enemy") {
                    Debug.Log(hit.collider.name + " took " + GunDamage + " damage.");
                    hit.collider.gameObject.GetComponentInParent<HealthScript>().TakeDamage(GunDamage);
                    
                }
                
            }
            else {
                LaserLine.SetPosition(1, rayorigin + (fpsCamera.transform.forward * Range));
            }
            CurAmmocount--;
            AmmoCountContainer.GetComponent<AmmoUpdater>().UpdateUI();
        }
        //Weapon Swapping
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwapWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwapWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SwapWeapon(3);
        }

        //Reloading
        if (Input.GetKeyDown(KeyCode.R) && (CurAmmocount != MaxAmmocount)) {
            StartCoroutine(ReloadEffect());
            
        }

    }
    
    private IEnumerator ShotEffect() { //Draws the line between the gun barrel and where it hit
        LaserLine.enabled = true;
        yield return ShotDuration;
        LaserLine.enabled = false;
    }

    private IEnumerator ReloadEffect() {
        Debug.Log("Reloading...");
        canFire = false;
        AmmoCountContainer.GetComponent<AmmoUpdater>().reloadingDisplay.enabled = true;
        yield return new WaitForSeconds(reloadSpeed);
        CurAmmocount = MaxAmmocount;
        AmmoCountContainer.GetComponent<AmmoUpdater>().UpdateUI();
        canFire = true;
        AmmoCountContainer.GetComponent<AmmoUpdater>().reloadingDisplay.enabled = false;
    }
    

    private void SwapWeapon(int WepNumber) {
        CurrentWep.GetComponent<GunStats>().CurAmmoCount = CurAmmocount;
        switch (WepNumber) {
            case 1:
                Debug.Log("Swapped To AssultRifle");
                CurrentWep.SetActive(false);
                CurrentWep = allWeps[0];
                CurrentWep.SetActive(true);
                GetWepStats(CurrentWep);
                break;
            case 2:
                Debug.Log("Swapped To Pistol");
                CurrentWep.SetActive(false);
                CurrentWep = allWeps[1];
                CurrentWep.SetActive(true);
                GetWepStats(CurrentWep);
                break;
            case 3:
                Debug.Log("Swapped To Machinegun");
                CurrentWep.SetActive(false);
                CurrentWep = allWeps[2];
                CurrentWep.SetActive(true);
                GetWepStats(CurrentWep);
                break;
            default:
                Debug.Log("WeaponSwap Switch Statment Failed");
                break;
        }
        startpoint = GameObject.Find("FirePoint");
    }
    private void GetWepStats(GameObject WepPrefab) { //Updates the stats of ShootScript to those of currently equiped wepeon, if a new stat is added to GunStats.cs this function must be updated 
       GunDamage = WepPrefab.GetComponent<GunStats>().GunDamage;
        Range = WepPrefab.GetComponent<GunStats>().Range;
        FireRate = WepPrefab.GetComponent<GunStats>().FireRate;
        Hitforce = WepPrefab.GetComponent<GunStats>().Hitforce;
        originalPos = WepPrefab.GetComponent<GunStats>().originalPos;
        aimingPos = WepPrefab.GetComponent<GunStats>().aimingPos;
        AdsSpeed = WepPrefab.GetComponent<GunStats>().AdsSpeed;
        CurAmmocount = WepPrefab.GetComponent<GunStats>().CurAmmoCount;
        MaxAmmocount = WepPrefab.GetComponent<GunStats>().MaxAmmocount;
        AmmoCountContainer.GetComponent<AmmoUpdater>().UpdateUI();
        reloadSpeed = WepPrefab.GetComponent<GunStats>().reloadSpeed;
    }

   
}
