using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;


public class HealthManager : MonoBehaviourPunCallbacks
{
    private static HealthManager _instance;
    private Dictionary<int, float> playerHPs = new Dictionary<int, float>();

    public static HealthManager Instance
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
    public void RegisterPlayerHealth(int playerId, float playerHP)
    {
        playerHPs[playerId] = playerHP;
    }

    public void RegisterPhotonView(int OwnerActorID, float HP)
    {
        photonView.RPC("RegisterPlayerHealth", RpcTarget.MasterClient, OwnerActorID, HP);
    }

    public float GetPlayerHP(int playerId)
    {
        if (playerHPs.TryGetValue(playerId, out float hp))
        {
            return hp;
        }
        return 0;
    }
}

