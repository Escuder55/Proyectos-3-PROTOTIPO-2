﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LeverMinecart : MonoBehaviour
{
    // Variables
    [SerializeField] Animator activateObject;
    [SerializeField] GameObject lever;

    bool showCanvas = false;
    public float distanceToLever = 3f;
    GameObject player = null;
    bool leverPulled = false;
    [SerializeField] bool minecartAction = false;
    [SerializeField] bool canPullLeverAlways = false;

    void Start()
    {

    }

    void Update()
    {
        if (player != null)
        {
            var currentDistanceToLever = Vector3.Distance(transform.position, player.transform.position);
            if (showCanvas && (currentDistanceToLever < distanceToLever) && (!leverPulled || canPullLeverAlways) && Input.GetButtonDown("Interact"))
            {
                showCanvas = false;
                leverPulled = true;
                lever.transform.localEulerAngles = new Vector3(lever.transform.localRotation.x, lever.transform.localRotation.y, lever.transform.localEulerAngles.z - 45);
                activateObject.SetBool("Straight", minecartAction);
                minecartAction = !minecartAction;
                //We could have another object attached here, such as a GameObject MortalTrap, and that
                //object has a function activate. That way we could easily do MortalTrap.activate(); from here.
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //If the player enters the lever trigger, we'll show the canvas to press button E
        if (other.CompareTag("Player"))
        {
            showCanvas = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //If the player exits the trigger, we'll hide the canvas
        if (other.CompareTag("Player"))
        {
            showCanvas = false;
            player = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceToLever);
        if (activateObject != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, activateObject.transform.position);
        }
    }
}
