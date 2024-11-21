using UnityEngine;

public class WalkingCamel : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            _animator.SetBool("Running", true);
        else
            _animator.SetBool("Running", false);
    }
}
