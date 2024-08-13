using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class RooomListItem : MonoBehaviour
{
    	PlayerManager playerManager;
    [SerializeField] TMP_Text text;

    public RoomInfo info;

   public void SetUp(RoomInfo _info)
   {
    info = _info;
    text.text = _info.Name;
   }

   public void OnClick()
   {
    Launcher.Instance.JoinRoom(info);
/*
    playerManager.PlayerJoinAfter();
	Debug.Log("playerManagerOnclick workkkkkkkkkkkkkkk");*/
	}
}
