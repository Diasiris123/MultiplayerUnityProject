using System;
using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviourPunCallbacks
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed = 10f;
    [SerializeField] float lifetime = 50f;
    public float damage = 1f;

    private void Start()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.TryGetComponent<PlayerHealth>(out PlayerHealth ph))
                other.GetComponent<PlayerHealth>().ApplyDamage(damage);
            else
                other.GetComponent<AIHealth>().ApplyDamage(damage);
        }
        
        Destroy(gameObject);
    }
}
