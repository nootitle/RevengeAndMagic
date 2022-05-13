using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject _loginCanvas = null;
    [SerializeField] GameObject _lobbyCanvas = null;
    [SerializeField] GameObject _stageSelectCanvas = null;
    [SerializeField] GameObject _loadingCanvas = null;
    [SerializeField] GameObject _storageWindow = null;
    [SerializeField] Text _hpText = null;
    [SerializeField] Text _meleeText = null;
    [SerializeField] Text _magicText = null;
    [SerializeField] Text _healText = null;
    [SerializeField] Text _weaponText = null;
    string CurrentID = "";
    public void setID(string str) { CurrentID = str; }
    List<string> statusList = null; // 0 : hp, 1 : �������ݷ�, 2 : ���ݸ������õ�(���� �߰�������), 3 : ȸ���������õ�(�߰� ȸ��), 4 : Ư������
    [SerializeField] StorageManager _storage = null;

    public List<int> weaponList = null;

    public void LobbyInit(string id)
    {
        statusList = new List<string>();
        string temp = PlayerPrefs.GetString(id + "PlayerSetting");
        string temp2 = "";

        for (int i = 0; i < temp.Length; ++i)
        {
            if (temp[i] == ' ')
            {
                statusList.Add(temp2);
                temp2 = "";
            }
            else
                temp2 += temp[i];
        }

        _hpText.text = "�ִ�ü�� : " + statusList[0];
        _meleeText.text = "�������ݷ� : " + statusList[1];
        _magicText.text = "���ݸ������õ� : " + statusList[2];
        _healText.text = "ȸ���������õ� : " + statusList[3];
        if (statusList[4] == "0")
            _weaponText.text = "Ư����� : ����";
        else if (statusList[4] == "1")
            _weaponText.text = "Ư����� : �糯��";
        _storage.setGoldDirectly(int.Parse(statusList[5]));

        weaponList = new List<int>();
        if (statusList[4] == "1")
            weaponList.Add(1);

        SoundManager.Instance.initMasterVolumeSlider();
    }

    public void updateStatus(float hp, float melee, float magic, float heal)
    {
        statusList[0] = (float.Parse(statusList[0]) + hp).ToString();
        statusList[1] = (float.Parse(statusList[1]) + melee).ToString();
        statusList[2] = (float.Parse(statusList[2]) + magic).ToString();
        statusList[3] = (float.Parse(statusList[3]) + heal).ToString();

        _hpText.text = "�ִ�ü�� : " + statusList[0];
        _meleeText.text = "�������ݷ� : " + statusList[1];
        _magicText.text = "���ݸ������õ� : " + statusList[2];
        _healText.text = "ȸ���������õ� : " + statusList[3];
    }

    public void updateStatus(float hp, float melee, float magic, float heal, int weapon)
    {
        statusList[0] = (float.Parse(statusList[0]) + hp).ToString();
        statusList[1] = (float.Parse(statusList[1]) + melee).ToString();
        statusList[2] = (float.Parse(statusList[2]) + magic).ToString();
        statusList[3] = (float.Parse(statusList[3]) + heal).ToString();

        _hpText.text = "�ִ�ü�� : " + statusList[0];
        _meleeText.text = "�������ݷ� : " + statusList[1];
        _magicText.text = "���ݸ������õ� : " + statusList[2];
        _healText.text = "ȸ���������õ� : " + statusList[3];

        if (weapon == 0)
        {
            _weaponText.text = "Ư����� : ����";
            statusList[4] = "0";
        }
        else if (weapon == 1)
        {
            _weaponText.text = "Ư����� : �糯��";
            statusList[4] = "1";
            weaponList.Add(1);
        }
    }

    public void updateGold(int gold)
    {
        statusList[5] = gold.ToString();
    }

    public void finalSetStatus()
    {
        PlayerPrefs.SetString(CurrentID + "PlayerSetting", statusList[0] + " " +
            statusList[1] + " " + statusList[2] + " " + statusList[3] + " " +
            statusList[4] + " " + statusList[5] + " ");
    }

    public void GoToStageSelect()
    {
        _lobbyCanvas.SetActive(false);
        _stageSelectCanvas.SetActive(true);
    }

    public void GoToStage(int id)
    {
        _stageSelectCanvas.SetActive(false);
        _loadingCanvas.SetActive(true);
        switch (id)
        {
            case 1: SceneManager.LoadScene(1); break;
            case 2: SceneManager.LoadScene(2); break;
            case 3: SceneManager.LoadScene(3); break;
            case 4: SceneManager.LoadScene(5); break;
            case 5: SceneManager.LoadScene(6); break;
        }
    }

    public void showStatus()
    {
        if (_storageWindow.activeSelf)
            _storageWindow.SetActive(false);
        else
            _storageWindow.SetActive(true);
    }

    public void GoToLogin()
    {
        _lobbyCanvas.SetActive(false);
        _loginCanvas.SetActive(true);
    }
}
