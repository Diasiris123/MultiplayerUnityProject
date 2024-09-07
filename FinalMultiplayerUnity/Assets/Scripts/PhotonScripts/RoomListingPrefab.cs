using Photon.Pun;
using PhotonScripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonScripts
{
    public class RoomListingPrefab : MonoBehaviour
    {
        [SerializeField] TMP_Text roomNameText;
        [SerializeField] TMP_Text roomPlayersText;
        [SerializeField] Button joinRoomBtn;
    
        private int _maxPlayers;
        private string _roomName;
        private int _roomPlayers;

        public void Init(string roomName, int roomMaxPlayers, int inRoomPlayers)
        {
            _roomName = roomName;
            _maxPlayers = roomMaxPlayers;
            _roomPlayers = inRoomPlayers;
        
            InitVisualInfo();
            joinRoomBtn.onClick.AddListener(JoinRoom);
        }

        public void UpdateRoomInfo( int roomPlayers)
        {
            _roomPlayers = roomPlayers;
        
            UpdateVisualInfo();
        }

        private void InitVisualInfo()
        {
            roomNameText.text = _roomName;
            roomPlayersText.text = _roomPlayers.ToString() + "/" + _maxPlayers.ToString();
        }

        private void UpdateVisualInfo()
        {
            roomPlayersText.text = _roomPlayers.ToString() + "/" + _maxPlayers.ToString();
        }
        
        public void JoinRoom()
        {
            Debug.Log("joining " + roomNameText.text);
            ConnectionUIManager.Instance.ShowLoadingScreen();
            PhotonNetwork.JoinRoom(roomNameText.text);
            
            
        }
    }
}
