using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class AI_Patrol : MonoBehaviourPunCallbacks
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    public Animator animator;
    public NavMeshAgent agent;
    public float range;

    public Transform centrePoint;

    private float _animationBlend;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        _animationBlend = Mathf.Lerp(_animationBlend, 2f, Time.deltaTime * 10f);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        if (agent.velocity.sqrMagnitude > 0f)
        {
            animator.SetFloat(Speed,_animationBlend);
        }
        else
        {
            animator.SetFloat(Speed, 0f);
        }
        
        
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
