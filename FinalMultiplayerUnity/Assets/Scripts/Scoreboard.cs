using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;
    [SerializeField] GameObject ScoreboardCanvas;
    
    Dictionary<Player, ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem>();

    void Start()
    {
        UpdateScoreboard();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }
    
    void UpdateScoreboard()
    {
        foreach (var item in scoreboardItems.Values)
        {
            Destroy(item.gameObject);
        }
        scoreboardItems.Clear();
        
        List<Player> sortedPlayers = new List<Player>(PhotonNetwork.PlayerList);
        sortedPlayers.Sort((p1, p2) =>
        {
            int p1Order = p1.CustomProperties.ContainsKey("EliminationOrder") ? (int)p1.CustomProperties["EliminationOrder"] : int.MaxValue;
            int p2Order = p2.CustomProperties.ContainsKey("EliminationOrder") ? (int)p2.CustomProperties["EliminationOrder"] : int.MaxValue;
            return p1Order.CompareTo(p2Order);
        });

        foreach (Player player in sortedPlayers)
        {
            AddScoreboardItem(player);
        }
    }
        
    void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
        item.Initialize(player);
        scoreboardItems[player] = item;
    }

    void RemoveScoreboardItem(Player player)
    {
        if (scoreboardItems.ContainsKey(player))
        {
            Destroy(scoreboardItems[player].gameObject);
            scoreboardItems.Remove(player);
        }
    }
}