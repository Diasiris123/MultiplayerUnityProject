using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace CharacterSelection
{
    public class PlayerColorSetter : MonoBehaviourPunCallbacks, IPunObservable
    {
        public Renderer playerRenderer; 
        private Color playerColor;

        private void Start()
        {
            ApplyColor(GetPlayerColor());
            
        }
        

        public Color GetPlayerColor()
        {
            if (photonView.Owner.CustomProperties.TryGetValue("PlayerColor", out object colorObject))
            {
                string colorJson = (string)colorObject;
                playerColor = JsonUtility.FromJson<Color>(colorJson);
                return playerColor;
            }
            return Color.black;
        }

        private void ApplyColor(Color color)
        {
            if (playerRenderer != null)
            {
                foreach (Material material in playerRenderer.materials)
                {
                    material.color = color;
                }
                
            }
        }

        private void ApplyPlayerColor()
        {
            if (photonView.Owner.CustomProperties.TryGetValue("PlayerColor", out object colorObject))
            {
                string colorJson = (string)colorObject;
                Color playerColor = JsonUtility.FromJson<Color>(colorJson);
                ApplyColor(playerColor);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            if (targetPlayer == photonView.Owner && changedProps.ContainsKey("PlayerColor"))
            {
                ApplyPlayerColor();
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(playerColor.r);
                stream.SendNext(playerColor.g);
                stream.SendNext(playerColor.b);
            }
            else
            {
                float r = (float)stream.ReceiveNext();
                float g = (float)stream.ReceiveNext();
                float b = (float)stream.ReceiveNext();
                playerColor = new Color(r, g, b);
                ApplyColor(playerColor); 
            }
        }
    }
}