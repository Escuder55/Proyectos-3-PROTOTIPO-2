﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PickUpandDrop : PickUp
{
    protected Animator playerAnimator;
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        startingRotation = transform.localRotation;


        playerMovement = GameObject.Find("Character").GetComponent<PlayerMovement>();
        objectIsGrabbed = false;
        grabPlace = GameObject.Find("Hand_R_PickUp");
    } 

    // Update is called once per frame

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            distanceChecker = player.transform.GetChild(1).gameObject;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            playerAnimator = player.GetComponentInChildren<Animator>();
        }
        if (other.CompareTag("Death") && !transform.CompareTag("Stone"))
        {
            ResetPosition();
        }
    }
    protected void ObjectDrop()
    {
        playerMovement.ableToWhip = true;
        transform.SetParent(null);
        objectIsGrabbed = false;
    }
    public void DropObject()
    {
        playerMovement.ableToWhip = true;
        player.SendMessage("StopMovement", true);
        StartCoroutine(DropObjectCoroutine(0.5f));
        StartCoroutine(AnimationsCoroutine(0.5f));
    }
    public bool GetObjectIsGrabbed()
    {
        return objectIsGrabbed;
    }

    protected IEnumerator AnimationsCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        player.SendMessage("StopMovement", false);
        playerAnimator.SetBool("PickUp", false);
        playerAnimator.SetBool("DropObject", false);
        playerAnimator.SetBool("PlaceObject", false);

    }

    protected IEnumerator PickUpCoroutine(float time)
    {
        DropObject();
        yield return new WaitForSeconds(time);
        playerMovement.ableToWhip = false;
        PickUpObject();
    }
    protected IEnumerator DropObjectCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        playerMovement.ableToWhip = true;
        ObjectDrop();
    }
}