using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    void Start()
    {
        List<Dictionary<string, object>> DataList = new List<Dictionary<string, object>>();
        DataList = CSVReader.Read("Data"); //Resource ���� ���� ���, Ȯ���ڸ� �����

        for(int i = 0; i < DataList.Count; ++i)
        {
            Debug.Log(DataList[i]["aa"] + " " + DataList[i]["ee"]);
        }
    }
}
