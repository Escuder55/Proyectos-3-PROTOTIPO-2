﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.Animations;

public class PickUpDragandDrop : PickUpandDrop
{
    [SerializeField] float grabPointHeight = 0.5f;
    [SerializeField] float stoneSpeed = 5;
    [SerializeField] Animator animator;
    [HideInInspector] public bool playSound = false;
    [SerializeField] CharacterController playerController;
    [SerializeField] PlayerMovement playerMovement;
    Vector3[] grabPoints = new Vector3[4];
    Vector3 closestPoint;
    int minPoint = -1;[HideInInspector] public Rigidbody rb;
    [SerializeField] bool lerping = false;
    [SerializeField] bool rockGrabbed = false;
    bool lockVertical, lockHorizontal = false;
    bool thisRock = false;
    [HideInInspector] public AudioSource dragSound;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        try
        {
            dragSound = GameObject.Find("Drag sound").GetComponent<AudioSource>();
        }
        catch
        {
            Debug.LogWarning("Add global sounds");
        }
    }

    // Update is called once per frame
    void Update()
    {

        CheckVariables();
        if (Input.GetButtonDown("Interact") && isFacingBox && !cancelledDrop && playerController.isGrounded)
        {
            if (!objectIsGrabbed)
            {
                PickUpObject();
                playerMovement.grabbedToRock = true;
            }
            else
            {
                animator.SetBool("Attached", false);
                playerMovement.grabbedToRock = false;
                DropObject();
            }
        }
        else if (lerping)
        {
            animator.SetBool("Attached", true);
            DoLerp();
        }
        else if (rockGrabbed)
        {
            thisRock = true;
            player.transform.position = closestPoint;
            float horizontalMove = Input.GetAxis("Horizontal");
            float verticalMove = Input.GetAxis("Vertical");

            bool inputActive = horizontalMove != 0 || verticalMove != 0;

            if (closestPoint == grabPoints[0])
            {
                Vector3 movingDirection = grabPoints[1] - player.transform.position;
                if (Vector3.Angle(movingDirection, player.GetComponent<PlayerMovement>().movePlayer) <= 30 && inputActive)
                {
                    PushRock(movingDirection);
                }
                else if (Vector3.Angle(-movingDirection, player.GetComponent<PlayerMovement>().movePlayer) < 30 && inputActive)
                {
                    PullRock(movingDirection);
                }
                else
                {
                    other();
                }
            }
            else if (closestPoint == grabPoints[1])
            {
                Vector3 movingDirection = grabPoints[0] - player.transform.position;

                if (Vector3.Angle(movingDirection, player.GetComponent<PlayerMovement>().movePlayer) <= 30 && inputActive)
                {
                    PushRock(movingDirection);
                }
                else if (Vector3.Angle(-movingDirection, player.GetComponent<PlayerMovement>().movePlayer) < 30 && inputActive)
                {
                    PullRock(movingDirection);
                }
                else
                {
                    other();
                }
            }
            else if (closestPoint == grabPoints[2])
            {
                Vector3 movingDirection = grabPoints[3] - player.transform.position;

                if (Vector3.Angle(movingDirection, player.GetComponent<PlayerMovement>().movePlayer) <= 30 && inputActive)
                {
                    PushRock(movingDirection);
                }
                else if (Vector3.Angle(-movingDirection, player.GetComponent<PlayerMovement>().movePlayer) < 30 && inputActive)
                {
                    PullRock(movingDirection);
                }
                else
                {
                    other();
                }
            }
            else if (closestPoint == grabPoints[3])
            {
                Vector3 movingDirection = grabPoints[2] - player.transform.position;

                if (Vector3.Angle(movingDirection, player.GetComponent<PlayerMovement>().movePlayer) <= 30 && inputActive)
                {
                    PushRock(movingDirection);
                }
                else if (Vector3.Angle(-movingDirection, player.GetComponent<PlayerMovement>().movePlayer) < 30 && inputActive)
                {
                    PullRock(movingDirection);
                }
                else
                {
                    other();
                }
            }
        }
        else if(!rockGrabbed && player != null)
        {
            playerMovement.grabbedToRock = false;
            animator.SetBool("Attached", false);
        }
        else
            thisRock = false;

        if (playSound && !dragSound.isPlaying && thisRock)
        {
            dragSound.Play();
        }
        else if (!playSound && thisRock)
        {
            dragSound.Stop();
        }

        if (GetComponent<Animation>().isPlaying)
        {
            //player.GetComponent<PlayerMovement>().StopMovement(false);
            playSound = false;
            dragSound.Stop();
            lerping = false;
            rockGrabbed = false;
            thisRock = false;
        }
    }

    protected override void PickUpObject()
    {
        if (!rockGrabbed)
        {
            //FIND CLOSEST POINT
            closestPoint = FindClosestPoint();
            player.transform.DOMove(closestPoint, 0.5f, false);
            //LERP PLAYER TOWARDS POINT
            lerping = true;
            playSound = false;
            dragSound.Stop();
        }
        else
        {
            //Let go of rock
            player.GetComponent<PlayerMovement>().StopMovement(false);
            lerping = false;
            rockGrabbed = false;
            playSound = false;
            dragSound.Stop();
        }
    }
    void PushRock(Vector3 movingDirection)
    {
        animator.SetBool("Push", true);
        rb.velocity = movingDirection.normalized * stoneSpeed;
        playSound = true;
    }
    void other()
    {
        animator.SetBool("Push", false);
        animator.SetBool("Pulling", false);
        playSound = false;
    }
    void PullRock(Vector3 movingDirection)
    {
        animator.SetBool("Pulling", true);
        rb.velocity = -movingDirection.normalized * stoneSpeed;
        playSound = true;
    }
    protected override void CheckVariables()
    {
        //calculate the grab points every frame in case the rock moves
        grabPoints[0] = transform.position + transform.forward * 3;
        grabPoints[1] = transform.position - transform.forward * 3;
        grabPoints[2] = transform.position + transform.right * 3;
        grabPoints[3] = transform.position - transform.right * 3;

        //put the points in the desired height
        grabPoints[0].y = transform.position.y - grabPointHeight;
        grabPoints[1].y = transform.position.y - grabPointHeight;
        grabPoints[2].y = transform.position.y - grabPointHeight;
        grabPoints[3].y = transform.position.y - grabPointHeight;

        //if minpoint isn't null, grab the closest point to the player
        if (minPoint != -1)
            closestPoint = grabPoints[minPoint];
        if (player != null)
        {
            isFacingBox = false;
            //checks if the distance from the player to the rock is close enough
            distancePlayerObject = Vector3.Distance(player.transform.position, transform.position);
            //checks if the player is staring in the direction of the rock
            Vector2 playerForward2d = new Vector2(player.transform.forward.x, player.transform.forward.z);
            Vector2 dirToObject2d = new Vector2(transform.position.x - player.transform.position.x, transform.position.z - player.transform.position.z);
            //we do the dot product of X and Z, to ignore the Y in case the object is placed above or below
            float dot = Vector3.Dot(playerForward2d, dirToObject2d);
            if (dot > 0.5f) { isFacingBox = true; }
        }
    }
    public void LetGoRock()
    {
        animator.SetBool("Attached", false);
        animator.SetBool("Push", false);
        animator.SetBool("Pulling", false);
        playerMovement.grabbedToRock = false;
        playSound = false;
        dragSound.Stop();
        lerping = false;
        rockGrabbed = false;
        thisRock = false;
    }
    void DoLerp()
    {
        player.GetComponent<PlayerMovement>().StopMovement(true);
        //lerp player towards closest point
        //player.transform.position = Vector3.Lerp(player.transform.position, closestPoint, 0.1f);


        //lerp rotation to face object
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - player.transform.position);
        player.transform.rotation = (targetRotation);
        
        if (Vector3.Distance(player.transform.position, closestPoint) < 2f)
        {
            //stop lerping and look at object
            lerping = false;
            rockGrabbed = true;
        }
    }

    Vector3 FindClosestPoint()
    {
        //FINDS CLOSEST POINT
        float minDistance = -1;
        for (int i = 0; i < 4; i++)
        {
            float dist = Vector3.Distance(player.transform.position, grabPoints[i]);
            if (minDistance == -1 || dist < minDistance)
            {
                minPoint = i;
                minDistance = dist;
            }
        }

        return grabPoints[minPoint];

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("Attached", false);
            player = null;
            thisRock = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistanceToGrabObject);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(grabPoints[0], 0.5f);
        Gizmos.DrawSphere(grabPoints[1], 0.5f);
        Gizmos.DrawSphere(grabPoints[2], 0.5f);
        Gizmos.DrawSphere(grabPoints[3], 0.5f);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10);
    }
}
