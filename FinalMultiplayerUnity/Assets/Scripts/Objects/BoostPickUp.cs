using Photon.Pun;
using UnityEngine;

public class Boost : MonoBehaviour
{
    public BoostSpawnPoint spawner;

    public void Collect()
    {
        spawner.FreeSpawn();
        PhotonNetwork.Destroy(gameObject);
    }
}
