﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using InControl;
public class openDoorTutorialpt2 : MonoBehaviour
{
    bool canPlace = false;
    [SerializeField] GameObject dor;
    [SerializeField] int index;
    public GameObject placePosition;
    GameObject skull = null;

    public bool isActivated = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (skull != null)
        {
            if (canPlace && skull.GetComponent<DragAndDrop>().objectIsGrabbed && !isActivated && InputManager.ActiveDevice.Action3.WasPressed)
            {
                dor.SendMessage("Solved", index);
                isActivated = true;
            }
        }


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPlace = true;
        }
        else if (other.CompareTag("Place"))
        {
            skull = other.transform.gameObject;

        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Place"))
        {
            skull = other.transform.gameObject;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPlace = false;
        }
        else if (other.CompareTag("Place"))
        {
            other.gameObject.transform.GetComponent<DragAndDrop>().cancelledDrop = (false);
            skull = null;
        }

    }
}
