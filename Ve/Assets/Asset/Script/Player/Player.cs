using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //ĳ���� �⺻ ���

    [SerializeField] private GameObject SkillList;
    [SerializeField] float _jumpPower = 5.0f;
    [SerializeField] float _sprintSpeed = 10.0f;
    [SerializeField] float _mapInterval = 30.0f;
    [SerializeField] float _maxHp = 100.0f;
    public float _hp = 100.0f;
    [SerializeField] float _attackDistance = 2.0f;
    [SerializeField] float _attackDelay = 2.0f;
    [SerializeField] float _attackDamage = 2.0f;
    [SerializeField] float _stun = 2.0f;
    [SerializeField] GameObject _stunFx = null;
    [SerializeField] public List<int> _SkillLevelList = null;
    Coroutine _stunCo = null;
    Coroutine _extraAttackCo = null;
    Coroutine _extraMeleeAttackCo = null;
    float _meleeExtraDamage = 0.0f;
    float _magicExtraDamage = 0.0f;
    public float _extraHeal = 0.0f;
    float _delayCount = 0.0f;

    bool _jumpTrigger = false;
    bool _sprintTrigger = false;
    bool _isDie = false;
    bool _isStun = false;

    private bool skillListCheck;
    private Rigidbody2D _rb = null;

    [SerializeField] GameObject _bg = null;
    [SerializeField] bool _bgExtension = false;
    private float pointX = 0;
    [SerializeField] PlayerController _pc = null;
    [SerializeField] GameObject _normalMode = null;
    [SerializeField] GameObject _swordMode = null;
    int _modeID = 0;
    [SerializeField] FootStepSE _se = null;
    [SerializeField] MeleeSE _se_Melee = null;
    [SerializeField] DamagedSE _se_damaged = null;
    [SerializeField] AudioSource _swordFallSE = null;

    [SerializeField] GameObject _hitFx = null;

    //UI

    [SerializeField] GameObject _hpBar = null;
    [SerializeField] GameObject _skillSlot = null;
    [SerializeField] Joystick _joyStick = null;
    [SerializeField] bool _joyStickMode = true;

    //��ų ������
    [SerializeField] GameObject _fireBall = null;

    [SerializeField] GameObject _healFx = null;

    [SerializeField] GameObject _bulletsObject = null;
    Coroutine _spreadProcessCo = null;

    [SerializeField] GameObject _throwingStoneObject = null;

    [SerializeField] GameObject _sharpShooterFx = null;
    [SerializeField] float _sharpShooterDuration = 5.0f;
    [SerializeField] float _magicExtraDamage_sharpShooter = 5.0f;
    Coroutine _sharpShooterCo = null;

    [SerializeField] List<GameObject> _trap = null;

    [SerializeField] List<GameObject> _trackingBullets = null;
    [SerializeField] GameObject _trackingCastFx = null;
    [SerializeField] float _trackingAreaRadius = 5.0f;

    [SerializeField] GameObject _shieldBar = null;
    [SerializeField] GameObject _shieldFx = null;
    [SerializeField] AudioSource _shieldSE = null;
    [SerializeField] float _shieldMax = 500.0f;
    float _shield = 0.0f;

    [SerializeField] float _durationSliding = 2.0f;
    [SerializeField] AudioSource _slidingSE = null;
    Coroutine _slidingCo = null;
    bool _isSliding = false;

    [SerializeField] AudioSource _counterReadySE = null;
    [SerializeField] GameObject _counterFx = null;
    bool _isReadyToCounter = false;

    [SerializeField] GameObject _steamPackFx = null;

    [SerializeField] List<GameObject> _airforceBombs = null;
    Coroutine _airforceCo = null;

    string currentID = "";

    [SerializeField] GameObject _gameOverCanvas = null;
    [SerializeField] GameObject _escMenuCanvas = null;

    [SerializeField] List<GameObject> _alies1 = null;
    [SerializeField] List<GameObject> _alies2 = null;

    Coroutine _ultGaugeUpCo = null;
    Coroutine _ultCo = null;
    Coroutine _ult2Co = null;

    private void Awake()
    {
        List<string> statusList = new List<string>();

        if (DataStreamToStage.Instance != null)
        {
            string id = DataStreamToStage.Instance.getID();

            string stream = PlayerPrefs.GetString(id + "PlayerSetting");
            currentID = id;

            string temp = "";
            for (int i = 0; i < stream.Length; ++i)
            {
                if (stream[i] == ' ')
                {
                    statusList.Add(temp);
                    temp = "";
                }
                else
                    temp += stream[i];
            }

            _maxHp = float.Parse(statusList[0]);
            _hpBar.GetComponent<HpBar>().MaxHpBarInit(_maxHp);
            _hpBar.GetComponent<HpBar>().setHpBar(_maxHp);
            _attackDamage = float.Parse(statusList[1]);
            _magicExtraDamage = float.Parse(statusList[2]);
            _extraHeal = float.Parse(statusList[3]);
            if (statusList[4] == "0")
                _modeID = 0;
            else if (statusList[4] == "1")
            {
                useSwordMode();
            }
            StorageManager.Instance.setGold(int.Parse(statusList[5]));
        }
       
    }

    void Start()
    {
        skillListCheck = false;
        SkillList.SetActive(false);
        _rb = this.GetComponent<Rigidbody2D>();
        _hp = _maxHp;

        if(_SkillLevelList.Count == 0)
        {
            for (int i = 0; i <= Skill_Info.Instance._iconSource.Count; ++i)
                _SkillLevelList.Add(1);
        }
    }

    void Update()
    {
        if (_isDie) return;
        if (_isStun) return;
 
        if (StageManager.Instance.pause)
        {
            jumpCoolDown();
            _pc.MoveForJoyStick(0.0f);
            return;
        }

        float Hor;
        if(_joyStickMode)
        {
            float Joy = _joyStick.GetSpeed();
            if (Joy < 0)
                Hor = -0.8f;
            else if (Joy > 0)
                Hor = 0.8f;
            else
                Hor = 0.0f;         
        }
        else
        {
            Hor = Input.GetAxis("Horizontal");
        }

        //Ŭ������ ����, ��ų ����ϴ� �κ��� SkillSlot���� ã�ư� ��
        //�⺻����(Ű����) 
        if (Input.GetKeyDown(KeyCode.F))
        {
            cancelCounter();
            _skillSlot.transform.GetChild(0).GetComponent<SkillSlotController>().StartCooldown();
            Attack();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            cancelCounter();
            _skillSlot.transform.GetChild(1).GetComponent<SkillSlotController>().StartCooldown();
            stomping();
        }

        if (!_joyStickMode)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                cancelCounter();
                jump();
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                cancelCounter();
                sprint();
            }
            else
            {
                sprintStop();
            }
        }

        if (Hor >= 0.2f || Hor <= -0.2f)
        {
            if (!_jumpTrigger && !_sprintTrigger)
                _se.PlayWalk();
            else if (!_jumpTrigger && _sprintTrigger)
                _se.PlayRun();
        }
          
        if(!_sprintTrigger)
            transform.Translate(Hor * 5.0f * Time.deltaTime, 0.0f, 0.0f);
        else
            transform.Translate(Hor * _sprintSpeed * Time.deltaTime, 0.0f, 0.0f);
        if (_joyStickMode) _pc.MoveForJoyStick(Hor);

        if(_bgExtension)
        {
            if (transform.position.x > pointX - _mapInterval)
            {
                GameObject Obj = Instantiate(_bg);
                Vector3 vPos = new Vector3(pointX + _mapInterval, 0.0f, 0.0f);
                pointX += _mapInterval;
                Obj.transform.position = vPos;
            }
        }

        //��ų(Ű����)
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _skillSlot.transform.GetChild(2).GetComponent<SkillSlotController>().StartCooldown();
            fireBall();
        }
        */

        if (Input.GetKeyDown(KeyCode.L))
            Ult();

        if (Input.GetKeyDown(KeyCode.P))
            Ult2();

        if (Input.GetKeyDown(KeyCode.M))
            UltBarManager.Instance.updateSlider(100.0f);

        if (Input.GetKeyDown(KeyCode.Escape))
            CallEsc();

        if (_delayCount < _attackDelay)
            _delayCount += Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        jumpCoolDown();
        if (_isSliding)
        {
            if(collision.transform.gameObject.tag.Contains("Enemy"))
            {
                Enemy_Hit EH = collision.gameObject.GetComponent<Enemy_Hit>();
                if (EH != null)
                {
                    _slidingSE.Play();
                    EH.Hit(_attackDamage * _SkillLevelList[10]);
                }
            }
        }
    }

    void CallEsc()
    {
        SoundManager.Instance.clickSE();
        if (_escMenuCanvas.activeSelf)
            _escMenuCanvas.SetActive(false);
        else
            _escMenuCanvas.SetActive(true);
        SoundManager.Instance.initMasterVolumeSlider();
    }

    public void SkillListInvisible()
    {
        skillListCheck = !skillListCheck;
        SkillList.SetActive(skillListCheck);
    }

    public void jump()
    {
        if (!_jumpTrigger)
        {
            _se.PlayJump();
            _jumpTrigger = true;
            if(_rb.velocity.y <= 10.0f)
                _rb.AddForce(Vector3.up * _jumpPower, ForceMode2D.Impulse);
        }
    }

    private void sprint()
    {
        if (!_sprintTrigger)
            _sprintTrigger = true;
    }

    private void sprintStop()
    {
        if (_sprintTrigger)
            _sprintTrigger = false;
    }

    public void sprintForButton()
    {
        if (_sprintTrigger)
            sprintStop();
        else
            sprint();
    }

    void jumpCoolDown()
    {
        if(_jumpTrigger)
        {
            _jumpTrigger = false;
            _pc.jumpCoolDown();
            _se.PlayLand();
        }
    }

    public void Attack()
    {
        if (_delayCount >= _attackDelay)
        {
            _pc.Attack();
            _se_Melee.bladeSE();
            if(_modeID == 0)
                _delayCount = 0.0f;
            Collider2D[] co = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), _attackDistance);

            foreach (Collider2D c in co)
            {
                Enemy_Hit EH = c.gameObject.GetComponent<Enemy_Hit>();
                if (EH != null)
                {
                    float dis_y = Mathf.Abs(EH.GetCenter().transform.position.y - this.transform.position.y);
                    if (dis_y < 2.5f)
                    {
                        EH.Hit(_attackDamage * _SkillLevelList[0] + _meleeExtraDamage);
                        GameObject gm = Instantiate(_hitFx);
                        gm.transform.position = EH.GetCenter().transform.position;
                    }
                }
                    
            }

            if (_modeID == 1)
            {
                if (_extraAttackCo != null) StopCoroutine(_extraAttackCo);
                _extraAttackCo = StartCoroutine(extraAttack());
            }
        }
    }

    IEnumerator extraAttack()
    {
        yield return new WaitForSeconds(0.1f);

        _se_Melee.bladeSE();
        _delayCount = 0.0f;
        Collider2D[] co = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), _attackDistance);

        foreach (Collider2D c in co)
        {
            Enemy_Hit EH = c.gameObject.GetComponent<Enemy_Hit>();
            if (EH != null)
            {
                float dis_y = Mathf.Abs(EH.GetCenter().transform.position.y - this.transform.position.y);
                if (dis_y < 2.5f)
                {
                    EH.Hit(_attackDamage * _SkillLevelList[0]);
                    GameObject gm = Instantiate(_hitFx);
                    gm.transform.position = EH.GetCenter().transform.position;
                }
            }
        }
    }

    public void stomping()
    {
        if (_delayCount >= _attackDelay)
        {
            _pc.Stomp();
            _se_Melee.bladeSE();
            _delayCount = 0.0f;

            Collider2D[] co = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y + 2.0f), _attackDistance);

            foreach (Collider2D c in co)
            {
                Enemy_Hit EH = c.gameObject.GetComponent<Enemy_Hit>();
                if (EH != null)
                {
                    float dis_y = (EH.GetCenter().transform.position.y - this.transform.position.y);
                    if(dis_y < 4.0f && dis_y > 0.0f)
                    {
                        if (_modeID == 0)
                        {
                            EH.Hit(_attackDamage * 1.5f * _SkillLevelList[1] + _meleeExtraDamage);
                            GameObject gm = Instantiate(_hitFx);
                            gm.transform.position = EH.GetCenter().transform.position;
                        }
                        else if (_modeID == 1)
                        {
                            EH.Hit(_attackDamage * 2.0f * _SkillLevelList[1] + _meleeExtraDamage);
                            GameObject gm = Instantiate(_hitFx);
                            gm.transform.position = EH.GetCenter().transform.position;
                        }
                    }
                }
            }
        }
    }

    public void Damaged(float value)
    {
        if (_isDie) return;
        if (_isSliding) return;
        if (StageManager.Instance.pause) return;

        if (_isReadyToCounter)
        {
            _counterFx.SetActive(true);
            Collider2D[] co = Physics2D.OverlapCircleAll(this.transform.position, _attackDistance);
            for(int i = 0; i < co.Length; ++i)
            {
                Enemy_Hit EH = co[i].gameObject.GetComponent<Enemy_Hit>();
                if(EH != null)
                {
                    EH.Hit(_attackDamage * _SkillLevelList[11] * 2.0f + _meleeExtraDamage);
                }
            }
            _isReadyToCounter = false;
            _pc.CounterAnim();
            _se_Melee.bladeSE();
            return;
        }

        float temp = value;
        if(_shield > 0)
            _shieldSE.Play();

        if (_shield >= temp)
        {
            _shield -= temp;
            _shieldBar.GetComponent<HpBar>().setHpBar(_shield);
            return;
        }
        else
        {
            temp -= _shield;
            _shieldBar.GetComponent<HpBar>().setHpBar(_shield);
            _hp -= value;
        }

        if (_shield <= 0)
            _shieldFx.SetActive(false);

        _pc.DamagedAnim();
        if (_hpBar != null)
            _hpBar.GetComponent<HpBar>().setHpBar(_hp);
        _se_damaged.PlayDamaged();
        if (_hp <= 0)
            Die();
    }

    public void StunStart()
    {
        if (!_isDie && !_isStun)
        {
            _isStun = true;
            if (_stunCo != null) StopCoroutine(_stunCo);
            _stunFx.SetActive(true);
            _stunCo = StartCoroutine(Stun());
        }
    }

    public void healed(float value)
    {
        if (_isDie) return;

        GameObject gm = Instantiate(_healFx);
        float temp = Mathf.Min(_hp + value, _maxHp);
        _hp = temp;
        if (_hpBar != null)
            _hpBar.GetComponent<HpBar>().setHpBar(temp);
    }

    IEnumerator Stun()
    {
        yield return new WaitForSeconds(_stun);

        if (!_isDie)
        {
            _isStun = false;
            _stunFx.SetActive(false);
            _pc.MoveForJoyStick(0.0f);
        }
    }

    void Die()
    {
        StageManager.Instance.offRewardWindow();
        _hpBar.GetComponent<HpBar>().setHpBar(0);
        _pc.DieAnim();
        _isDie = true;
        _rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        _swordFallSE.Play();
        updateUserData();
        _gameOverCanvas.SetActive(true);
        SoundManager.Instance.defeatBGM();
    }

    public void updateUserData()
    {
        int gold = StorageManager.Instance.getGold();
        PlayerPrefs.SetString(currentID + "PlayerSetting", _maxHp.ToString() + " " +
            _attackDamage.ToString() + " " + _magicExtraDamage.ToString() + " " +
            _extraHeal.ToString() + " " + _modeID.ToString() + " " + gold.ToString() + " ");
    }

    //��ų �۾���

    public void setSkillLevel(int id, int level)
    {
        if (id == -1)
            _SkillLevelList[id] = 1;
        else
            _SkillLevelList[id] += level;
    }

    public void fireBall()
    {
        if (_fireBall != null)
        {
            _pc.Attack();
            GameObject gm = Instantiate(_fireBall);
            gm.GetComponent<Bullet_straight>().setAlies(true);
            gm.GetComponent<Bullet_straight>().setExtraDamage(_SkillLevelList[2]);
            if (_magicExtraDamage > 0)
                gm.GetComponent<Bullet_straight>().addExtraDamage(_magicExtraDamage);
            if (_pc.flip)
            {
                gm.GetComponent<Bullet_straight>().setDirection(Vector3.left);
                gm.transform.position = this.transform.position + Vector3.left;
            }
            else
            {
                gm.GetComponent<Bullet_straight>().setDirection(Vector3.right);
                gm.transform.position = this.transform.position + Vector3.right;
            }
        }
    }

    public void heal()
    {
        if (_healFx != null)
        {
            healed(5.0f * _SkillLevelList[3] + _extraHeal);
        }
    }

    public void spreadBullets()
    {
        if (_bulletsObject != null)
        {
            _pc.Attack();
            if (_spreadProcessCo != null) StopCoroutine(_spreadProcessCo);
            _spreadProcessCo = StartCoroutine(spreadBulletProcess());
        }
    }

    IEnumerator spreadBulletProcess()
    {
        for(int i = 0; i < 3; ++i)
        {
            MakeSpreadBullets();
            yield return new WaitForSeconds(0.2f);
        }
    }

    void MakeSpreadBullets()
    {
        Vector3 _angle = Vector3.zero;
        Vector3 _dir = Vector3.right;
        for (int i = 0; i < 5; ++i)
        {
            switch(i)
            {
                case 0: _angle = new Vector3(0.0f, 0.0f, 75.0f); _dir = new Vector3(0.8f, 0.2f, 0.0f); break;
                case 1: _angle = new Vector3(0.0f, 0.0f, 35.0f); _dir = new Vector3(0.9f, 0.1f, 0.0f); break;
                case 2: _angle = new Vector3(0.0f, 0.0f, 0.0f); _dir = new Vector3(1.0f, 0.0f, 0.0f); break;
                case 3: _angle = new Vector3(0.0f, 0.0f, -35.0f); _dir = new Vector3(0.9f, -0.1f, 0.0f); break;
                case 4: _angle = new Vector3(0.0f, 0.0f, -75.0f); _dir = new Vector3(0.8f, -0.2f, 0.0f); break;
            }

            GameObject gm = Instantiate(_bulletsObject);
            gm.GetComponent<Bullet_straight>().setAlies(true);
            gm.GetComponent<Bullet_straight>().setExtraDamage(_SkillLevelList[4]);
            if (_magicExtraDamage > 0)
                gm.GetComponent<Bullet_straight>().addExtraDamage(_magicExtraDamage);
            if (_pc.flip)
            {
                gm.GetComponent<Bullet_straight>().setDirection(-_dir, -_angle);
                gm.transform.position = this.transform.position + Vector3.left;
            }
            else
            {
                gm.GetComponent<Bullet_straight>().setDirection(_dir, _angle);
                gm.transform.position = this.transform.position + Vector3.right;
            }
        }
    }

    public void ThrowingStone()
    {
        if (_throwingStoneObject != null)
        {
            _pc.Attack();
            GameObject gm = Instantiate(_throwingStoneObject);
            gm.GetComponent<Bullet_straight>().setAlies(true);
            gm.GetComponent<Bullet_straight>().setExtraDamage(_SkillLevelList[5]);
            if (_magicExtraDamage > 0)
                gm.GetComponent<Bullet_straight>().addExtraDamage(_magicExtraDamage);
            if (_pc.flip)
            {
                gm.GetComponent<Bullet_straight>().setDirection(Vector3.left, new Vector3(0.0f, 0.0f, -180.0f));
                gm.transform.position = this.transform.position + Vector3.left;
            }
            else
            {
                gm.GetComponent<Bullet_straight>().setDirection(Vector3.right);
                gm.transform.position = this.transform.position + Vector3.right;
            }
            
        }
    }

    public void sharpShooter()
    {
        if(_sharpShooterFx != null)
        {
            if (_sharpShooterCo != null) 
                StopCoroutine(_sharpShooterCo);

            _pc.Stomp();
            _sharpShooterFx.SetActive(true);
            _magicExtraDamage += _magicExtraDamage_sharpShooter * _SkillLevelList[6];
            _sharpShooterCo = StartCoroutine(sharpShooterCoolDown());
        }
    }

    IEnumerator sharpShooterCoolDown()
    {
        yield return new WaitForSeconds(_sharpShooterDuration);

        _sharpShooterFx.SetActive(false);
        _magicExtraDamage -= _magicExtraDamage_sharpShooter * _SkillLevelList[6];
    }

    //���� ��ų�� �߰� �رݵǴ� ��ų

    public void setTrap()
    {
        for (int i = 0; i < _trap.Count; ++i)
        {
            if(!_trap[i].activeSelf)
            {
                _trap[i].SetActive(true);
                _trap[i].GetComponent<BearTrap>().UpgradeDuration(_SkillLevelList[7]);
                if(_pc.flip)
                    _trap[i].transform.position = this.transform.position + new Vector3(-3.0f, 3.0f, 0.0f);
                else
                    _trap[i].transform.position = this.transform.position + new Vector3(3.0f, 3.0f, 0.0f);
                break;
            }
        }
    }

    public void trackingBullet()
    {
        GameObject gm = Instantiate(_trackingCastFx);
        gm.transform.position = this.transform.position;
        Collider2D[] co = Physics2D.OverlapCircleAll(this.transform.position, _trackingAreaRadius);
        int idx = 0;

        foreach (Collider2D c in co)
        {
            if (c.transform.gameObject.tag.Contains("Enemy"))
            {
                if (!_trackingBullets[idx].activeSelf)
                {
                    _trackingBullets[idx].SetActive(true);
                    _trackingBullets[idx].GetComponent<TrackingBullet>().Upgrade(_SkillLevelList[8]);
                    _trackingBullets[idx].GetComponent<TrackingBullet>().startTracking(c.transform.gameObject);

                }
                ++idx;
            }
            if (idx == _trackingBullets.Count)
                break;
        }
    }

    public void MakeShield()
    {
        _shieldSE.Play();
        _shieldFx.SetActive(true);
        _shield = Mathf.Min(_shield + _shieldMax * _SkillLevelList[9], 100.0f);
        _shieldBar.GetComponent<HpBar>().setHpBar(_shield);
    }

    public void Sliding()
    {
        _pc.slidingAnim();
        _isSliding = true;
        if (_slidingCo != null) StopCoroutine(_slidingCo);
        _slidingCo = StartCoroutine(ExitSliding());
    }

    IEnumerator ExitSliding()
    {
        yield return new WaitForSeconds(_durationSliding * _SkillLevelList[10]);

        _isSliding = false;
    }

    public void Counter()
    {
        if(!_isReadyToCounter)
        {
            _isReadyToCounter = true;
            _pc.CounterReadyAnim();
            _counterReadySE.Play();
        }
    }

    void cancelCounter()
    {
        if(_isReadyToCounter)
        {
            _isReadyToCounter = false;
            _pc.CacnelCounterAnim();
        }
    }

    public void SteamPack()
    {
        _attackDelay /= 2.0f;
        _steamPackFx.SetActive(true);
        Invoke("endSteamPack", 15.0f * _SkillLevelList[12]);
    }

    void endSteamPack()
    {

        _steamPackFx.SetActive(false);
        _attackDelay *= 2.0f;
    }

    public void Airforce()
    {
        if (_airforceCo != null) StopCoroutine(_airforceCo);
        _airforceCo = StartCoroutine(extraAirforce());
    }

    IEnumerator extraAirforce()
    {
        for(int j = 0; j < _SkillLevelList[13]; ++j)
        {
            for (int i = 0; i < _airforceBombs.Count; ++i)
            {
                yield return new WaitForSeconds(0.2f);
                _airforceBombs[i].transform.position = this.transform.position +
                    Vector3.right * (i % 5) * 0.5f + Vector3.up * 15.0f;
                _airforceBombs[i].SetActive(true);
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    public void MakeAlies()
    {
        for(int i = 0; i < _SkillLevelList[14]; ++i)
        {
            if(_SkillLevelList[14] < 3)
            {
                _alies1[i].SetActive(true);
                _alies1[i].GetComponent<Alies>().respawn();
                _alies1[i].transform.position = this.transform.position + Vector3.right * 2.0f;
            }
            else
            {
                _alies2[i].SetActive(true);
                _alies2[i].GetComponent<Alies>().respawn();
                _alies2[i].transform.position = this.transform.position + Vector3.right * 2.0f;
            }
        }
    }

    //�⺻����, ��ų ���� ȣ�� �Լ�

    public void CallSkill(int id)
    {
        if (_isDie) return;

        cancelCounter();

        switch (id)
        {
            case 0: Attack(); break;
            case 1: stomping(); break;
            case 2: fireBall(); break;
            case 3: heal(); break;
            case 4: spreadBullets(); break;
            case 5: ThrowingStone(); break;
            case 6: sharpShooter(); break;
            case 7: setTrap(); break;
            case 8: trackingBullet(); break;
            case 9: MakeShield(); break;
            case 10: Sliding(); break;
            case 11: Counter(); break;
            case 12: SteamPack(); break;
            case 13: Airforce(); break;
            case 14: MakeAlies(); break;
        }
    }

    //�нú�

    public void setMaxHp(float value)
    {
        _maxHp += value;
        _hpBar.GetComponent<HpBar>().setMaxHpBar(5.0f);
    }

    public void UpgradeMelee(float value)
    {
        _attackDamage += value;
    }

    public void UpgradeMagic(float value)
    {
        _magicExtraDamage += value;
    }

    public void UpgradeHeal(float value)
    {
        _extraHeal += value;
    }

    public void useSwordMode()
    {
        if(_modeID != 1)
        {
            _swordMode.SetActive(true);
            _normalMode.SetActive(false);
            _pc = _swordMode.GetComponent<PlayerController>();
            _attackDistance += 1.0f;
            _modeID = 1;
        }
        else
        {
            _swordMode.SetActive(false);
            _normalMode.SetActive(true);
            _pc = _normalMode.GetComponent<PlayerController>();
            _attackDistance -= 1.0f;
            _modeID = 0;
        }
    }

    public void AddNewSkillOnList(int id)
    {
        Skill_Info.Instance.AddNewSkillOnList(id);
    }

    //��Ÿ ��ų(������ ȹ�� �� �ߵ� ��ų ��)

    public void riseExtraAttack(float value, float duration)
    {
        _meleeExtraDamage += value;
        if (_extraMeleeAttackCo != null) StopCoroutine(_extraMeleeAttackCo);
        _extraMeleeAttackCo = StartCoroutine(offriseExtraAttack(value, duration));
    }

    IEnumerator offriseExtraAttack(float value, float duration)
    {
        yield return new WaitForSeconds(duration);
        _meleeExtraDamage -= value;
    }

    public void MakeShieldByItem(float value)
    {
        _shieldSE.Play();
        _shieldFx.SetActive(true);
        _shield = Mathf.Max(_shieldMax, _shield + value);
        _shieldBar.GetComponent<HpBar>().setHpBar(_shield);
    }

    public void UltGaugeUp(float value)
    {
        if (_ultGaugeUpCo != null) StopCoroutine(_ultGaugeUpCo);
        _ultGaugeUpCo = StartCoroutine(UltGaugeUpSlowly(value));
    }

    IEnumerator UltGaugeUpSlowly(float value)
    {
        float _value = value;

        while(_value > 0)
        {
            yield return new WaitForSeconds(0.1f);

            _value -= 1.0f;
            UltBarManager.Instance.updateSlider(1.0f);
        }
    }

    //�ñر�

    public void Ult()
    {
        if(UltBarManager.Instance.GetUltGauge() >= 100.0f)
        {
            _pc.UltAnim_Ready();
            if (_ultCo != null) StopCoroutine(_ultCo);
            _ultCo = StartCoroutine(Ult_AnimProcess());
            CutInManager.Instance.Activate();
            UltBarManager.Instance.initSlider();
        }
    }

    IEnumerator Ult_AnimProcess()
    {
        yield return new WaitForSeconds(1.0f);
        _pc.UltAnim();
        yield return new WaitForSeconds(1.0f);
        _pc.UltAnim_Over();
    }

    public void Ult2()
    {
        if (UltBarManager.Instance.GetUltGauge() >= 100.0f)
        {
            if (_ult2Co != null) StopCoroutine(_ult2Co);
            _ult2Co = StartCoroutine(Ult2_AnimProcess());
            CutInManager.Instance.Activate2();
            UltBarManager.Instance.initSlider();
        }
    }

    IEnumerator Ult2_AnimProcess()
    {
        yield return new WaitForSeconds(1.1f);
        _pc.UltAnim2_Ready();
        yield return new WaitForSeconds(0.9f);
        _pc.UltAnim2();
        yield return new WaitForSeconds(2.0f);
        _pc.UltAnim2_Over();
    }
}
