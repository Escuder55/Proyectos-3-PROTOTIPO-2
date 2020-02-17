﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ActivateAnimation : MonoBehaviour
{
    public enum typeAnimator
    {
        NONE,
        DOOR,
        PLATFORM,
        BRIDGE
    }
    [SerializeField] Animator myAnimator;
    [SerializeField] typeAnimator type;

    [Header("FOR CAMERA SHAKE")]
    [SerializeField] Camera myCamera;
    [SerializeField] float durationShake;
    [SerializeField] float strength;
    [SerializeField] int vibrato;
    [SerializeField] float randomness;
    [SerializeField] Transform objectPos = null;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Skull") && type == typeAnimator.DOOR )
        {
            if (!other.GetComponent<PickUpandDrop>().GetObjectIsGrabbed() && objectPos!=null)
            {
                other.gameObject.transform.position = objectPos.transform.position;
                myAnimator.SetBool("Active", true);
                Invoke("DeactivateAnimation", 2f);
            }
        }
        else if (other.CompareTag("Player") && type == typeAnimator.PLATFORM)
        {
            myAnimator.SetBool("Active", true);
            Invoke("DeactivateAnimation", 2.5f);
        }
        else if (other.CompareTag("Block") && type == typeAnimator.BRIDGE)
        {
            myAnimator.SetBool("Active", true);
            Debug.Log("Camera Shake!!");
            //myCamera.DOShakePosition(durationShake, strength, vibrato, randomness,true);
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            DeactivateAnimation();
            //myCamera.DOShakePosition(durationShake, strength, vibrato, randomness, true);
        }
    }

    void DeactivateAnimation()
    {
        myAnimator.SetBool("Active", false);
    }


}
