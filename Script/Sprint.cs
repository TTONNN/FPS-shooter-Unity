using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Sprint : MonoBehaviourPunCallbacks {
    public bool sprinting;
	// Use this for initialization
	void Start () {
        sprinting = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {   
            sprinting = true;
            Sprint1();
        }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
               StartCoroutine(WaitForSprintReturn());
            }
        }

IEnumerator WaitForSprintReturn()
{
    SprintReturn();
    yield return new WaitForSeconds(0.2f);
    sprinting = false;
    
}

    void Sprint1 ()
    {
        GetComponent<Animation>().Stop("Sprint");
        GetComponent<Animation>().Play("Sprint");
    }

        void SprintReturn()
        {
            GetComponent<Animation>().Stop("SprintReturn");
            GetComponent<Animation>().Play("SprintReturn");
        }
    }
