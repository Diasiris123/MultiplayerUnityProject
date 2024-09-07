using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;


public class PhotonViewManager : MonoBehaviourPunCallbacks
{
    private static PhotonViewManager _instance;
    private Dictionary<int, PhotonView> playerPhotonViews = new Dictionary<int, PhotonView>();

    public static PhotonViewManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    [PunRPC]
    public void RegisterPlayerPhotonView(int playerId, int photonViewId)
    {
        PhotonView pv = PhotonView.Find(photonViewId);
        if (pv != null)
        {
            playerPhotonViews[playerId] = pv;
        }
    }

    public void RegisterPhotonView(int OwnerActorID, int ViewID)
    {
        photonView.RPC("RegisterPlayerPhotonView", RpcTarget.MasterClient, OwnerActorID, ViewID);
    }
    public PhotonView GetPlayerPhotonView(int playerId)
    {
        if (playerPhotonViews.TryGetValue(playerId, out PhotonView pv))
        {
            return pv;
        }
        return null;
    }

    public Transform GetPlayerTransform(int playerId)
    {
        PhotonView pv = GetPlayerPhotonView(playerId);
        if (pv != null)
        {
            return pv.transform;
        }
        return null;
    }
}

