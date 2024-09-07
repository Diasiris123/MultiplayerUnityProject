using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonScripts
{
    public class RoomListing : MonoBehaviourPunCallbacks
    {
        public GameObject roomButtonPrefab; 
        public Transform content; 

        private Dictionary<string, GameObject> roomListings = new Dictionary<string, GameObject>();

        private void Awake()
        {
            
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("room list update");
            Dictionary<string, RoomInfo> existingRooms = new Dictionary<string, RoomInfo>();
        
            // create/update the existing rooms
            foreach (RoomInfo room in roomList)
            {
                if (!roomListings.ContainsKey(room.Name))
                {
                    // Create a new room listing
                    CreateRoomListing(room);
                }
                else if (roomListings.ContainsKey(room.Name) && room.RemovedFromList)
                {
                    // The room is no longer available
                    RemoveRoomListing(room.Name);
                }
                else
                {
                    // Update existing room listing
                    UpdateRoomListing(room);
                }
            
                existingRooms[room.Name] = room;
            }
      
            // Remove room listings that are no longer in the received room list
            foreach (var roomName in new List<string>(roomListings.Keys))
            {
                if (!existingRooms.ContainsKey(roomName))
                {
                    RemoveRoomListing(roomName);
                }
            }
        }

        private void CreateRoomListing(RoomInfo room)
        {
            Debug.Log("create room listing" + room.Name);
            GameObject roomListing = Instantiate(roomButtonPrefab, content);
            var roomListScript = roomListing.GetComponent<RoomListingPrefab>();
                roomListScript.Init(room.Name, room.MaxPlayers,room.PlayerCount);
                
            roomListings.Add(room.Name, roomListing);
        }

        private void UpdateRoomListing(RoomInfo room)
        {
            GameObject roomListing = roomListings[room.Name];
            roomListing.GetComponent<RoomListingPrefab>().UpdateRoomInfo(room.PlayerCount);
        }

        private void RemoveRoomListing(string roomName)
        {
            Debug.Log("removed room listing" + roomName);
            if (roomListings.ContainsKey(roomName))
            {
                Destroy(roomListings[roomName]);
                roomListings.Remove(roomName);
            }
        }

       
    
    
    }
}