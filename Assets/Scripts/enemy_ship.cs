using UnityEngine;
using System.Collections;

public class Detection : MonoBehaviour
{
    // Player and Ship references
    [SerializeField] private Rigidbody player;
    [SerializeField] private Rigidbody shipRB;

    [Header("Enemy Settings")]
    [Tooltip("The health of the ship")]
    [SerializeField] private int health = 100;
    [Tooltip("The speed at which the ship rotates to face the player")]
    [SerializeField] private float rotationSpeed = 5f;
    [Tooltip("The maximum speed at which the ship can move")]   
    [SerializeField] private float maxSpeed = 5f;
    [Tooltip("The speed at which the ship moves when cruising")]
    [SerializeField] private float cruisingSpeed = 2f;
    [Tooltip("The radius within which the ship will start chasing the player")]
    [SerializeField] private float distanceRadius = 10f;
    [Tooltip("The amount of ammo the ship has")]
    [SerializeField] private int ammoCount = 5;
    [Tooltip("The distance at which the ship will start firing at the player")]
    [SerializeField] private float sideFireDistance = 7f;
    [Tooltip("The cooldown between each shot")]
    [SerializeField] private float firingCooldown = 2f;
    [Tooltip("The size of the circle it will make")]
    [SerializeField] private float wiggleMagnitude = 0.3f;
    [SerializeField] private GameObject cannonballPrefab;
    [SerializeField] private Transform cannonPoint;
    [SerializeField] private Transform cannonPoint2;
    [SerializeField] private ParticleSystem shotEffect1;
    [SerializeField] private ParticleSystem shotEffect2;

    private float firingTimer = 0f;
    // ocillation time
    private float wiggleTime = 0f;


 private enum ShipState { Idle, Cruising, Chasing, Firing, Ramming }
    private ShipState currentState;

    //how long the ship has been idle
    private float idleTimer = 0f;
    //how long the ship will stay idle before cruising
    private float idleDuration = 3f; 
    private Vector3 cruisingDirection; 

    public AudioSource cannonshoot;

    private void Start()
    {
        currentState = ShipState.Idle;
        cruisingDirection = transform.forward; 
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= distanceRadius)
        {
            if (distanceToPlayer <= sideFireDistance && ammoCount > 0)
                currentState = ShipState.Firing; 
            else if (ammoCount <= 0)
                currentState = ShipState.Ramming; 
            else
                currentState = ShipState.Chasing; 
        }
        else
        {
            if (currentState != ShipState.Idle && currentState != ShipState.Cruising)
            {
                currentState = ShipState.Idle;
                idleTimer = 0f;
            }
        }

        switch (currentState)
        {
            case ShipState.Idle:
                IdleBehavior();
                break;
            case ShipState.Cruising:
                CruisingBehavior();
                break;
            case ShipState.Chasing:
                ChaseBehavior();
                break;
            case ShipState.Firing:
                FiringBehavior();
                break;
            case ShipState.Ramming:
                RammingBehavior();
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("friendlycannonball"))
        {
            Debug.Log("Friendly cannonball hit enemy ship!");
            health -= 25;
            Debug.Log($"Health: {health}");
            if (health <= 0)
            {
                // OR SINKING ANIMATION!!!!!!!!!!!!!!!!!!!!!!!!! AAAAAAAAAAAAAAAAA
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Land"))
        {
            StopAndTurnAround();
        }
    }

   private void IdleBehavior()
    {
        idleTimer += Time.deltaTime;
        shipRB.velocity = Vector3.zero; 
        if (idleTimer >= idleDuration)
        {
            currentState = ShipState.Cruising;
            cruisingDirection = transform.forward; 
        }
    }

    
    private void CruisingBehavior()
    {
        wiggleTime += Time.deltaTime;

        Vector3 wiggleOffset = new Vector3(
            Mathf.Sin(wiggleTime * 2f),
            0,
            Mathf.Cos(wiggleTime * 2f)
        ) * wiggleMagnitude;

        cruisingDirection = (transform.forward + wiggleOffset).normalized;
        shipRB.velocity = cruisingDirection * cruisingSpeed;

        Quaternion targetRotation = Quaternion.LookRotation(cruisingDirection);
        shipRB.rotation = Quaternion.Slerp(shipRB.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void ChaseBehavior()
    {
        RotateToPlayer();
        MoveTowardsPlayer();
    }

    private void FiringBehavior()
    {
        Vector3 playerDirection = (player.position - transform.position).normalized;
        // get the direction perpendicular to the player direction
        Vector3 sideDirection = Vector3.Cross(Vector3.up, playerDirection).normalized;

        // target the side direction
        Quaternion targetRotation = Quaternion.LookRotation(sideDirection);
        // smoothly rotate the ship to the side direction
        shipRB.rotation = Quaternion.Slerp(shipRB.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        shipRB.velocity = Vector3.zero;

        firingTimer += Time.deltaTime;
        if (firingTimer >= firingCooldown && ammoCount > 0)
        {
            FireAtPlayer();
            firingTimer = 0f;
            cannonshoot.Play();
        }
    }

    private void RammingBehavior()
    {
        RotateToPlayer();
        MoveTowardsPlayer();
    }

    private void RotateToPlayer()
    {
        Vector3 playerDirection = (player.position - transform.position).normalized;

        // rotate the ship to face the player
        Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
        shipRB.rotation = Quaternion.Slerp(shipRB.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void MoveTowardsPlayer()
    {
        shipRB.velocity = transform.forward * maxSpeed;
    }

    private void FireAtPlayer()
    {
        // Debug.Log("Firing at player!");
        ammoCount--;

        shotEffect1.Play();
        shotEffect2.Play();
        FireCannon();   

        if (ammoCount <= 0)
        {
            currentState = ShipState.Ramming;
        }
    }

    private void FireCannon()
    {
        GameObject cannonball = Instantiate(cannonballPrefab, cannonPoint.position, cannonPoint.rotation);
        GameObject cannonball2 = Instantiate(cannonballPrefab, cannonPoint2.position, cannonPoint2.rotation);
        Cannonball cb = cannonball.GetComponent<Cannonball>();
        Cannonball cb2 = cannonball2.GetComponent<Cannonball>();
        if (cb != null && cb2 != null)
        {
            cb.Launch(cannonPoint.forward);
            cb2.Launch(cannonPoint2.forward);
        }
    }

    private void StopAndTurnAround()    
    {
        shipRB.velocity = Vector3.zero;
        Vector3 turnaroundDirection = -transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(turnaroundDirection);
        StartCoroutine(TurnAndCruise(targetRotation));
    }

    private IEnumerator TurnAndCruise(Quaternion targetRotation)
    {
        float rotationProgress = 0f;
        while (rotationProgress < 1f)
        {
            shipRB.rotation = Quaternion.Slerp(shipRB.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            rotationProgress += rotationSpeed * Time.deltaTime;
            yield return null;
        }
        currentState = ShipState.Cruising;
        cruisingDirection = transform.forward;
    }
}