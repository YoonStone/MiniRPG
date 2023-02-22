using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodUse : Slot
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClickThis);        
    }

    void OnClickThis()
    {

    }
}
