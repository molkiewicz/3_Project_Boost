﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerPrefs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.DeleteKey("EndingZero");
        PlayerPrefs.DeleteKey("Shield");
        PlayerPrefs.SetInt("Shield",0);
    }

}
