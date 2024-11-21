using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onlandcontroller : MonoBehaviour
{
   [SerializeField] float _rotationSpeed;
    // [SerializeField] float _tiltAngle;
    // [SerializeField] float _tiltSpeed;
    [SerializeField] float _moveSpeed;
    [SerializeField] GameObject _speedParticles;
    [SerializeField] Transform _camera;

     [SerializeField] Transform rayOrigin;
    [SerializeField] float rayDistance = 10f;
    [SerializeField] string targetTag = "Boundry";
    [SerializeField] GameObject promptUI;

    public UIManager _uiManager;
    public Animator camelanimator;
    public GameController gameController;
    private float _originalSpeed;
    public ParticleSystem dust;
    private direction _playerDirection;
    private float _targetSpeed = 0.5f;
    private float _duration = 1f;
    private float _elapsedTime = 0f;
    private bool _isActive = false;
    private Rigidbody _playerRb;
    private float _currentAngle;
    private PlayerStats _playerStats;
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

    void Start()
    {
        _playerDirection = new direction(false, false, false, false, false);
        _playerRb = GetComponent<Rigidbody>();
        _currentAngle = gameObject.transform.rotation.y;
        _originalSpeed = _moveSpeed;
        _playerStats = PlayerStats.Instance;
        _uiManager = UIManager.Instance;
    }

    void Update()
    {        
        _playerDirection.Forward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        _playerDirection.Backward = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        _playerDirection.Left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        _playerDirection.Right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        camelanimator.SetBool("Moving?", _playerDirection.Forward || _playerDirection.Backward);
        if (Input.GetKeyDown(KeyCode.LeftShift))
            _playerDirection.Sprint = !_playerDirection.Sprint;
        if (_moveSpeed == 0.5 && _isActive == false)
        {
            _speedParticles.SetActive(true);
            _isActive = true;
        }
        if (_isActive)
            _speedParticles.transform.rotation = _camera.transform.rotation;
        if(_moveSpeed > 0)
            dust.Play();
        else if (_moveSpeed <= 0)
            dust.Stop();
        DetectObjectInFront();

    }

    void FixedUpdate()
    {
        if(gameController.inputEnabled)
        {
            if (_playerDirection.Forward)
                goForward();
            if (_playerDirection.Backward)
                goReverse();
            if (_playerDirection.Right)
                turnRight();
            if (_playerDirection.Left)
                turnLeft();
            if (_playerDirection.Sprint && _playerDirection.Forward && _playerStats.getStaminaMeter() > 0f)
                startedSprinting();
            else if (!_playerDirection.Sprint || _playerStats.getStaminaMeter() == 0f
                    || !_playerDirection.Forward || _playerDirection.Backward)
                stoppedSprinting();
        }
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
        _currentAngle += _rotationSpeed * Time.deltaTime;
        Quaternion targetRotation = Quaternion.Euler(0, _currentAngle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
    }

    void turnLeft()
    {
        _currentAngle -= _rotationSpeed * Time.deltaTime;
        Quaternion targetRotation = Quaternion.Euler(0, _currentAngle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
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
                gameController.SwitchPlayerreverse();
                // Show the prompt if the object has the target tag
                if (promptUI != null)
                    promptUI.SetActive(true);
                return;
            }
        }
    }
}
