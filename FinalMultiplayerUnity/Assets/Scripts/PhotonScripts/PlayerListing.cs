using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviourPunCallbacks
{
    public GameObject playerItemPrefab; 
    public Transform content; 

    private Dictionary<int, GameObject> playerListings = new Dictionary<int, GameObject>();

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            UpdatePlayerList();
        }
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} entered the room.");
        AddPlayerToList(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player {otherPlayer.NickName} left the room.");
        RemovePlayerFromList(otherPlayer);
    }

    private void UpdatePlayerList()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if(player.NickName != PhotonNetwork.LocalPlayer.NickName)
                AddPlayerToList(player);
        }
    }

    private void AddPlayerToList(Player player)
    {
        if (!playerListings.ContainsKey(player.ActorNumber))
        {
            GameObject playerItem = Instantiate(playerItemPrefab, content);
            playerItem.GetComponent<PlayerListPrefab>().playerNameText.text = player.NickName;
            playerListings[player.ActorNumber] = playerItem;
        }
    }

    private void RemovePlayerFromList(Player player)
    {
        if (playerListings.ContainsKey(player.ActorNumber))
        {
            Destroy(playerListings[player.ActorNumber]);
            playerListings.Remove(player.ActorNumber);
        }
    }
}