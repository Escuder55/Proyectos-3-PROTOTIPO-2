﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SendToLevel1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("TestTutorialPart1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
