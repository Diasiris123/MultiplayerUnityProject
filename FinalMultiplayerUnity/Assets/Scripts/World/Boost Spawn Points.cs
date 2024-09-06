using System;
using UnityEngine;

public class BoostSpawnPoint : MonoBehaviour
{
    public int Id = 0;
        
    private bool _isTaken = false;
    public bool IsTaken => _isTaken;

    public void Take()
    {
        _isTaken = true;
    }

    public void FreeSpawn()
    {
        _isTaken = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position,1f); 
    }
}