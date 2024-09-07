using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonScripts.Managers
{
    public class ConnectionUIManager : MonoBehaviourPunCallbacks
    {
        public static ConnectionUIManager Instance;
        
        [Header("loading Screen")]
        [SerializeField] private GameObject loadingScreen;
        
        [Header("Main Connection")]
        [SerializeField] private GameObject connectionPanel;
        
        [SerializeField] public TMP_InputField playerNameIF;
        [SerializeField] private Button connectBtn;
        
       
        
        [Header("Room Connection")]
        [SerializeField] private GameObject chooseRoomPanel;
        
        [SerializeField] public TMP_InputField roomNameIF;
        [SerializeField] public Slider roomMaxPlayersSlider;
        [SerializeField] public TMP_Text roomMaxPlayersText;
        [SerializeField] private Button createRoomBtn;
        
        [Header("In Room")]
        [SerializeField] private GameObject inRoomPanel;

        [SerializeField] private Button startGameBtn;
        

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SwitchUIScreen(UIScreen.Connect);
            
            
        }

        private void Update()
        {
            if (connectionPanel.activeSelf)
            {
                connectBtn.interactable = playerNameIF.text.Length > 0;
            }

            if (chooseRoomPanel.activeSelf)
            {
                createRoomBtn.interactable = roomNameIF.text.Length > 0;
                roomMaxPlayersText.text = roomMaxPlayersSlider.value.ToString();
            }
        }

        public void ShowLoadingScreen()
        {
            SwitchUIScreen(UIScreen.Loading);
        }

        private void SwitchUIScreen(UIScreen uiScreen)
        {
            connectionPanel.SetActive(false);
            chooseRoomPanel.SetActive(false);
            inRoomPanel.SetActive(false);
            loadingScreen.SetActive(false);
            
            switch (uiScreen)
            {
                case UIScreen.Connect:
                    connectionPanel.SetActive(true);
                    break;
                case UIScreen.ChooseRoom:
                    chooseRoomPanel.SetActive(true);
                    break;
                case UIScreen.InRoom:
                    inRoomPanel.SetActive(true);
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        startGameBtn.gameObject.SetActive(false);
                    }
                    break;
                case UIScreen.Loading:
                    loadingScreen.SetActive(true);
                    break;
            }
        }

        private enum UIScreen
        {
            Connect,
            ChooseRoom,
            InRoom,
            Loading
        }

        #region Pun Callbacks

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            SwitchUIScreen(UIScreen.ChooseRoom);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            SwitchUIScreen(uiScreen: UIScreen.Connect);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            SwitchUIScreen(uiScreen: UIScreen.InRoom);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            SwitchUIScreen(UIScreen.ChooseRoom);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            SwitchUIScreen(UIScreen.ChooseRoom);
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            SwitchUIScreen(uiScreen: UIScreen.ChooseRoom);
        }

        #endregion
    }
}