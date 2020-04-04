﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlatformMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Vector3 position1, position2;
    [SerializeField] float speed;
    [SerializeField] bool NeverStop = false;
    [SerializeField] bool isActive = false;
    Rigidbody rb;
    bool toPos2 = true;
    bool hasBeenActivated = false;
    GameObject player;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Character");
    }

    void Update()
    {
        //Use this to check if the platform has reached its destination

        //If the platform has reached position 2, set position1 as new destination
        //If the platform never stops, just make the platform go to position 1 automatically
        //If the platform does stop when reaching the end, just set velocity to zero
        if (NeverStop && isActive)
        {
            //When reaches the destination, sends to the other destination
            if (toPos2 && CheckHasReachedDestination(position2))
            {
                toPos2 = false;
                isActive = true;
                MovePlatform();
            }
            else if (!toPos2 && CheckHasReachedDestination(position1))
            {
                toPos2 = true;
                isActive = true;
                MovePlatform();
            }
        }
        else
        {
            //When reaches the destination, stops
            if (toPos2 && CheckHasReachedDestination(position2))
            {
                toPos2 = false;
                isActive = false;
                StopPlatform();
            }
            else if (!toPos2 && CheckHasReachedDestination(position1))
            {
                toPos2 = true;
                isActive = false;
                StopPlatform();
            }
        }

        float distanceToPlatform = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlatform <2.8f)
        {
            player.transform.SetParent(transform);
            Debug.Log("parented!");
        }
        else if (distanceToPlatform >= 2.8f && distanceToPlatform < 3.5f)
        {
            player.transform.SetParent(null);
            Debug.Log("unparented!");
        }

    }

    bool CheckHasReachedDestination(Vector3 destination)
    {
        return (transform.position == destination || Vector3.Distance(transform.position, destination) < 0.5);
    }

    void StopPlatform()
    {
        //If it's not activated, leave the velocity at zero so the platform is stopped
        //rb.velocity = Vector3.zero;
        transform.DOKill();
    }

    void MovePlatform()
    {
        //Vector3 dir;
        Vector3 toPos;
        if (toPos2)
        {
            //dir = position2 - transform.position;
            toPos = position2;
        }
        else
        {
            //dir = position1 - transform.position;
            toPos = position1;
        }
        //dir.Normalize();
        transform.DOMove(toPos, speed, false).SetEase(Ease.Linear);
        //rb.velocity = dir * speed;
    }

    public void ActivateObject()
    {
        //When the function is called from the lever, check if the platform is activated or not
        isActive = isActive ? false : true;

        //If it's activated, decide which direction it has to take
        if(isActive)
        {
            MovePlatform();
        }
        else
        {
            //If it's not activated, leave the velocity at zero so the platform is stopped
            StopPlatform();
        }
    }
}
