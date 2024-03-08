using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Unity.VisualScripting;
using static EnemyAI;

public class EnemyAI : MonoBehaviour
{
    //Enemy State
    public enum enemyStates { Patrol, Chase, Retreat, Search, Attack }
    public enemyStates enemyCurrentState;
    public NavMeshAgent agent;
    Renderer enemyStateColor;
    public TextMeshProUGUI currentenemyState;

    [Header("Target Player")]
    public Transform player;
    private Transform target;

    //Patrol State for enemy
    [Header("Patrol State")]
    public Transform[] patrolPoints;
    private int countPatrol;
    private Vector3 lastPartolPoint = Vector3.zero;
    private bool searchingPlayer;
    [Header("Wait time for Patrol")]
    [SerializeField] private float patrolTime = 2f;
    private float waitTime = 0f;
    private bool waitTimeOn = false;

    [Header("Time")]
    [SerializeField] private float searchTime = 5.0f;
    [SerializeField] private float retreatTime = 5.0f;
    private float retreatOn;

    [Header("Distance Floats")]
    [SerializeField] private float attackRadius = 5.0f;
    [SerializeField] private float chaseRadius = 8.0f;
    private float distanceToPoint;



    // Start is called before the first frame update
    void Start()
    {
        enemyCurrentState = enemyStates.Patrol;
        countPatrol = 0;
        target = patrolPoints[countPatrol];

        Vector3 distance = gameObject.transform.position - target.transform.position;
        enemyStateColor = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        StateChange();
        switch (enemyCurrentState)
        {
            case enemyStates.Patrol: PatrolState(); break;
            case enemyStates.Chase: TargetsPlayer(); break;
            case enemyStates.Attack: AttacksPlayer(); break;
            case enemyStates.Search: Searching(); break;
            case enemyStates.Retreat: EnemyRetreat(); break;
        }

        currentenemyState.text = "Enemy State: " + enemyCurrentState.ToString();
    }

    public void StateChange()
    {
        if (Vector3.Distance(transform.position, player.position) <= chaseRadius)
        {
            enemyCurrentState = enemyStates.Chase;
            if (Vector3.Distance(transform.position, player.position) > chaseRadius)
            {
                enemyCurrentState = enemyStates.Search; // Searches for the Player if found
                Debug.Log(currentenemyState);
            }
        }
        if (Vector3.Distance(transform.position, player.position) <= attackRadius)
        {
            enemyCurrentState = enemyStates.Attack; // Enemy attacks the Player
            Debug.Log(currentenemyState);
        }
    }

    public void EnemyRetreat()
    {
        enemyStateColor.material.color = Color.black;
        agent.SetDestination(target.position);
        distanceToPoint = Vector3.Distance(transform.position, target.position);
        if (distanceToPoint <= 3f)
        {
            agent.SetDestination(transform.position);
            //Debug.Log("Position" + distanceToPoint);
            enemyCurrentState = enemyStates.Patrol;
        }
    }

    public void PatrolState()
    {
        if (!waitTimeOn)
        {
            enemyStateColor.material.color = Color.green;
            agent.SetDestination(target.position);
            distanceToPoint = Vector3.Distance(transform.position, target.position);

            if (distanceToPoint <= 3f)
            {
                waitTimeOn = true;
                waitTime = patrolTime;
                Debug.Log("Waiting");
            }
        }
        else
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                countPatrol++;
                if (countPatrol == patrolPoints.Length)
                {
                    countPatrol = 0;
                }
                target = patrolPoints[countPatrol];

                waitTimeOn = false;
                agent.SetDestination(target.position);
                Debug.Log(target.position);
            }
        }
    }

    public void TargetsPlayer()
    {
        enemyStateColor.material.color = Color.red;
        agent.SetDestination(player.position);
        if (Vector3.Distance(transform.position, player.position) > chaseRadius)
        {
            enemyCurrentState = enemyStates.Search;
            Debug.Log(chaseRadius);
        }
    }

    public void AttacksPlayer()
    {
        enemyStateColor.material.color = Color.yellow;
        agent.SetDestination(transform.position);
        if (Vector3.Distance(transform.position, player.position) > attackRadius)
        {
            enemyCurrentState = enemyStates.Chase;
        }
    }

    public void Searching()
    {
        enemyStateColor.material.color = Color.blue;
        if (!searchingPlayer)
        {
            lastPartolPoint = player.position;
            searchingPlayer = true;
        }

        float RadiusToPlayer = Vector3.Distance(transform.position, lastPartolPoint);
        if (RadiusToPlayer > 0.1f)
        {
            agent.SetDestination(lastPartolPoint);
        }

        if (enemyCurrentState != enemyStates.Search)
        {
            searchTime = 10f; // Search Time 
        }
        searchTime -= Time.deltaTime;

        if (searchTime <= 0)
        {
            enemyCurrentState = enemyStates.Retreat; // Goes to Retreats 
            target = patrolPoints[countPatrol];
            retreatOn = retreatTime;
            searchingPlayer = false;
            searchTime = 10f;
            Debug.Log("Retreat" + retreatOn);
        }

        if (enemyCurrentState == enemyStates.Retreat) // If Retreats
        {
            EnemyRetreat();
            retreatOn -= Time.deltaTime;
            if (retreatOn <= 0)
            {
                enemyCurrentState = enemyStates.Patrol; // Goes to Partrol
                countPatrol++; 
                if (countPatrol >= patrolPoints.Length)
                {
                    countPatrol = 0; 
                }
                target = patrolPoints[countPatrol];
            }
        }
    }
}

