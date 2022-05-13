using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScroll_CutScene : MonoBehaviour
{
    [SerializeField] GameObject _textCanvas = null;
    [SerializeField] GameObject _playerCG = null;
    [SerializeField] GameObject _bossCG = null;
    [SerializeField] Text _text = null;
    [SerializeField] Animator _playerAnim = null;
    [SerializeField] Animator _bossAnim = null;
    [SerializeField] GameObject _clearCanvas = null;
    Image _pImage = null;
    Image _bImage = null;
    int textID = 0;

    private void OnEnable()
    {
        _pImage = _playerCG.GetComponent<Image>();
        _bImage = _bossCG.GetComponent<Image>();

        textID = 0;
        switchingText();
        ++textID;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switchingText();
            ++textID;
        }
    }

    void switchingText()
    {
        SoundManager.Instance.clickSE();
        switch (textID)
        {
            case 0:
                {
                    _text.text = "��Ż�� ���ְ� : �� �̷� ������ �� �־�. �̰� ���� ���α��Ƴ�.";
                    _text.color = Color.red;
                    _bossCG.transform.position += Vector3.up * 30.0f;
                    _pImage.color = new Color(_pImage.color.r, _pImage.color.g, _pImage.color.b, 0.7f);
                    _bImage.color = new Color(_bImage.color.r, _bImage.color.g, _bImage.color.b, 1.0f);
                    break;
                }
            case 1:
                {
                    _text.text = "���ΰ� : �װ� �׷� ���� ������ �����µ�.";
                    _text.color = Color.white;
                    _bossCG.transform.position -= Vector3.up * 30.0f;
                    _playerCG.transform.position += Vector3.up * 30.0f;
                    _pImage.color = new Color(_pImage.color.r, _pImage.color.g, _pImage.color.b, 1.0f);
                    _bImage.color = new Color(_bImage.color.r, _bImage.color.g, _bImage.color.b, 0.7f);
                    break;
                }
            case 2:
                {
                    _text.text = "��Ż�� ���ְ� : �̺�. �� ����ָ�, �� �� �ɸ� ������� 2�踦 �ٰ�. �� ���� �ٽ� �����غ����.";
                    _text.color = Color.red;
                    _bossCG.transform.position += Vector3.up * 30.0f;
                    _playerCG.transform.position -= Vector3.up * 30.0f;
                    _pImage.color = new Color(_pImage.color.r, _pImage.color.g, _pImage.color.b, 0.7f);
                    _bImage.color = new Color(_bImage.color.r, _bImage.color.g, _bImage.color.b, 1.0f);
                    break;
                }
            case 3:
                {
                    _text.text = "���ΰ� : �� ������ ��ġ�� �� ë��.";
                    _text.color = Color.white;
                    _bossCG.transform.position -= Vector3.up * 30.0f;
                    _playerCG.transform.position += Vector3.up * 30.0f;
                    _pImage.color = new Color(_pImage.color.r, _pImage.color.g, _pImage.color.b, 1.0f);
                    _bImage.color = new Color(_bImage.color.r, _bImage.color.g, _bImage.color.b, 0.7f);
                    break;
                }
            case 4:
                {
                    _text.text = "��Ż�� ���ְ� : �װ� ���� ������.";
                    _text.color = Color.red;
                    _bossCG.transform.position += Vector3.up * 30.0f;
                    _playerCG.transform.position -= Vector3.up * 30.0f;
                    _pImage.color = new Color(_pImage.color.r, _pImage.color.g, _pImage.color.b, 0.7f);
                    _bImage.color = new Color(_bImage.color.r, _bImage.color.g, _bImage.color.b, 1.0f);
                    break;
                }
            case 5:
                {
                    _text.text = "���ΰ� : 10�� ��, �ʶ� �� ������ �� �ƹ����� ���̰�, ��Ӵϸ� ��Ż�ذ� �� ����.";
                    _text.color = Color.white;
                    _bossCG.transform.position -= Vector3.up * 30.0f;
                    _playerCG.transform.position += Vector3.up * 30.0f;
                    _pImage.color = new Color(_pImage.color.r, _pImage.color.g, _pImage.color.b, 1.0f);
                    _bImage.color = new Color(_bImage.color.r, _bImage.color.g, _bImage.color.b, 0.7f);
                    break;
                }
            case 6:
                {
                    _text.text = "��Ż�� ���ְ� : �׷� �ʴ�.. �ƴ� ��� ���� �̾�..";
                    _text.color = Color.red;
                    _bossCG.transform.position += Vector3.up * 30.0f;
                    _playerCG.transform.position -= Vector3.up * 30.0f;
                    _pImage.color = new Color(_pImage.color.r, _pImage.color.g, _pImage.color.b, 0.7f);
                    _bImage.color = new Color(_bImage.color.r, _bImage.color.g, _bImage.color.b, 1.0f);
                    _playerAnim.SetTrigger("Ult_CutScene");
                    CutInManager.Instance.Activate();
                    _bossAnim.SetBool("Die", true);
                    break;
                }
            case 7:
                {
                    _text.text = "���ΰ� : �ƴ� ����� �ʿ����. �����غ���, ���� ��� ���� �ʳ�. �׳� �׾�.";
                    _text.color = Color.white;
                    _bossCG.transform.position -= Vector3.up * 30.0f;
                    _playerCG.transform.position += Vector3.up * 30.0f;
                    _pImage.color = new Color(_pImage.color.r, _pImage.color.g, _pImage.color.b, 1.0f);
                    _bImage.color = new Color(_bImage.color.r, _bImage.color.g, _bImage.color.b, 0.7f);
                    _playerAnim.SetTrigger("Idle");
                    break;
                }
            case 8:
                {
                    _text.text = "���ΰ� : �� ���� ���� 8���� ��ٷ���. 8����...";
                    break;
                }
            case 9:
                {
                    _text.text = "";
                    _textCanvas.SetActive(false);
                    _bossCG.transform.position += Vector3.up * 30.0f;
                    _playerCG.transform.position -= Vector3.up * 30.0f;
                    _clearCanvas.SetActive(true);
                    SoundManager.Instance.clearBGM();
                    break;
                }
        }
    }
}
