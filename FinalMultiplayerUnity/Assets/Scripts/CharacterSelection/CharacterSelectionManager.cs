﻿using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterSelection
{
    public class CharacterSelectionManager : MonoBehaviourPunCallbacks
    {
        private const string ClientIsReady_RPC = nameof(ClientIsReady);
        private const string GAME_SCENE_NAME = "SampleScene";
        
        public static CharacterSelectionManager Instance;
        
        public SelectableCharacter selectedCharacter;
        [SerializeField] private Button readyBtn;

        public bool isReady;
        private int _playersReady;

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
            }
        }

        private void Update()
        {
            readyBtn.interactable = selectedCharacter != null;
        }

        public void Ready()
        {
            if (!isReady)
            {
                isReady = true;
                readyBtn.interactable = false;
                
                SetPlayerColor();
                            
                NotifyReadyToMasterClient();
            }
            
        }

        private void SetPlayerColor()
        {
            
            string colorJson = JsonUtility.ToJson(selectedCharacter.color);
            
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "PlayerColor", colorJson }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            
            // set in room properties for AI
            Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            string colorKey = "PlayerColor_" + PhotonNetwork.LocalPlayer.ActorNumber;
            
            roomProps[colorKey] = colorJson;
            
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        }

        private void NotifyReadyToMasterClient()
        {
            photonView.RPC(ClientIsReady_RPC, RpcTarget.MasterClient);
        }
        
        [PunRPC]
        private void ClientIsReady(PhotonMessageInfo info)
        {
            if (PhotonNetwork.IsMasterClient)
            {
               

                _playersReady++;
                if (_playersReady >= PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    Debug.Log("Loading Game Scene");
                    PhotonNetwork.LoadLevel(GAME_SCENE_NAME);
                }
            }
        }
        
    }
}