﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whip : MonoBehaviour
{
    [SerializeField] LineRenderer whip;
    [SerializeField] Transform playerTransform;
    Vector3 newPlayerPos;
    Transform whipableJumpObjectTransform;
    Transform destinationTrasnform;
    [SerializeField] float lineDrawSpeed;
    float distToWhipable;
    float distToDestination;
    float counter = 0;
    float curveCounter = 0;
    float time = 0;
    bool inputDown = false;
    bool ableToWhipJump = false;
     bool whippin = false;
    // Start is called before the first frame update
    void Start()
    {
        whip.SetPosition(0, playerTransform.position);
        whip.SetPosition(1, playerTransform.position);
        whip.startWidth = (0.2f);
        whip.endWidth = (0.2f);
    }

    // Update is called once per frame
    void Update()
    {

        #region WHIP UPDATE
        whip.SetPosition(0, playerTransform.position);
        whip.SetPosition(1, playerTransform.position);
        if (counter < distToWhipable && inputDown && ableToWhipJump)
        {
            time += Time.deltaTime;
            counter += .1f / lineDrawSpeed;
            float x = Mathf.Lerp(0, distToWhipable, counter);
            Vector3 pA = playerTransform.position;
            Vector3 pB = whipableJumpObjectTransform.position;

            Vector3 pointBetweenAandB = x * Vector3.Normalize(pB - pA) + pA;
            whip.SetPosition(1, pointBetweenAandB);
        }
        #endregion

        #region PLAYER WHIPJUMP
        if (time >= lineDrawSpeed / 4 && ableToWhipJump)
        {
            whip.SetPosition(1, whipableJumpObjectTransform.position);
            float x = Mathf.Lerp(0, distToDestination, Time.deltaTime);
            Vector3 pA = playerTransform.position;
            Vector3 pB = destinationTrasnform.position;
            newPlayerPos = x * Vector3.Normalize(pB - pA) + pA;

            /*whip.SetPosition(1, whipableJumpObjectTransform.position);
            Vector3 startingPoint = playerTransform.position;
            Vector3 middlePoint = whipableJumpObjectTransform.position;
            Vector3 endPoint = destinationTrasnform.position;
            newPlayerPos = calculateBezierCurve(Time.deltaTime, startingPoint, endPoint , middlePoint);*/
            whippin = true;
        }
        if(ableToWhipJump)
            if(Vector3.Distance(playerTransform.position, destinationTrasnform.position) < 1)
            {
                whippin = false;
            }
        #endregion

        #region INPUT CONTROL
        if (Input.GetKeyDown(KeyCode.C) && ableToWhipJump)
        {
            whipableJumpObjectTransform.SendMessage("ChangeState");
            inputDown = true;
        }
        if (Input.GetKeyUp(KeyCode.C) )
        {
            whippin = false;
            inputDown = false;
            whip.SetPosition(1, playerTransform.position);
            counter = 0;
            time = 0;
        }
        #endregion

    }

    private void FixedUpdate()
    {
        if (whippin)
        {
            playerTransform.position = newPlayerPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "WhipJump")
        {
            other.SendMessage("SetPlayerTransform", playerTransform);
            ableToWhipJump = true;
            distToWhipable = Vector3.Distance(playerTransform.position, whipableJumpObjectTransform.position);
            distToDestination = Vector3.Distance(playerTransform.position, destinationTrasnform.position);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "WhipJump")
        {
            ableToWhipJump = false;
            inputDown = false;
            whippin = false;
            whipableJumpObjectTransform = null;
        }
    }

    public void setDestinationTransform(Transform transform)
    {
        destinationTrasnform = transform;
    }

    public void setWhipableJumpObjectTransform(Transform transform)
    {
        whipableJumpObjectTransform = transform;
    }

    Vector3 calculateBezierCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 newP = uu * p0;
        newP += 2 * u * t * p1;
        newP += tt * p2;
        return newP;

        //return p1 + Mathf.Sqrt(1 - t) * (p0 - p1) + Mathf.Sqrt(t) * (p2 - p1);
    }
}