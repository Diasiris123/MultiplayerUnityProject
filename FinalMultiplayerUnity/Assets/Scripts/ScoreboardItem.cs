using UnityEngine;
using TMPro;
using Photon.Realtime;

public class ScoreboardItem : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text statusText;
    
    public void Initialize(Player player)
    {
        usernameText.text = player.NickName;
        
        if (player.CustomProperties.ContainsKey("EliminationOrder"))
        {
            int eliminationOrder = (int)player.CustomProperties["EliminationOrder"];
            statusText.text = $"Eliminated (#{eliminationOrder})";
        }
        else
        {
            statusText.text = "Alive";
        }
    }
}