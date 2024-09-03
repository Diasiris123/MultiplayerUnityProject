using System;
using UnityEngine;
using Photon.Pun;

public class MainMenuConnectionManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("connecting...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("photon connection successful");
        base.OnConnectedToMaster();
    }
}
