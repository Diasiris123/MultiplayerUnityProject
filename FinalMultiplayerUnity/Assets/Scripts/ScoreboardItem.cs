using UnityEngine;
using TMPro;
using Photon.Realtime;

public class ScoreboardItem : MonoBehaviour
{
    public TMP_Text usernameText;

    public void Initialize(Player player)
    {
        usernameText.text = player.NickName;
    }
}