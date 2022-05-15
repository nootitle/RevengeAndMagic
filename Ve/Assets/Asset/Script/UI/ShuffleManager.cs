using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShuffleManager : MonoBehaviour
{
    public static ShuffleManager Instance = null;

    [SerializeField] List<RectTransform> _choiceCard = null;
    [SerializeField] List<GameObject> _choice = null;
    [SerializeField] float _speed = 10.0f;
    Coroutine _downCo = null;
    List<Vector2> _originalPos = null;
    bool _isOpenOne = false;

    [SerializeField] List<Sprite> _sp = null;
    public int getSpCount() { return _sp.Count; }
    [SerializeField] List<GameObject> _item = null;
    public List<int> ID_List = null;

    int chosen = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        ID_List = new List<int>();
        for (int i = 0; i < _sp.Count; ++i)
            ID_List.Add(0);

        _originalPos = new List<Vector2>();
        for(int i = 0; i < _choiceCard.Count; ++i)
            _originalPos.Add(_choiceCard[i].anchoredPosition);
    }

    public void startDown(int ch_id)
    {
        if(!_isOpenOne)
        {
            _isOpenOne = true;
            chosen = ch_id;
            if (_downCo != null) StopCoroutine(_downCo);
            _downCo = StartCoroutine(cardDown(ch_id));
        }
    }

    IEnumerator cardDown(int ch_id)
    {
        while(_choiceCard[ch_id].anchoredPosition.y >= -1200.0f)
        {
            yield return new WaitForSeconds(0.01f);
            _choiceCard[ch_id].anchoredPosition += Vector2.down * _speed;

            if (_choiceCard[ch_id].anchoredPosition.y <= -60.0f)
                _choice[ch_id].SetActive(true);
        }
    }

    public void startDownAll()
    {
        if(_isOpenOne)
        {
            if (_downCo != null) StopCoroutine(_downCo);
            _downCo = StartCoroutine(cardDownAll());
            _isOpenOne = false;
        }
    }

    IEnumerator cardDownAll()
    {
        bool flag = true;
        while (flag)
        {
            flag = false;
            for (int i = 0; i < _choiceCard.Count; ++i)
            {
                if(_choiceCard[i].anchoredPosition.y >= -1200.0f)
                {
                    flag = true;
                    break;
                }
            }                

            yield return new WaitForSeconds(0.01f);
            for (int i = 0; i < _choiceCard.Count; ++i)
            {
                _choiceCard[i].anchoredPosition += Vector2.down * _speed;

                if (_choiceCard[i].anchoredPosition.y <= -60.0f)
                    _choice[i].SetActive(true);
            }
        }

        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);
        StageManager.Instance.pause = false;
        makeItem();
    }

    public void restart()
    {
        if (StorageManager.Instance.getGold() >= 10 && _isOpenOne)
        {
            _isOpenOne = false;
            StorageManager.Instance.setGold(-10);
            for (int i = 0; i < _choiceCard.Count; ++i)
            {
                _choiceCard[i].anchoredPosition = _originalPos[i];
                _choice[i].SetActive(false);
            }
        }
    }

    public Sprite setSprite(int id)
    {
        return _sp[id];
    }

    public void setChoice(int value)
    {
        chosen = value;
    }

    public void makeItem()
    {
        GameObject gm = Instantiate(_item[ID_List[chosen]]);
        gm.transform.position = GameObject.Find("Player").transform.position + Vector3.up * 3.0f;
    }
}
