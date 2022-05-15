using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemChoiceID : MonoBehaviour
{
    [SerializeField] int slot_ID = 0;
    [SerializeField] int ID = 0;

    public void setID(int _id) { ID = _id; }
    public int getID() { return ID; }

    private void OnEnable()
    {
        int rnd = Random.Range(1, ShuffleManager.Instance.getSpCount());
        ID = rnd;
        this.GetComponent<Image>().sprite = ShuffleManager.Instance.setSprite(ID);
        ShuffleManager.Instance.ID_List[slot_ID] = ID;
    }
}
