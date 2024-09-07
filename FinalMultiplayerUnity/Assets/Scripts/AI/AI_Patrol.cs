using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class AI_Patrol : MonoBehaviourPunCallbacks
{
    public NavMeshAgent agent;
    public float range;

    public Transform centrePoint;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance) 
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point)) 
            {
                agent.SetDestination(point);
            }
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) 
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}
