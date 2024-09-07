using Photon.Pun;
using UnityEngine;

namespace CharacterSelection
{
    public class ProjectileColorSetter : MonoBehaviourPun, IPunObservable
    {
        public Renderer projectileRenderer; 
        private Color projectileColor = Color.black; 
        private void Start()
        {
            if (photonView.IsMine)
            {
                SetProjectileColor(); 
            }
        }

        private void SetProjectileColor()
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PlayerColor", out object colorObject))
            {
                string colorJson = (string)colorObject;
                projectileColor = JsonUtility.FromJson<Color>(colorJson);
                ApplyColor(projectileColor);
            }
        }

        private void ApplyColor(Color color)
        {
            if (projectileRenderer != null)
            {
                projectileRenderer.material.color = color;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(projectileColor.r);
                stream.SendNext(projectileColor.g);
                stream.SendNext(projectileColor.b);
            }
            else
            {
                float r = (float)stream.ReceiveNext();
                float g = (float)stream.ReceiveNext();
                float b = (float)stream.ReceiveNext();
                projectileColor = new Color(r, g, b);
                ApplyColor(projectileColor); 
            }
        }
    }
}