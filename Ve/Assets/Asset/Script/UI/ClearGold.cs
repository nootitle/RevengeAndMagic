using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearGold : MonoBehaviour
{
    private void OnEnable()
    {
        this.GetComponent<Text>().text = "ȹ���� ��� : " + StorageManager.Instance.getGoldDelta().ToString();
    }
}
