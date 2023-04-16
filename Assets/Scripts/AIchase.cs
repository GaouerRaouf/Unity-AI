using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
public class AIchase : MonoBehaviour
{
    public enum AISTATE { PATROL=0, CHASE=1, ATTACK=2};
    private NavMeshAgent ThisAgent = null;
    private Transform playerTransform = null;
    public AISTATE CurrentState
    {
        get { return _CurrentState; }
        set
        {
            StopAllCoroutines();
            _CurrentState = value;
            switch (CurrentState)
            {
                case AISTATE.PATROL:
                    StartCoroutine(StatePatrol());
                    break;

                case AISTATE.CHASE:
                    StartCoroutine(StateChase());
                    break;

                case AISTATE.ATTACK:
                    StartCoroutine(StateAttack());
                    break;
            }
        }
    }
    private AISTATE _CurrentState = AISTATE.PATROL;
    private void Awake()
    {
        ThisAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    private void Start()
    {
        CurrentState = AISTATE.PATROL;
    }


    public IEnumerator StateAttack()
    {
        float AttackDistance = 2.0f;

        while(CurrentState == AISTATE.ATTACK)
        {
            if (Vector3.Distance(transform.position, playerTransform.transform.position)>AttackDistance)
            {
                CurrentState = AISTATE.CHASE;
                yield break;
            }
                Debug.Log("attack");
                ThisAgent.SetDestination(playerTransform.transform.position);
            yield return null;
        }
    }

    public IEnumerator StateChase()
    {
        float attackDistance = 2f;

        while (CurrentState == AISTATE.CHASE)
        {
            if (Vector3.Distance(transform.position, playerTransform.transform.position) < attackDistance)
            {
                CurrentState = AISTATE.ATTACK;
                yield break;
            }
                Debug.Log("chase");
            ThisAgent.SetDestination(playerTransform.transform.position);
            yield return null;
        }
    }

    public IEnumerator StatePatrol()
    {
        GameObject[] Waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        GameObject CurrentWaypoint = Waypoints[Random.Range(0, Waypoints.Length)];
        float TargetDistance = 2.0f;

        while (CurrentState==AISTATE.PATROL)
        {
            ThisAgent.SetDestination(CurrentWaypoint.transform.position);
            if (Vector3.Distance(transform.position,CurrentWaypoint.transform.position)<TargetDistance)
            {
                CurrentWaypoint = Waypoints[Random.Range(0, Waypoints.Length)];
            }


        yield return null;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        CurrentState = AISTATE.CHASE;
    }

}
