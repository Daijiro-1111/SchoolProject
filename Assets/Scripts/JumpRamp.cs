using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpRamp : MonoBehaviour
{
    private Collider _collider;
    private Animator _animator;
    private float jumprampPower = 7.5f;
    private bool _isJumpRamp;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Player") && !_isJumpRamp)
        {
            _animator.SetBool("isJumpRamp", true);
            _isJumpRamp = true;
            _collider.isTrigger = true;
            collision.transform.GetComponent<PlayerController>().Stop();
            collision.transform.GetComponent<PlayerController>().Gimic(jumprampPower, jumprampPower * 2, 1);
        }
    }

    private void ResetJumpRump()
    {
        _animator.SetBool("isJumpRamp", false);
        _isJumpRamp = false;
        _collider.isTrigger = false;
    }
}
