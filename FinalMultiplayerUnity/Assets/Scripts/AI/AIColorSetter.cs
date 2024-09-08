using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace CharacterSelection
{
    public class AIColorSetter : MonoBehaviourPunCallbacks, IPunObservable
    {
        public Renderer playerRenderer; 
        private Color playerColor;

        public void SetColor(Color color)
        {
            ApplyColor(color);
        }


        public Color GetPlayerColor(Player player)
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

        private void ApplyColor(Color color)
            {
                playerColor = color;
                if (playerRenderer != null)
                {
                    foreach (Material material in playerRenderer.materials)
                    {
                        material.color = color;
                    }
                
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