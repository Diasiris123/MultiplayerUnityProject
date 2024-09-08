using Photon.Pun;
using UnityEngine;

public class Boost : MonoBehaviourPun
{
    private const string DESTROY_RPC = nameof(NetworkDestroy);
    public BoostSpawnPoint spawner;

    public void Collect()
    {
        if(spawner != null && PhotonNetwork.IsMasterClient)
            spawner.FreeSpawn();
        
        photonView.RPC(DESTROY_RPC, RpcTarget.All);
    }

    [PunRPC]
    private void NetworkDestroy()
    {
        Destroy(gameObject);
    }
}
