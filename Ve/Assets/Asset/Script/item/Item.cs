using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] GameObject _player = null;
    [SerializeField] GameObject _center = null;
    [SerializeField] float _getDistance = 5.0f;
    [SerializeField] float _speed = 5.0f;
    [SerializeField] float _value = 50.0f;
    [SerializeField] int _itemType = 0;
    float _moveInterval = 0.2f;
    Vector3 InitialPos = Vector3.zero;
    bool dir = false;

    [SerializeField] GameObject _fx = null;

    /*
     * 0 : 카드게임
     * 1 : HP 회복
     * 2 : 근접공격력 일시적 증가
     * 3 : 보호막 생성
     * 4 : 궁극기 게이지 증가
     * 5 : +100G
     */

    void Start()
    {
        InitialPos = _center.transform.position;
        if (_center == null)
            _center = this.gameObject;
        if (_player == null)
            _player = GameObject.Find("Player");
    }

    void Update()
    {
        getItem();
        Move();
    }

    private void Move()
    {
        if(dir)
        {
            this.transform.position += Vector3.up * _speed * Time.deltaTime;
            if(this.transform.position.y >= InitialPos.y + _moveInterval)
                dir = false;
        }
        else if (!dir)
        {
            this.transform.position += Vector3.down * _speed * Time.deltaTime;
            if(this.transform.position.y <= InitialPos.y - _moveInterval)
                dir = true;
        }
    }

    void getItem()
    {
        if(Vector2.Distance(_center.transform.position, _player.transform.position) < _getDistance)
        {
            switch(_itemType)
            {
                case 0: item_type0(); break;
                case 1: item_type1(); break;
                case 2: item_type2(); break;
                case 3: item_type3(); break;
                case 4: item_type4(); break;
                case 5: item_type5(); break;
            }
        }
    }

    void item_type0()
    {
        StageManager.Instance.showCardCanvas();
        if (_fx != null)
        {
            GameObject gm = Instantiate(_fx);
            gm.transform.position = _center.transform.position + Vector3.down;
        }

        Destroy(this.gameObject);
    }

    void item_type1()
    {
        _player.GetComponent<Player>().healed(_value);
        if(_fx != null)
        {
            GameObject gm = Instantiate(_fx);
            gm.transform.position = _center.transform.position;
        }

        Destroy(this.gameObject);
    }

    void item_type2()
    {
        _player.GetComponent<Player>().riseExtraAttack(_value, 20.0f);
        if (_fx != null)
        {
            GameObject gm = Instantiate(_fx);
            gm.transform.position = _center.transform.position;
        }

        Destroy(this.gameObject);
    }

    void item_type3()
    {
        _player.GetComponent<Player>().MakeShieldByItem(_value);
        if (_fx != null)
        {
            GameObject gm = Instantiate(_fx);
            gm.transform.position = _center.transform.position;
        }

        Destroy(this.gameObject);
    }

    void item_type4()
    {
        _player.GetComponent<Player>().UltGaugeUp(_value);
        if (_fx != null)
        {
            GameObject gm = Instantiate(_fx);
            gm.transform.position = _center.transform.position + Vector3.down;
        }

        Destroy(this.gameObject);
    }

    void item_type5()
    {
        StorageManager.Instance.setGold(100);
        if (_fx != null)
        {
            GameObject gm = Instantiate(_fx);
            gm.transform.position = _center.transform.position + Vector3.down;
        }

        Destroy(this.gameObject);
    }
}
