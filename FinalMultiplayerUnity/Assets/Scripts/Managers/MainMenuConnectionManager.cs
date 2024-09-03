using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;

public class MainMenuConnectionManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("connecting...");

        PhotonNetwork.ConnectUsingSettings();
    }


    #region Pun Callbacks

     public override void OnConnectedToMaster()
        {
            Debug.Log("photon connection successful");
            base.OnConnectedToMaster();
        }
    
        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            Debug.Log("Disconnected from master. " + cause);
            
        }
    
        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            Debug.Log("joined lobby");
        }
        
    
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("joined room");
        }
    
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.Log("failed to join room, " + message);
        }
    
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.Log("created room successfully");
        }
    
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.Log("failed to create room, " + message);
        }
    
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log("left room");
        }
    
        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            Debug.Log("left lobby");
        }

    #endregion

   
}
