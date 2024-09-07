using UnityEngine;
using Photon.Pun;
using UnityEngine;

public class PlayerSender : MonoBehaviourPunCallbacks
{
    private PhotonView playerPhotonView;

    private void Start()
    {
        playerPhotonView = GetComponent<PhotonView>();

        if (playerPhotonView != null && photonView.IsMine)
        {
            RegisterWithPhotonViewManager();
        }
    }

    private void RegisterWithPhotonViewManager()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonViewManager.Instance.RegisterPlayerPhotonView(photonView.OwnerActorNr, playerPhotonView.ViewID);
        }
        else
        {
            PhotonViewManager.Instance.RegisterPhotonView(photonView.OwnerActorNr, playerPhotonView.ViewID);
        }
    }
}
