﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Whip : MonoBehaviour
{
    [SerializeField] LineRenderer whip;
    [SerializeField] Transform playerTransform;
    [SerializeField] GameObject spriteIndicateObject;
    [SerializeField] Camera mainCamera;
    [SerializeField] float lineDrawSpeed;
    float timePlus = 0.15f;
    float timeMinus = 0.15f;
    Transform whipableObjectTransform;
    [SerializeField] List<Transform> enemyList = new List<Transform>();
    float distToWhipable;
    float counter = 0;
    float time = 0;
    float oldFOV = 0;
    [SerializeField] bool inputDown = false;
    [SerializeField] bool ableToAttack = false;
    [SerializeField] bool attackMode = false;
    bool ableToWhipObject = false;
    bool whippin = false;
    [SerializeField] int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        whip.SetPosition(0, playerTransform.position);
        whip.SetPosition(1, playerTransform.position);
        whip.startColor = Color.black;
        whip.endColor = Color.black;
        whip.startWidth = (0.2f);
        whip.endWidth = (0.2f);
    }

    // Update is called once per frame
    void Update()
    {

        #region WHIP UPDATE
        if (index > enemyList.Count - 1) index = enemyList.Count - 1;
        whip.SetPosition(0, playerTransform.position);
        if (!whippin)
            whip.SetPosition(1, playerTransform.position);
        if (enemyList.Count == 0)
        {
            index = 0;
            ableToAttack = false;
            attackMode = false;
            oldFOV = mainCamera.fieldOfView;
        }
        else
        {
            ableToAttack = true;
        }

        if (attackMode)
        {
            whipableObjectTransform = enemyList[index];
            mainCamera.DOFieldOfView(60, 1);
            spriteIndicateObject.SetActive(true);
            spriteIndicateObject.transform.position = whipableObjectTransform.position + new Vector3(0, 2.5f, 0);
            distToWhipable = Vector3.Distance(playerTransform.position, enemyList[index].position + new Vector3(0,5,0) );
        }
        
        if (counter < distToWhipable && inputDown && (attackMode || ableToWhipObject))
        {
            time += Time.deltaTime;
            counter += .1f / lineDrawSpeed;
            float x = Mathf.Lerp(0, distToWhipable, counter);
            Vector3 pA = playerTransform.position;
            Vector3 pB = whipableObjectTransform.position + new Vector3(0, 2.5f, 0);
            Vector3 pointBetweenAandB = x * Vector3.Normalize(pB - pA) + pA;
            whip.SetPosition(1, pointBetweenAandB);

        }
        #endregion

        #region PLAYER WHIPATTACK & WHIPOBJECT

        if (time >= lineDrawSpeed/4  && attackMode)
        {
            Debug.Log("enemyDead");
            enemyList[index].SendMessage("Die");
            whip.SetPosition(1, whipableObjectTransform.position + new Vector3(0, 2.5f, 0));
        }
        else if (time >= lineDrawSpeed  && ableToWhipObject)
        {
            whipableObjectTransform.SendMessage("ChangeState");
        }


        #endregion

        #region INPUT CONTROL
        if (Input.GetButtonDown("Whip") && (ableToAttack || ableToWhipObject))
        {
            whippin = true;
            inputDown = true;
        }
        else if (Input.GetButtonUp("Whip") )
        {
            whippin = false;
            inputDown = false;
            whip.SetPosition(1, playerTransform.position);
            counter = 0;
            time = 0;
        }
        if (Input.GetAxis("RightJoystickHorizontal") == 1f && attackMode)
        {
            if (timePlus == 0.15f)
            {
                if (index < enemyList.Count - 1) index++;
                else index = 0;
            }
            timePlus -= Time.deltaTime;
            if (timePlus < 0) timePlus = 0.15f;
        }
        else if (Input.GetAxis("RightJoystickHorizontal") == -1f && attackMode)
        {
            if (timeMinus == 0.15f)
            {
                if (index > 0) index--;
                else index = enemyList.Count - 1;
            }
            timeMinus -= Time.deltaTime;
            if (timeMinus < 0) timeMinus = 0.15f;
        }
        if (ableToAttack && Input.GetButtonDown("EnterCombatMode"))
        {
            if(!attackMode) oldFOV = mainCamera.fieldOfView;
            else
            {
                mainCamera.DOFieldOfView(oldFOV, 1);
                spriteIndicateObject.SetActive(false);
                whipableObjectTransform = null;
            }
            attackMode = !attackMode;
        }

        #endregion

    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "WhipEnemy")
        {
            enemyList.Add(other.gameObject.transform);
        }
        else if (other.tag == "WhipObject")
        {
            other.SendMessage("SetPlayerTransform", playerTransform);
            ableToWhipObject = true;
            distToWhipable = Vector3.Distance(playerTransform.position, whipableObjectTransform.position);
            spriteIndicateObject.SetActive(true);
            spriteIndicateObject.transform.position = whipableObjectTransform.position;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "WhipEnemy")
        {
            enemyList.Remove(other.transform);
            mainCamera.DOFieldOfView(oldFOV, 1);
            spriteIndicateObject.SetActive(false);
            whipableObjectTransform = null;
        }
        else if (other.tag == "WhipObject")
        {
            ableToWhipObject = false;
            whipableObjectTransform = null;
            spriteIndicateObject.SetActive(false);
        }
    }

    public void setWhipableJumpObjectTransform(Transform transform)
    {
        whipableObjectTransform = transform;
    }
}