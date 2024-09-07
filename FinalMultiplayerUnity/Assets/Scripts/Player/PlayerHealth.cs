using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    private const string TAKE_DAMAGE_RPC = nameof(TakeDamage);
    private const string ELIMINATE_RPC = nameof(NotifyPlayersOfElimination);
    
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider screenHealthBar;

    private Slider ChosenSlider;

    private float maxHealth = 20f;
    private float currentHealth;
    private Transform playerCamera;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        playerCamera = Camera.main.transform;

        if (photonView.AmOwner)
        {
            healthBar.gameObject.SetActive(false);
            ChosenSlider = screenHealthBar;
        }
        else
        {
            screenHealthBar.gameObject.SetActive(false);
            ChosenSlider = healthBar;
        }
    }

    private void Update()
    {
        UpdateHealthBar();
    }


    
    private void UpdateHealthBar()
    {
        ChosenSlider.value = currentHealth / maxHealth;
        
        if (healthBar != null )
        {
            // face local player camera
            healthBar.transform.LookAt(healthBar.transform.position + playerCamera.rotation * Vector3.forward, playerCamera.rotation * Vector3.up);
        }
        
        
        
        
    }


    public void ApplyDamage(float damage)
    {
        photonView.RPC(TAKE_DAMAGE_RPC,RpcTarget.All, damage,photonView.Owner.NickName);
    }
    
    
    [PunRPC]
    private void TakeDamage(float amount,string playerName)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            HandlePLayerDeath(playerName);
        }
            
    }

    private void HandlePLayerDeath(string playerName)
    {
        if(playerName != PhotonNetwork.LocalPlayer.NickName)
            isDead = true;
        
        if (photonView.IsMine)
        {
            photonView.RPC(ELIMINATE_RPC, RpcTarget.All);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    private void NotifyPlayersOfElimination()
    {
        Debug.Log($"Player {photonView.Owner.NickName} has been eliminated.");

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("EliminatedPlayersCount"))
        {
            int eliminatedPlayers = (int)PhotonNetwork.CurrentRoom.CustomProperties["EliminatedPlayersCount"];
            eliminatedPlayers++;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "EliminatedPlayersCount", eliminatedPlayers } });
            Debug.Log($"Eliminated Players: {eliminatedPlayers}");

            CheckForWinner(eliminatedPlayers); 
        }
        else
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "EliminatedPlayersCount", 1 } });
            Debug.Log("Eliminated Players: 1");

            CheckForWinner(1); 
        }
    }
        
    
    
    private void CheckForWinner(int eliminatedPlayers)
    {
        int totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        if (totalPlayers - eliminatedPlayers == 1)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!player.CustomProperties.ContainsKey("IsEliminated") || !(bool)player.CustomProperties["IsEliminated"])
                {
                    string winnerName = player.NickName;
                    AnnounceWinner(winnerName);
                    break;
                }
            }
        }
    }
    
    [PunRPC]
    private void AnnounceWinner(string winnerName)
    {
        Debug.Log($"Winner: Player {winnerName} has won the game!");
    }
    

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}