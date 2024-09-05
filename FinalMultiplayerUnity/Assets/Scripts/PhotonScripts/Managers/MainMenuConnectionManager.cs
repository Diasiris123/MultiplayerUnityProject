using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace PhotonScripts.Managers
{
    public class MainMenuConnectionManager : MonoBehaviourPunCallbacks
    {
        private const string GAME_SCENE_NAME = "SampleScene";
        private const string LOBBY_NAME = "MyLobby";
        
        public void ConnectToPhoton()
        {
            PhotonNetwork.NickName = ConnectionUIManager.Instance.playerNameIF.text;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        
        
        
        public void CreateRoom()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                RoomOptions roomOptions = new RoomOptions()
                {
                    MaxPlayers = (int)ConnectionUIManager.Instance.roomMaxPlayersSlider.value
                };
                PhotonNetwork.CreateRoom(ConnectionUIManager.Instance.roomNameIF.text, roomOptions, TypedLobby.Default);
            }
            else
            {
                Debug.LogError("Not connected to Photon servers.");
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(GAME_SCENE_NAME);
            }
        }


        #region Pun Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("photon connection successful");
            base.OnConnectedToMaster();

            PhotonNetwork.JoinLobby(new TypedLobby(LOBBY_NAME, LobbyType.Default));
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

        private void OnGUI()
        {
            GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
        }
    }
}
