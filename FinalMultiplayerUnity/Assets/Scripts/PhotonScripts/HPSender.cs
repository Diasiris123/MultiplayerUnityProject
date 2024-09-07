using UnityEngine;
using Photon.Pun;
using UnityEngine;

public class HPSender : MonoBehaviourPunCallbacks
{
    private float playersHP;

    private void Start()
    {
        playersHP = GetComponent<PlayerHealth>().currentHealth;

        if (photonView.IsMine)
        {
            RegisterWithHealthManager();
        }
    }

    private void RegisterWithHealthManager()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            HealthManager.Instance.RegisterPlayerHealth(photonView.OwnerActorNr, playersHP);
        }
        else
        {
            HealthManager.Instance.RegisterPhotonView(photonView.OwnerActorNr, playersHP);
        }
    }
}
