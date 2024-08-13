using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;
using UnityEngine.UI;


public class SingleShotGun : Gun
{   
    public Transform bulletSpawnPont;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;

    [SerializeField] Camera cam;
    
    public bool canFire;
    public float fireRate;
    float nextfire;
    public GameObject muzzleFlash;
    public AudioClip firesound;
    public int Ammo;
    public int ammomax;
    public int AmmoLeft;
    [SerializeField] TMP_Text AmmoText;
    [SerializeField] TMP_Text AmmoLeftText;
    public Transform mainCam;
    public Sprint sprint;
    public bool reloading;
    public float reloadtime;
    public GameObject targetAim;
    public bool Aimming;
    private Quaternion originalRotation;
    


    /*
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;     //recoil
    */
    void Start () 
    {
        StartCoroutine(drawdelay());
        originalRotation = cam.transform.rotation;
        Aimming = false;
    }

     IEnumerator drawdelay()
    {
        yield return new WaitForSeconds(0.5f);
        canFire = true;
    }





    void Update () {

        AmmoText.text = Ammo.ToString();
        //AmmoLeftText.text = AmmoLeft.ToString();            // //for inf
        
    	//if (Input.GetKeyDown(KeyCode.R) && reloading == false)        //before and it bug reload other gun
        if (Input.GetKeyDown(KeyCode.R) && reloading == false && sprint.sprinting == false && Ammo < ammomax)
        {	
                Reload();
                Debug.Log("Reloading");
        }


    /*
    targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
    currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);         //recoil
    transform.localRotation = Quaternion.Euler(currentRotation);   
    */
	}
    
    public void PlayRightClickAnimation()
    {   
        bulletPrefab.SetActive(false);
        targetAim.SetActive(false);
        Debug.Log("ZoomIN");
        GetComponent<Animation>().Stop("ScopeAnimationIN");
        GetComponent<Animation>().Play("ScopeAnimationIN");
        Aimming = true;
    }
    public void StopRightClickAnimation()
    {   
        bulletPrefab.SetActive(true);
        targetAim.SetActive(true); 
        Debug.Log("ZoomOUT");
        GetComponent<Animation>().Stop("ScopeAnimationOUT");
        GetComponent<Animation>().Play("ScopeAnimationOUT");
        Aimming = false;
    }

    void UpdateAmmo()
    {
        AmmoText.text = Ammo.ToString();
    }

    public void Reload ()
    {
        reloading = true;
        StartCoroutine(reloadwait());
        Ammo = ammomax;
        GetComponent<Animation>().Stop("Reload");
        GetComponent<Animation>().Play("Reload");
        Debug.Log("Reloaded");
    }


    

    IEnumerator reloadwait ()
    {
        yield return new WaitForSeconds(reloadtime);
        Ammo = ammomax;
        reloading = false;
    }



    public override void Use()
    {
                   AmmoText.text = Ammo.ToString();
        //AmmoLeftText.text = AmmoLeft.ToString();        // //for inf

        if (Input.GetMouseButton(0)  && canFire && Ammo > 0 && sprint.sprinting == false  && reloading == false)
        {
            Shoot();
            canFire = false;
        }

    }


    void Shoot()    

    {   
        var bullet = Instantiate(bulletPrefab, bulletSpawnPont.position, bulletSpawnPont.rotation);     //bullet
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPont.forward * bulletSpeed;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
        if (hit.collider.gameObject.CompareTag("Player"))
{
            //Debug.Log("We hit " + hit.collider.gameObject.name);                                                //debug hit object
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            ((GunInfo)itemInfo).PlayHitSound(hit.point);
}
        }

        //targetRotation += currentRotation + new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));             //recoil

        Ammo -= 1;

        GetComponent<AudioSource>().PlayOneShot(firesound);
        muzzleFlash.SetActive(true); 
        StartCoroutine(flashdelay());
        StartCoroutine(firedelay());
        StartCoroutine(RestoreRotation());

        //Animation
        if (Aimming == true){
            GetComponent<Animation>().Stop("AimFire");
            GetComponent<Animation>().Play("AimFire");
            Debug.Log("fireAIM");
        }
            if (Aimming == false){
        GetComponent<Animation>().Stop("Fire");
        GetComponent<Animation>().Play("Fire");
        Debug.Log("fire NO AIM");
        }
    }
	
        private IEnumerator RestoreRotation()
    {
        //recoil 1
        cam.transform.Rotate(-2, 0, 0); 
        yield return new WaitForSeconds(0.05f);
        cam.transform.Rotate(2, 0, 0); 
        //recoil 2
        /*
    float randomX = Random.Range(0f, -5f);
    float randomY = Random.Range(-2f, 2f);

    cam.transform.Rotate(randomX, randomY, 0);
    Debug.Log("recoil");
    yield return new WaitForSeconds(0.1f);
    cam.transform.Rotate(-randomX, -randomY, 0); 
    Debug.Log("recoil reset");
    */
    }


    IEnumerator flashdelay ()
    {
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }

    IEnumerator firedelay()
    {if(Time.time > nextfire)
        {
            nextfire = Time.time + fireRate;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
        }
    }


}
/*
public void OnAmmoPickup(int amount)
{
    Ammo += amount;
    AmmoLeft = Ammo;

    // Update the ammo text components
    AmmoText.text = Ammo.ToString();
    AmmoLeftText.text = AmmoLeft.ToString();                //pickupammo look cool
}*/


/*  if(Time.time > nextfire)
        {
            nextfire = Time.time + fireRate;*/
