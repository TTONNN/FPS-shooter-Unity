using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;


public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{	

	[SerializeField] Image healthbarImage;
	[SerializeField] GameObject ui;


	[SerializeField] TMP_Text healthbarText;
	
	[SerializeField] GameObject cameraHolder;

	[SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

	[SerializeField] Item[] items;
	[SerializeField] GameObject DesUiGun1;
	[SerializeField] GameObject DesUiGun2;
	[SerializeField] GameObject DesUiGun3;
	[SerializeField] GameObject sounddes;
	

	int itemIndex;
	int previousItemIndex = -1;

	float verticalLookRotation;
	bool grounded;
	Vector3 smoothMoveVelocity;
	Vector3 moveAmount;

	Rigidbody rb;

	PhotonView PV;

	const float maxHealth = 100f;
	float currentHealth = maxHealth;

	PlayerManager playerManager;

	SingleShotGun singleShotGun;


	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();
		singleShotGun = GetComponentInChildren<SingleShotGun>();
		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();	
	}
/*
	[PunRPC]
    private void OnPlayerCreated()
    {

        photonView.RPC("OnPlayerCreated", RpcTarget.All);
    }*/
	 void Start()
	{	
        Cursor.visible = false;						//ADD
        Cursor.lockState = CursorLockMode.Locked;	//ADD
		if(PV.IsMine)
		{
			EquipItem(0);
		}
		else
		{
			Destroy(GetComponentInChildren<Camera>().gameObject);
			Destroy(rb);
			Destroy(ui);
			Destroy(DesUiGun1);
			Destroy(DesUiGun2);
			Destroy(DesUiGun3);
			Destroy(sounddes);
			
		}
	}
/*
	void DestroyOtherPlayerComponents()
{
   foreach(Transform child in transform)
    {
        if(child != cameraHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }
}*/



	public void Update()
	{	

		
		if(!PV.IsMine)
		return;

		Look();
		Move();
		Jump();

		for(int i = 0; i < items.Length; i++)
		{
			if(Input.GetKeyDown((i + 1).ToString()))
			{
				EquipItem(i);
				break;
			}
		}

		if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
		{
			if(itemIndex >= items.Length - 1)
			{
				EquipItem(0);
			}
			else
			{
				EquipItem(itemIndex + 1);
			}
		}
		else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
		{
			if(itemIndex <= 0)
			{
				EquipItem(items.Length - 1);
			}
			else
			{
				EquipItem(itemIndex - 1);
			}
		}

		if(Input.GetMouseButtonDown(0))
		{
			items[itemIndex].Use();
		}

		StartCoroutine(PressAndHoldLeft());

	
			RIGHTCLICK();
		
			

		if(transform.position.y < -10f) // Die if you fall out of the world
		{
			Die();
		}

	}

	void RIGHTCLICK()
	{
/*
 if (singleShotGun != null)			//check error
    {
        singleShotGun.PlayRightClickAnimation();
    }
    else
    {
        Debug.LogError("singleShotGun is null!");
    }*/

		if(Input.GetMouseButtonDown(1))
		{	
			singleShotGun.PlayRightClickAnimation();
			Debug.Log("Right Click");
		}
		else if (Input.GetMouseButtonUp(1))
    {	
		singleShotGun.StopRightClickAnimation();
        Debug.Log("Right Click (released)");
    }
	}

	IEnumerator PressAndHoldLeft()
{
    while (true)
    {
        if (Input.GetMouseButton(0))
        {	
			items[itemIndex].Use();
            //Debug.Log("Left Click");
        }

        yield return new WaitForSeconds(0.1f); // adjust the delay as needed when mouse0 click hold
    }
}

	void Look()                                             //can look x because code x can y it function call in Update u can change Look to Update
	{
		transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

		verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

		cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
	}

	void Move()
	{
		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

		moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);


	}

	void Jump()
	{
		if(Input.GetKeyDown(KeyCode.Space) && grounded)
		{
			rb.AddForce(transform.up * jumpForce);
		}

	}


	void EquipItem(int _index)
	{
		if(_index == previousItemIndex)
			return;

		itemIndex = _index;

		items[itemIndex].itemGameObject.SetActive(true);

		if(previousItemIndex != -1)
		{
			items[previousItemIndex].itemGameObject.SetActive(false);
		}

		previousItemIndex = itemIndex;

		if(PV.IsMine)
		{
			Hashtable hash = new Hashtable();
			hash.Add("itemIndex", itemIndex);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		}
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if(changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
		{
			EquipItem((int)changedProps["itemIndex"]);
		}
	}

	public void SetGroundedState(bool _grounded)
	{
		grounded = _grounded;
	}

	void FixedUpdate()
	{
		if(!PV.IsMine)
			return;

		rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}

	public void TakeDamage(float damage)
	{
		PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
	}

	[PunRPC]
	void RPC_TakeDamage(float damage, PhotonMessageInfo info)
	{

		currentHealth -= damage;

		healthbarImage.fillAmount = currentHealth / maxHealth;
		healthbarText.text = currentHealth.ToString();

		if(currentHealth <= 0)
		{
			Die();
			PlayerManager.Find(info.Sender).GetKill();
		}
	}
	
	public void TakeHeal(float ammountHeal)
	{
		PV.RPC(nameof(RPC_TakeHeal), PV.Owner, ammountHeal);
	}

	[PunRPC]
	void RPC_TakeHeal(float ammountHeal, PhotonMessageInfo info)
	{

		currentHealth += ammountHeal;

		healthbarImage.fillAmount = currentHealth / maxHealth;
		healthbarText.text = currentHealth.ToString();
	}

	void Die()
	{
		playerManager.Die();
	}
	
}