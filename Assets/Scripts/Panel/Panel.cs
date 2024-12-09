using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public void ChangePanelState(bool state)
    {
        gameObject.SetActive(state);
    }
}
