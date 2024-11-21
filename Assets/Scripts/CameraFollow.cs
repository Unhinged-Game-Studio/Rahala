using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject _player;
    public float pLerp = .02f;
    public float rLerp = .01f;
    private Vector3 _offset;
    public float SmoothFactor = 0.5f;
    public float rotationSpeed = 5.0f;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        _offset = transform.position - _player.transform.position;
        transform.position = _player.transform.position + _offset;
    }

    public void LateUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Quaternion camTurnAngle = 
                Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);
            camTurnAngle *=
                Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * rotationSpeed, Vector3.right);
            _offset = camTurnAngle * _offset;
            Vector3 newPos =
                _player.transform.position + _offset;
            transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
            transform.LookAt(_player.transform);
            transform.Rotate(-15f, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
    }
}
