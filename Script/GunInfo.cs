using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "FPS/New Gun")]		//have this line because create guninfo
public class GunInfo : ItemInfo			
{
    public float damage;					 //control darmage
	public AudioClip HitSound;
	

    public void PlayHitSound(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(HitSound, position);
    }
}