﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSlingShot : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject prefabProjectile;
    public float velocityMult = 8f;
    
    [Header("Set Dynamically")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    private Rigidbody projectileRigidbody;

    void Awake()
    {
        Transform launchPointTrans = transform.FindChild("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }
    void OnMouseEnter()
    {
        //print("Slingshot: OnMouseEnter()");
        launchPoint.SetActive(true);
    }

    void OnMouseExit()
    { 
        //print("Slingshot: OnMouseExit()");
        launchPoint.SetActive(false);
    }

    void OnMouseDown()
    {
        //The player has pressed the mouse button while over SlingShot
        aimingMode = true;
        //Instanitte a Projectile
        projectile = Instantiate(prefabProjectile) as GameObject;
        //Start it at the launchPoint
        projectile.transform.position = launchPos;
        //Set it to isKinematic for now
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.isKinematic = true;

    }

    void Update()
    {
        //If SlingShot is not in aimingMod, don't run this code
        if (!aimingMode) return;

        //Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        //Find the delta from the launchPos to the mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;
        //Limit mouseDelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if(mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        //Move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        if(Input.GetMouseButtonUp(0))
            //The mouse has been released
        {
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile;
            projectile = null;
        }
    }
}
