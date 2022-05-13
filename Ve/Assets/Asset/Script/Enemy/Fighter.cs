using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [SerializeField] GameObject _center = null;
    [SerializeField] float _maxHp = 100.0f;
    float _hp = 100.0f;
    [SerializeField] float _jumpPower = 5.0f;
    [SerializeField] float _walkSpeed = 5.0f;
    [SerializeField] float _sprintSpeed = 10.0f;
    float direction = 0.0f;
    [SerializeField] float _patrolRange = 10.0f;
    [SerializeField] float _chaseRange = 4.0f;
    [SerializeField] float _attackDistance = 2.0f;
    [SerializeField] float _attackDelay = 2.0f;
    [SerializeField] float _attackDamage = 2.0f;
    [SerializeField] float _stun = 2.0f;
    [SerializeField] AudioSource _hitSE = null;
    [SerializeField] AudioSource _painSE = null;
    [SerializeField] AudioSource _meleeSE = null;
    [SerializeField] GameObject _meleeFx = null;
    float _delayCount = 0.0f;
    bool _jumpTrigger = false;
    bool _sprintTrigger = false;
    bool _isStun = false;
    bool _isDie = false;
    bool _isAttacking = false;
    Coroutine _sprintCo;
    Coroutine _stunCo;
    Coroutine _extraHitCo;
    Coroutine _extraHitCo2;
    Coroutine _jumpCo;

    private Rigidbody2D _rb = null;
    [SerializeField] Fighter_Anim _pc = null;
    Vector3 _originalPosition;

    [SerializeField] GameObject _target = null;
    Player _player = null;

    [SerializeField] GameObject _fireBall = null;
    [SerializeField] AudioSource _fireBallSE = null;
    [SerializeField] float _fireBallDistance = 7.0f;
    [SerializeField] float _fireBallDelay = 2.0f;
    float _fBDelay = 0.0f;

    public GameObject GetCenter() { return _center; }

    void Start()
    {
        _hp = _maxHp;
        _rb = this.GetComponent<Rigidbody2D>();
        _originalPosition = this.transform.position;
        direction = 1.0f;
        _delayCount = _attackDelay;
        _fBDelay = _fireBallDelay;
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
        {
            _pc.MoveAnim(false, true, 0.0f);
            int rnd = Random.Range(0, 100);
            if (rnd < 50)
                Attack();
            else
                stomping();

        }
        else if (_target != null && Vector2.Distance(_target.transform.position, _center.transform.position) <= _fireBallDistance)
        {
            _pc.MoveAnim(false, true, 0.0f);
            casting();
        }
        else if (_target != null && Vector2.Distance(_target.transform.position, _center.transform.position) <= _chaseRange)
        {
            _sprintTrigger = true;
            chasing();
        }
        else
        {
            _sprintTrigger = false;
            int rnd = Random.Range(0, 1000);
            if (rnd < 998 || _jumpTrigger)
                Patrol();
            else
                jump();
        }

        if (_delayCount < _attackDelay)
            _delayCount += Time.deltaTime;
        if (_fBDelay < _fireBallDelay)
            _fBDelay += Time.deltaTime;
    }

    private void Moving()
    {
        if (!_sprintTrigger)
            transform.Translate(direction * _walkSpeed * Time.deltaTime, 0.0f, 0.0f);
        else
            transform.Translate(direction * _sprintSpeed * Time.deltaTime, 0.0f, 0.0f);

        int rnd = Random.Range(0, 1000);
        if (!_jumpTrigger && rnd < 5)
            jump();
    }

    private void chasing()
    {
        float dir = _target.transform.position.x - _center.transform.position.x;
        if (dir > 0)
            direction = 1;
        else
            direction = -1;
        _pc.MoveAnim(_sprintTrigger, false, direction * _sprintSpeed);
        transform.Translate(direction * _sprintSpeed * Time.deltaTime, 0.0f, 0.0f);
    }

    private void Patrol()
    {
        if (_center.transform.position.x < _originalPosition.x - _patrolRange)
        {
            direction = 1;
            _pc.MoveAnim(_sprintTrigger, false, direction * _walkSpeed);
            Moving();
        }
        else if (_center.transform.position.x > _originalPosition.x + _patrolRange)
        {
            direction = -1;
            _pc.MoveAnim(_sprintTrigger, false, direction * _walkSpeed);
            Moving();
        }
        else
        {
            _pc.MoveAnim(_sprintTrigger, false, direction * _walkSpeed);
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
            _pc.jumpAnim();
            if (direction == 1)
                _rb.AddForce(Vector3.up * _jumpPower + Vector3.right * 5.0f, ForceMode2D.Impulse);
            else if (direction == -1)
                _rb.AddForce(Vector3.up * _jumpPower + Vector3.left * 5.0f, ForceMode2D.Impulse);
            else
                _rb.AddForce(Vector3.up * _jumpPower, ForceMode2D.Impulse);
        }
    }

    IEnumerator jumpCoolDownIE()
    {
        yield return new WaitForSeconds(5.0f);

        jumpCoolDown();
    }

    private void sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!_sprintTrigger)
                _sprintCo = StartCoroutine(sprintTerm());
        }
        else
        {
            if (_sprintCo != null)
                StopCoroutine(_sprintCo);
            _sprintTrigger = false;
        }
    }

    void jumpCoolDown()
    {
        if (_jumpTrigger)
        {
            _jumpTrigger = false;
            _pc.jumpCoolDown();
        }
    }

    IEnumerator sprintTerm()
    {
        yield return new WaitForSeconds(0.5f);

        _sprintTrigger = true;
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

            if (_extraHitCo != null) StopCoroutine(_extraHitCo);
            _extraHitCo = StartCoroutine(ExtraHit());
        }
    }

    IEnumerator ExtraHit()
    {
        yield return new WaitForSeconds(0.4f);
        _meleeSE.Play();
        if (_target != null && Vector2.Distance(_target.transform.position, _center.transform.position) <= _attackDistance)
        {
            GameObject gm = Instantiate(_meleeFx);
            gm.transform.position = _target.transform.position;
            _player.Damaged(_attackDamage);
        }

        yield return new WaitForSeconds(0.5f);
        _meleeSE.Play();
        if (_target != null && Vector2.Distance(_target.transform.position, _center.transform.position) <= _attackDistance)
        {
            GameObject gm = Instantiate(_meleeFx);
            gm.transform.position = _target.transform.position;
            _player.Damaged(_attackDamage);
        }

        _isAttacking = false;
        _delayCount = 0.0f;
    }

    void stomping()
    {
        if (_delayCount >= _attackDelay)
        {
            _isAttacking = true;
            _pc.Stomp();
            if (_target.transform.position.x - _center.transform.position.x > 0)
                _pc.setFlip(false);
            else
                _pc.setFlip(true);

            if (_extraHitCo2 != null) StopCoroutine(_extraHitCo2);
            _extraHitCo2 = StartCoroutine(ExtraHit2());
        }
    }

    IEnumerator ExtraHit2()
    {
        yield return new WaitForSeconds(0.2f);
        _meleeSE.Play();
        if (_target != null && Vector2.Distance(_target.transform.position, _center.transform.position) <= _attackDistance)
        {
            GameObject gm = Instantiate(_meleeFx);
            gm.transform.position = _target.transform.position;
            _player.Damaged(_attackDamage);
        }

        _isAttacking = false;
        _delayCount = 0.0f;
    }

    void casting()
    {
        if (_fBDelay >= _fireBallDelay)
        {
            _pc.Cast();
            _fireBallSE.Play();
            GameObject gm = Instantiate(_fireBall);
            gm.transform.position = _center.transform.position;
            float temp = Mathf.Atan((_target.transform.position.y - _center.transform.position.y) /
                (_target.transform.position.x - _center.transform.position.x)) * Mathf.Rad2Deg;
            if (_target.transform.position.x - _center.transform.position.x > 0)
                gm.transform.eulerAngles = new Vector3(0.0f, 0.0f, gm.transform.eulerAngles.z + temp + 180.0f);
            else
                gm.transform.eulerAngles = new Vector3(0.0f, 0.0f, gm.transform.eulerAngles.z + temp);
            gm.GetComponent<Bullet_straight>().setDirection(_target.transform.position - gm.transform.position);

            gm.GetComponent<Bullet_straight>().setDirection(_target.transform.position
                - _center.transform.position);
            _fBDelay = 0.0f;
        }
    }

    public void Damaged(float value)
    {
        if (_isDie) return;

        _hitSE.Play();
        _painSE.Play();
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
            _pc.MoveAnim(_sprintTrigger, false, _rb.velocity.x);
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
            _pc.MoveAnim(_sprintTrigger, false, _rb.velocity.x);
        }
    }
}
