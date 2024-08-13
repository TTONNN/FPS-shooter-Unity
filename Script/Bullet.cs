using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   public float life = 2;
   //private Rigidbody rb;
   void Awake()
   {
    Destroy(gameObject, life);
    //rb = GetComponent<Rigidbody>();
   }
   //void OnCollisionEnter(Collision collision)   
   void OnCollisionEnter(Collision collision)
   {
      //GetComponent<IDamageable>()?.TakeDamage(((GunInfo)ItemInfo).damage);
      Debug.Log("Bullettouch");
   }
}
