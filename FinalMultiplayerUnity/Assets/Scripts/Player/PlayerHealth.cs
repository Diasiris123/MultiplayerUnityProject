using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    private const string TAKE_DAMAGE_RPC = nameof(TakeDamage);
    
    [SerializeField] private Slider healthBar;

    private float maxHealth = 20f;
    private float currentHealth;
    private Transform playerCamera;
    private int eliminatedPlayers= 0;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        playerCamera = Camera.main.transform;
    }

    private void Update()
    {
        UpdateHealthBar();
    }



    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
            
            // face local player camera
            healthBar.transform.LookAt(healthBar.transform.position + playerCamera.rotation * Vector3.forward, playerCamera.rotation * Vector3.up);
        }
    }


    public void ApplyDamage(float damage)
    {
        photonView.RPC(TAKE_DAMAGE_RPC,RpcTarget.All, damage);
    }
    
    
    [PunRPC]
    private void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            KillPLayer();
        }
            
    }

    private void KillPLayer()
    {
        eliminatedPlayers++;
        Debug.Log(eliminatedPlayers + " eliminated");
        isDead = true;
        PhotonNetwork.Destroy(gameObject);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}