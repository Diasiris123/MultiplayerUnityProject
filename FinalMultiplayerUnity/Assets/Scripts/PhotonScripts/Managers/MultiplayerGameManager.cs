using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Photon.Pun;
using StarterAssets;
using UnityEngine;
using Random = UnityEngine.Random;
using Photon.Realtime;
using System.Collections;
using CharacterSelection;
using UnityEngine.UIElements;
using Photon.Pun.UtilityScripts;
using Unity.VisualScripting;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MultiplayerGameManager : MonoBehaviourPunCallbacks
{
    private const string PLAYER_PREFAB = "Player/PlayerArmature";
    private const string AI_PREFAB = "AI/AIPrefab";
    private const string BOOST_PREFAP_NAME = "World/BoostPrefab";

    private const string ClientIsReady_RPC = nameof(ClientIsReady);
    private const string SetSpawnPoint_RPC = nameof(SetSpawnPoint);
    private const string SetBoostSpawner_RPC = nameof(SetBoostSpawner);
    private const string SetNextSpawnPoint_RPC = nameof(SetNextBoostSpawnPoint);

    [Header("Setup")]
    [SerializeField] private SpawnPoint[] spawnPoints;
    [SerializeField] private BoostSpawnPoint[] boostSpawners;

    [SerializeField] private CinemachineCamera cinemachineCamMain;
    [SerializeField] private CinemachineCamera cinemachineCamAim;
    [SerializeField] private GameObject cameraFollowTarget;

    private ThirdPersonController _myThirdPersonController;
    private ThirdPersonShooterController _myShooterController;

    private int _playersReady;
    private GameObject _playerObject;
    private GameObject _AIObject;
    private BoostSpawnPoint _nextBoostSpawnPoint;
    private string _roomName;
    private Vector3 _lastLocation;
    private Quaternion _lastRotation;


    private void Start()
    {
        _roomName = PhotonNetwork.CurrentRoom.Name;

        NotifyReadyToMasterClient();
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(nameof(WaitTenSecondsAndSpawnBoost));
        }
    }

    #region Game Setup

    private void NotifyReadyToMasterClient()
    {
        photonView.RPC(ClientIsReady_RPC, RpcTarget.MasterClient);
    }

    private void SpawnPlayer(Vector3 position, Quaternion rotation)
    {
        Debug.Log("Spawning player");
        _playerObject = PhotonNetwork.Instantiate(PLAYER_PREFAB, position, rotation);

        if (_playerObject.GetPhotonView().IsMine)
        {

            _playerObject.transform.LookAt(Vector3.zero);

            
            _myThirdPersonController = _playerObject.GetComponent<ThirdPersonController>();
            _myShooterController = _playerObject.GetComponent<ThirdPersonShooterController>();
            
            // setup cinemachine
            cameraFollowTarget = _myShooterController.cameraFollowTarget;

            cinemachineCamMain.Follow = cameraFollowTarget.transform;
            cinemachineCamAim.Follow = cameraFollowTarget.transform;
            _myShooterController.aimCamera = cinemachineCamAim;
            
            //disable controlls until all players are ready
            _myShooterController.enabled = false;
            _myThirdPersonController.enabled = false;
            
            
        }
    }

    private SpawnPoint GetRandomSpawnPoint()
    {
        List<SpawnPoint> availablePoints = new List<SpawnPoint>();
        foreach (var point in spawnPoints)
        {
            if (!point.IsTaken)
            {
                availablePoints.Add(point);
            }
        }

        if (availablePoints.Count == 0)
        {
            Debug.Log("all points are taken");
            return null;
        }

        int rnd = Random.Range(0, availablePoints.Count);
        return availablePoints[rnd];
    }

    private void SpawnBoost(BoostSpawnPoint boost)
    {
        if (boost != null)
        {
            boost.Take();
            GameObject item = PhotonNetwork.InstantiateRoomObject(BOOST_PREFAP_NAME, boost.transform.position, boost.transform.rotation);
            item.GetComponent<Boost>().spawner = boost;
        }
    }

    [PunRPC]
    private void SetBoostSpawner(int boostSpawnID)
    {
        foreach (var boostSpawn in boostSpawners)
        {
            if (boostSpawn.Id == boostSpawnID)
            {
                SpawnBoost(boostSpawn);
                break;
            }
        }
    }

    private BoostSpawnPoint GetRandomBoostSpawner()
    {
        List<BoostSpawnPoint> availableBoostSpawners = new();

        foreach (var spawner in boostSpawners)
        {
            if (!spawner.IsTaken)
            {
                availableBoostSpawners.Add(spawner);
            }
        }

        if (availableBoostSpawners.Count == 0)
        {
            Debug.Log("all spawners taken");
            return null;
        }

        int randomBoostSpawnerIndex = Random.Range(0, availableBoostSpawners.Count);
        return availableBoostSpawners[randomBoostSpawnerIndex];
    }

    private void SetNextBoostSpawnPoint()
    {
        _nextBoostSpawnPoint = GetRandomBoostSpawner();
    }

    private IEnumerator WaitTenSecondsAndSpawnBoost()
    {
        int spawnCount = 0;
        for(; ; )
        {
            SpawnBoost(_nextBoostSpawnPoint);
            SetNextBoostSpawnPoint();
            spawnCount++;
            yield return new WaitForSeconds(10f);
        }
    }
    
    private float GetPlayerHP(int playerID)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("HP_" + playerID, out object hp))
        {
            Debug.Log("Player HP: " + hp);
            return (float)hp;
            
        }
        else
        {
            Debug.LogWarning($"No HP information found for player with ID: {playerID}");
            
        }
        return 0;
    }
    
    private Color GetPlayerColor(Player player)
    {
        Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        string colorKey = "PlayerColor_" + player.ActorNumber;
        
        if (roomProps.ContainsKey(colorKey))
        {
            string colorJson = roomProps[colorKey] as string;
            Color playerColor = JsonUtility.FromJson<Color>(colorJson);

            return playerColor;
        }
        
        return Color.black;
    }

    #region RPCs

    [PunRPC]
    private void ClientIsReady(PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log(info.Sender + " is ready");

            SpawnPoint randomSpawnPoint = GetRandomSpawnPoint();
            if (randomSpawnPoint != null)
            {
                randomSpawnPoint.Take();
                photonView.RPC(SetSpawnPoint_RPC, info.Sender, randomSpawnPoint.transform.position, randomSpawnPoint.transform.rotation);
            }

            _playersReady++;
            if (_playersReady >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                Debug.Log("Game started");
                photonView.RPC(nameof(GameStarted), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void SetSpawnPoint(Vector3 position, Quaternion rotation)
    {
        Debug.Log("Setting spawn point");
        SpawnPlayer(position, rotation);
    }

    [PunRPC]
    private void GameStarted()
    {
        Debug.Log("Game started");
        
        //give control to players
        _myShooterController.enabled = true;
        _myThirdPersonController.enabled = true;
        
    }

    #endregion

    #endregion

    #region New Master Client

    [ContextMenu("Switch Master Client")]
    public void ChangeMasterClient()
    {
        Player MasterClientCandidate = PhotonNetwork.LocalPlayer.GetNext();

        bool success = PhotonNetwork.SetMasterClient(MasterClientCandidate);
        Debug.Log($"New Master Setting secces is {success}");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        Debug.Log($"The new master client is {newMasterClient}");
    }

    #endregion

    #region Disconnection & Regoin

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(!PhotonNetwork.IsMasterClient)
            return;
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"player has been disconnected. Player Active/Inactive {otherPlayer.IsInactive}");

        if (!otherPlayer.IsInactive)
        {
            Debug.Log(otherPlayer.UserId);
            _lastLocation = PhotonViewManager.Instance.GetPlayerTransform(otherPlayer.ActorNumber).position;
            _lastRotation = PhotonViewManager.Instance.GetPlayerTransform(otherPlayer.ActorNumber).rotation;
            _AIObject = PhotonNetwork.Instantiate(AI_PREFAB, _lastLocation, _lastRotation);
            _AIObject.GetComponent<AIHealth>().currentHealth = GetPlayerHP(otherPlayer.ActorNumber);
            _AIObject.GetComponent<AIColorSetter>().SetColor(GetPlayerColor(otherPlayer));
        }

        if (otherPlayer.IsMasterClient)
        {
            Debug.Log("player was the master client, changing master...");
            ChangeMasterClient();
        }
    }

    [ContextMenu("Disconnect")]
    public void Disconnect()
    {
        PhotonNetwork.LeaveRoom();
    }

    [ContextMenu("Rejoin")]
    public void RejoinRoom()
    {
        PhotonNetwork.RejoinRoom(_roomName);
    }



    #endregion
}