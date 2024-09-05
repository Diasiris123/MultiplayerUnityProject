using System;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public int Id = 0;
        
    private bool _isTaken = false;
    public bool IsTaken => _isTaken;

    public void Take()
    {
        _isTaken = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position,1f); 
    }
}