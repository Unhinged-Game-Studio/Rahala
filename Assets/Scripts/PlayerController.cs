using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _rotationSpeed;
    [SerializeField] float _moveSpeed;
    [SerializeField] GameObject _speedParticles;
    [SerializeField] Transform _camera;

    [SerializeField] GameObject cannonBallPrefab;
    [SerializeField] Transform pointRight1;
    [SerializeField] Transform pointRight2;
    [SerializeField] Transform pointLeft1;
    [SerializeField] Transform pointLeft2;
    [SerializeField] float _firingCooldown;

    [SerializeField] Transform rayOrigin;
    [SerializeField] float rayDistance = 10f;
    [SerializeField] string targetTag = "Ground";
    [SerializeField] GameObject promptUI;
    [SerializeField] GameController gameController;


    private float _originalSpeed;
    private direction _playerDirection;
    private float _targetSpeed = 0.5f;
    private float _duration = 1f;
    private float _elapsedTime = 0f;
    private bool _isActive = false;
    private Rigidbody _playerRb;
    private PlayerStats _playerStats;
    private UIManager _uiManager;

    public AudioSource shootsound;
    private struct direction 
    { 
        public bool Forward; 
        public bool Backward;
        public bool Right;
        public bool Left;
        public bool Sprint;

        public direction(bool forward, bool backward, bool right, bool left, bool sprint)
        {
            Forward = forward;
            Backward = backward;
            Right = right;
            Left = left;
            Sprint = sprint;
        }
    };
    private float _leftFiringTime = 0f;
    private float _rightFiringTime = 0f;

    void Start()
    {
        _playerDirection = new direction(false, false, false, false, false);
        _playerRb = GetComponent<Rigidbody>();
        _originalSpeed = _moveSpeed;
        _playerStats = PlayerStats.Instance;
        _uiManager = UIManager.Instance;
        _playerRb.maxAngularVelocity = 1f;
    }

    void Update()
    {
        _playerDirection.Forward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        _playerDirection.Backward = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        _playerDirection.Left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        _playerDirection.Right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        if (Input.GetKeyDown(KeyCode.LeftShift))
            _playerDirection.Sprint = !_playerDirection.Sprint;
        if (_playerDirection.Right)
            turnRight();
        if (_playerDirection.Left)
             turnLeft();
        if (_moveSpeed == 0.5 && _isActive == false)
        {
            _speedParticles.SetActive(true);
            _isActive = true;
        }
        if (_isActive)
            _speedParticles.transform.rotation = _camera.transform.rotation;
        if (Input.GetKeyDown(KeyCode.Tab))
            _uiManager.showInstructions();
        DetectObjectInFront();

    }

     void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemyball"))
        {
            _playerStats.coinloss(10);
            Destroy(collision.gameObject);
        }
    }

    void FixedUpdate()
    {
        _rightFiringTime += Time.deltaTime;
        _leftFiringTime += Time.deltaTime;
        if (_playerDirection.Forward)
            goForward();
        if (_playerDirection.Backward)
            goReverse();
        if (_playerDirection.Sprint && _playerDirection.Forward && _playerStats.getStaminaMeter() > 0f)
            startedSprinting();
        else if (!_playerDirection.Sprint || _playerStats.getStaminaMeter() == 0f
                || !_playerDirection.Forward || _playerDirection.Backward)
            stoppedSprinting();
        if (Input.GetMouseButtonDown(1) && _rightFiringTime > _firingCooldown)
            FireCannonRight();
        if (Input.GetMouseButtonDown(0) && _leftFiringTime > _firingCooldown)
            FireCannonLeft();
        if (_playerRb.angularVelocity.y > 0f && !_playerDirection.Right)
            _playerRb.AddTorque(transform.up * -_rotationSpeed);
        if (_playerRb.angularVelocity.y < 0f && !_playerDirection.Left)
            _playerRb.AddTorque(transform.up * _rotationSpeed);
    }

    void DetectObjectInFront()
{
    Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
    RaycastHit hit;

    // Draw the ray in the Scene view for debugging
    Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

    if (Physics.Raycast(ray, out hit, rayDistance))
    {
        if (hit.collider.CompareTag(targetTag))
        {
            gameController.SwitchPlayer();
            // Show the prompt if the object has the target tag
            if (promptUI != null)
                promptUI.SetActive(true);
            return;
        }
    }

    // Hide the prompt if no valid object is detected
    if (promptUI != null)
        promptUI.SetActive(false);
}
    void goForward()
    {
        Vector3 movement = gameObject.transform.forward;
        _playerRb.MovePosition(_playerRb.position + movement * _moveSpeed);
    }

    void goReverse()
    {
        Vector3 movement = -gameObject.transform.forward;
        _playerRb.MovePosition(_playerRb.position + movement * _moveSpeed);
    }

    void turnRight()
    {
        if (_playerRb.angularVelocity.y < 0f)
            _playerRb.angularVelocity = new Vector3(0f, 0f, 0f);
        _playerRb.AddTorque(transform.up * _rotationSpeed);
    }

    void turnLeft()
    {
        if (_playerRb.angularVelocity.y > 0f)
            _playerRb.angularVelocity = new Vector3(0f, 0f, 0f);
        _playerRb.AddTorque(transform.up * -_rotationSpeed);
    }

    void startedSprinting()
    {
        _uiManager.updateStamina();
        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / _duration);
        _moveSpeed = Mathf.Lerp(0f, _targetSpeed, t);
        _playerStats.isSprinting();
    }

    void stoppedSprinting()
    {
        _elapsedTime = 0f;
        _speedParticles.SetActive(false);
        _isActive = false;
        _moveSpeed = _originalSpeed;
        _playerDirection.Sprint = false;
        _playerStats.isNotSprinting();
        if (_playerStats.getStaminaMeter() < 100)
            _uiManager.updateStamina();
    }

    private void FireCannonRight()
    {
        _rightFiringTime = 0f;
        GameObject cannonball1 = Instantiate(cannonBallPrefab, pointRight1.position, pointRight1.rotation);
        GameObject cannonball2 = Instantiate(cannonBallPrefab, pointRight2.position, pointRight2.rotation);
        Cannonball cb1 = cannonball1.GetComponent<Cannonball>();
        Cannonball cb2 = cannonball2.GetComponent<Cannonball>();

        if (cb1 != null && cb2 != null)
        {
            cb1.Launch(pointRight1.forward);
            cb2.Launch(pointRight2.forward);
        }
        shootsound.Play();
    }

    private void FireCannonLeft()
    {
        _leftFiringTime = 0f;
        GameObject cannonball1 = Instantiate(cannonBallPrefab, pointLeft1.position, pointLeft1.rotation);
        GameObject cannonball2 = Instantiate(cannonBallPrefab, pointLeft2.position, pointLeft2.rotation);
        Cannonball cb1 = cannonball1.GetComponent<Cannonball>();
        Cannonball cb2 = cannonball2.GetComponent<Cannonball>();

        if (cb1 != null && cb2 != null)
        {
            cb1.Launch(pointLeft1.forward);
            cb2.Launch(pointLeft2.forward);
        }
        shootsound.Play();
    }
}
