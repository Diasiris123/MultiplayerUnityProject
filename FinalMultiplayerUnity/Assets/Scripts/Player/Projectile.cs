using System;
using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviourPunCallbacks
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed = 10f;
    public float damage = 1f;

    private void Start()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().ApplyDamage(damage);
        }
        
        Destroy(gameObject);
    }
}
