using UnityEngine;
using System.Collections;

public class LandPatrol : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Patrol Settings")]
    [Tooltip("The speed at which the entity moves while patrolling.")]
    [SerializeField] private float patrolSpeed = 2f;
    [Tooltip("The range within which the entity will switch to chasing.")]
    [SerializeField] private float detectionRadius = 10f;
    [Tooltip("The waypoints the entity will patrol.")]
    [SerializeField] private Transform[] waypoints;

    [Header("Chase Settings")]
    [Tooltip("The speed at which the entity moves while chasing.")]
    [SerializeField] private float chaseSpeed = 4f;

    [Header("Collision Settings")]
    [Tooltip("The layer mask for water detection.")]
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private Animator animator;

    private PlayerStats _playerstats;

    private int currentWaypointIndex = 0;
    private float idleTimer = 0f;
    private float idleDuration = 2f;

    private enum PatrolState { Cruising, Chasing }
    private PatrolState currentState;

    private void Start()
    {
        currentState = PatrolState.Cruising;
        _playerstats = PlayerStats.Instance;
    }


    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            currentState = PatrolState.Chasing;
            animator.speed = chaseSpeed;
        }
        else if (currentState == PatrolState.Chasing && distanceToPlayer > detectionRadius)
        {
            currentState = PatrolState.Cruising;
            animator.speed = patrolSpeed;
        }

        switch (currentState)
        {
            case PatrolState.Cruising:
                CruisingBehavior();
                break;
            case PatrolState.Chasing:
                ChasingBehavior();
                break;
        }
    }


    private void IdleBehavior()
    {
        // animator.SetBool("isCruise", false);
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            idleTimer = 0f;
            currentState = PatrolState.Cruising;
        }
    }

    private void CruisingBehavior()
    {
        if (waypoints.Length == 0) return;
        // animator.SetBool("isCruise", true);
        Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        transform.position += direction * patrolSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * patrolSpeed);

        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void ChasingBehavior()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chaseSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerstats.coinloss(20);
            Destroy(this.gameObject);
        }
    }
    private void CancelChaseAndTurnAround()
    {
        currentState = PatrolState.Cruising;
        transform.rotation = Quaternion.LookRotation(-transform.forward);
        if (waypoints.Length > 0)
            currentWaypointIndex = (currentWaypointIndex + waypoints.Length - 1) % waypoints.Length;
    }
}
