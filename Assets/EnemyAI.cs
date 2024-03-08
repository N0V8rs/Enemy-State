using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    //Enemy State
    public enum enemyStates { patrol, chase, retreat, search, attack}
    public enemyStates enemyCurrentState;
    public NavMeshAgent agent;
    Renderer enemyStateColor;
    public TextMeshProUGUI currentenemyState;

    [Header("Target Player")]
    public Transform player;
    public Transform target;

    //Patrol State for enemy
    [Header("Patrol State")]
    public Transform[] patrolPoints;
    private int countPatrol;
    private Vector3 lastPartolPoint;
    private bool searchingPlayer;
    [Header("Wait time for Patrol")]
    [SerializeField] private float patrolTime = 1.0f;
    private float waitTime = 0f;
    private bool patrolwaitTime = false;

    [Header("Search Time")]
    [SerializeField] private float searchTime = 5.0f;

    [Header("Distance Floats")]
    [SerializeField] private float attackRadius = 5.0f;
    [SerializeField] private float chaseRadius = 8.0f;
    private float distanceToPoint;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StateChange()
    {
        if (Vector3.Distance(transform.position, player.position) <= chaseRadius) 
        {
            enemyCurrentState = enemyStates.chase;
            if (Vector3.Distance(transform.position, player.position) > chaseRadius)
            {
                enemyCurrentState = enemyStates.search; // Searches for the Player if found
            }
        }
        if (Vector3.Distance(transform.position, player.position) <= attackRadius)
        {
            enemyCurrentState= enemyStates.attack; // Enemy attacks the Player
        }
    }

    public void EnemyRetreat()
    {
        enemyStateColor.material.color = Color.black;
        agent.SetDestination(target.position);
        distanceToPoint = Vector3.Distance(transform.position, target.position);
        if (distanceToPoint <= 2f)
        {
            agent.SetDestination(transform.position);
            Debug.Log("Position" +  distanceToPoint);
        }
    }
}
