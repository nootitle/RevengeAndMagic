using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alies_H2_Anim : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _sr;
    bool _isjumping = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
    }

    public void MoveAnim(bool isRunning, bool isStop, float speed)
    {
        if (_isjumping) return;

        if (isRunning)
        {
            _animator.ResetTrigger("Walk");
            _animator.ResetTrigger("Idle");
            _animator.SetTrigger("Run");
        }
        else
        {
            _animator.ResetTrigger("Run");
            if (!isStop)
            {
                _animator.SetTrigger("Walk");
                _animator.ResetTrigger("Idle");
            }
            else
            {
                _animator.ResetTrigger("Walk");
                _animator.SetTrigger("Idle");
            }
        }

        if (speed < 0)
            _sr.flipX = true;
        else if (speed > 0)
            _sr.flipX = false;
        _animator.SetFloat("Speed", speed);
    }

    public void jumpAnim()
    {
        if (!_isjumping)
        {
            _isjumping = true;
            _animator.SetTrigger("jump");
            _animator.SetBool("isjumping", true);
            resetMoveTrigger();
        }
    }

    public void jumpCoolDown()
    {
        if (_isjumping)
        {
            _isjumping = false;
            _animator.ResetTrigger("jump");
            _animator.SetBool("isjumping", false);
        }
    }

    public void Attack()
    {
        resetMoveTrigger();
        _animator.SetTrigger("Attack_Normal");
    }

    public void Stomp()
    {
        resetMoveTrigger();
        _animator.SetTrigger("stomp");
    }

    void resetMoveTrigger()
    {
        _animator.ResetTrigger("Run");
        _animator.ResetTrigger("Walk");
        _animator.ResetTrigger("Idle");
    }

    public void DamagedAnim()
    {
        resetMoveTrigger();
        _animator.SetTrigger("Damaged");
    }

    public void setFlip(bool value)
    {
        _sr.flipX = value;
    }

    public void DieAnim()
    {
        _animator.SetBool("Die", true);
        _animator.SetTrigger("DieOnce");
    }
}
