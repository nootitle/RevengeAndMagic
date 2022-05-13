using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_Melee : MonoBehaviour
{
    [SerializeField] GameObject _center = null;
    [SerializeField] float _maxHp = 100.0f;
    float _hp = 100.0f;
    [SerializeField] float _jumpPower = 5.0f;
    [SerializeField] float _walkSpeed = 5.0f;
    float direction = 0.0f;
    [SerializeField] float _patrolRange = 10.0f;
    [SerializeField] float _chaseRange = 4.0f;
    [SerializeField] float _attackDistance = 6.0f;
    [SerializeField] float _attackDelay = 4.0f;
    [SerializeField] float _attackDamage = 2.0f;
    [SerializeField] float _stun = 2.0f;
    [SerializeField] AudioSource _hitSE = null;
    [SerializeField] AudioSource _attackSE = null;
    [SerializeField] GameObject _meleeFx = null;
    float _delayCount = 0.0f;
    bool _jumpTrigger = false;
    bool _isAttacking = false;
    bool _isStun = false;
    bool _isDie = false;
    Coroutine _stunCo;
    Coroutine _extraHitCo;
    Coroutine _jumpCo;

    private Rigidbody2D _rb = null;
    [SerializeField] Skeleton_Melee_Anim _pc = null;
    Vector3 _originalPosition;

    [SerializeField] GameObject _target = null;
    Player _player = null;

    public GameObject GetCenter() { return _center; }

    [SerializeField] GameObject _dropItem = null;

    void Start()
    {
        _hp = _maxHp;
        _rb = this.GetComponent<Rigidbody2D>();
        _originalPosition = this.transform.position;
        direction = 1.0f;
        _delayCount = _attackDelay;
        if (_target != null)
            _player = _target.GetComponent<Player>();
        jumpCoolDown();
    }

    void Update()
    {
        if (_isDie) return;
        if (_isStun) return;
        if (_isAttacking) return;
        if (_jumpTrigger)
        {
            transform.Translate(direction * _walkSpeed * Time.deltaTime, 0.0f, 0.0f);
            return;
        }
        if (StageManager.Instance.pause) return;

        if (_target != null && Vector2.Distance(_target.transform.position, _center.transform.position) <= _attackDistance)
            Attack();
        else if (_target != null && Vector2.Distance(_target.transform.position, _center.transform.position) <= _chaseRange)
            chasing();
        else
            Patrol();

        if (_delayCount < _attackDelay)
            _delayCount += Time.deltaTime;
    }

    private void Moving()
    {
         transform.Translate(direction * _walkSpeed * Time.deltaTime, 0.0f, 0.0f);
        if (direction == 1)
            _pc.setFlip(false);
        else if(direction == -1)
            _pc.setFlip(true);
        _pc.MoveAnim(false, _walkSpeed * direction);
    }

    private void chasing()
    {
        float dir = _target.transform.position.x - _center.transform.position.x;
        if (dir > 0)
        {
            direction = 1;
            _pc.setFlip(false);
        }
        else
        {
            direction = -1;
            _pc.setFlip(true);
        }
        _pc.MoveAnim(false, _walkSpeed * direction);
        transform.Translate(direction * _walkSpeed * Time.deltaTime, 0.0f, 0.0f);
    }

    private void Patrol()
    {
        if (_center.transform.position.x < _originalPosition.x - _patrolRange)
        {
            direction = 1;
            _pc.MoveAnim(false, direction * _walkSpeed);
            Moving();
        }
        else if (_center.transform.position.x > _originalPosition.x + _patrolRange)
        {
            direction = -1;
            _pc.MoveAnim(false, direction * _walkSpeed);
            Moving();
        }
        else
        {
            _pc.MoveAnim(false, direction * _walkSpeed);
            Moving();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        jumpCoolDown();
    }

    private void jump()
    {
        if (!_jumpTrigger)
        {
            if (_jumpCo != null) StopCoroutine(_jumpCo);
            _jumpCo = StartCoroutine(jumpCoolDownIE());
            _jumpTrigger = true;
            if (direction == 1)
                _rb.AddForce(Vector3.up * _jumpPower + Vector3.right * 5.0f, ForceMode2D.Impulse);
            else if (direction == -1)
                _rb.AddForce(Vector3.up * _jumpPower + Vector3.left * 5.0f, ForceMode2D.Impulse);
            else
                _rb.AddForce(Vector3.up * _jumpPower, ForceMode2D.Impulse);
        }
    }

    void jumpCoolDown()
    {
        if(_jumpTrigger)
        {
            _jumpTrigger = false;
            _pc.jumpCoolDown();
        }
    }

    IEnumerator jumpCoolDownIE()
    {
        yield return new WaitForSeconds(5.0f);

        jumpCoolDown();
    }

    void Attack()
    {
        if (_delayCount >= _attackDelay)
        {
            _pc.Attack();
            _isAttacking = true;

            if (_target.transform.position.x - _center.transform.position.x > 0)
                _pc.setFlip(false);
            else
                _pc.setFlip(true);

            _delayCount = 0.0f;
            if (_extraHitCo != null) StopCoroutine(_extraHitCo);
            _extraHitCo = StartCoroutine(ExtraHit());
        }
        else
            _pc.MoveAnim(true, 0);
    }

    IEnumerator ExtraHit()
    {
        yield return new WaitForSeconds(0.2f);
        if (!_isStun)
        {
            _attackSE.Play();
            if (_target != null && Vector2.Distance(_target.transform.position, _center.transform.position) <= _attackDistance)
            {
                GameObject gm = Instantiate(_meleeFx);
                gm.transform.position = _target.transform.position;
                _player.Damaged(_attackDamage);
            }
        }

        yield return new WaitForSeconds(0.5f);
        if (!_isStun)
        {
            _attackSE.Play();
            if (_target != null && Vector2.Distance(_target.transform.position, _center.transform.position) <= _attackDistance)
            {
                GameObject gm = Instantiate(_meleeFx);
                gm.transform.position = _target.transform.position;
                _player.Damaged(_attackDamage);
            }
        }

        _isAttacking = false;
        _pc.MoveAnim(false, 0);
    }

    public void Damaged(float value)
    {
        _hitSE.Play();
        _pc.DamagedAnim();
        _hp -= value;
        if (_hp <= 0)
            Die();
        else
        {
            _isStun = true;
            if (_stunCo != null) StopCoroutine(_stunCo);
            _stunCo = StartCoroutine("Stun");
        }
    }

    void Die()
    {
        _pc.DieAnim();
        _isDie = true;
        _isStun = false;
        _isAttacking = false;
        _jumpTrigger = false;

        int rnd = Random.Range(0, 100);
        if (rnd < 5)
            dropItem();

        StopAllCoroutines();
        StartCoroutine(SelfDestroy());
    }

    IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(1.0f);

        EnemyManager.Instance.deathCount();
        this.gameObject.SetActive(false);
    }

    public void respawn()
    {
        _isDie = false;
        _isStun = false;
        _isAttacking = false;
        _jumpTrigger = false;
        _hp = _maxHp;
    }

    public bool GetDead()
    {
        return _isDie;
    }

    IEnumerator Stun()
    {
        yield return new WaitForSeconds(_stun);

        if (!_isDie)
        {
            _isStun = false;
            _pc.MoveAnim(false, _rb.velocity.x);
        }
    }

    public void CallStun(float duration)
    {
        _isStun = true;
        if (_stunCo != null) StopCoroutine(_stunCo);
        _stunCo = StartCoroutine(exitCallStunFunction(duration));
    }

    IEnumerator exitCallStunFunction(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (!_isDie)
        {
            _isStun = false;
            _pc.MoveAnim(false, _rb.velocity.x);
        }
    }

    void dropItem()
    {
        if (_dropItem != null)
        {
            GameObject gm = Instantiate(_dropItem);
            gm.transform.position = _center.transform.position;
        }
    }
}
