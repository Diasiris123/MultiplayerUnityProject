using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Photon.Pun;
using StarterAssets;
using UnityEngine;
using Random = UnityEngine.Random;

public class MultiplayerGameManager : MonoBehaviourPunCallbacks
{
    private const string PLAYER_PREFAB = "Player/PlayerArmature";

    private const string ClientIsReady_RPC = nameof(ClientIsReady);
    private const string SetSpawnPoint_RPC = nameof(SetSpawnPoint);

    [Header("Setup")]
    [SerializeField] private SpawnPoint[] spawnPoints;

    [SerializeField] private CinemachineCamera cinemachineCamMain;
    [SerializeField] private CinemachineCamera cinemachineCamAim;
    [SerializeField] private GameObject cameraFollowTarget;

    private ThirdPersonController _myThirdPersonController;
    private ThirdPersonShooterController _myShooterController;

    private int _playersReady;
    private GameObject _playerObject;

    private void Start()
    {
        NotifyReadyToMasterClient();
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

            // Set local playerController and disable until game starts
            _myThirdPersonController = _playerObject.GetComponent<ThirdPersonController>();
            _myShooterController = _playerObject.GetComponent<ThirdPersonShooterController>();

            var controller = _playerObject.GetComponent<ThirdPersonShooterController>();
            cameraFollowTarget = controller.cameraFollowTarget;
            controller.aimVirtualCamera = cinemachineCamAim;

            cinemachineCamMain.Follow = cameraFollowTarget.transform;
            cinemachineCamAim.Follow = cameraFollowTarget.transform;
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
        if (_playerObject != null && _playerObject.GetPhotonView().IsMine)
        {
            var controller = _playerObject.GetComponent<ThirdPersonShooterController>();
            cameraFollowTarget = controller.cameraFollowTarget;
            controller.aimVirtualCamera = cinemachineCamAim;

            cinemachineCamMain.Follow = cameraFollowTarget.transform;
            cinemachineCamAim.Follow = cameraFollowTarget.transform;
        }
    }

    #endregion

    #endregion
}