using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")]   //this shows up in the Inspector pane, contains fields meant to be set within the Inspector
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;

    [Header("Dynamic")] //this shows up in the Inspector pane, set dynamically when the game is running
    public GameObject launchPoint;
    public Vector3 launchPos; //stores the 3D world position of launchPoint
    public GameObject projectile;   //reference to the new Projectile instance that is created
    public bool aimingMode;


    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint"); //searches for a child of Slingshot named LaunchPoint and returns its Transform
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }

    void OnMouseEnter()
    {
        launchPoint.SetActive(true);    //when the mouse enters the object, the halo will activate
    }

    void OnMouseExit()
    {
        launchPoint.SetActive(false); //when the mouse exits the object, the halo will deactivate
    }

    void OnMouseDown()
    {
        
        aimingMode = true;  //the player has pressed the mouse button while over the slingshot
        projectile = Instantiate(projectilePrefab) as GameObject; //instantiate a projectile
        projectile.transform.position = launchPos;  //start it at the launchPoint
        projectile.GetComponent<Rigidbody>().isKinematic = true;    //set it to isKinematic for now
    }

    void Update()
    {
        //if Slingshot is not in aimingMode, don't run this code
        if (!aimingMode) return;

        //get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        //find the delta from the launchPos to the mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;

        //limit mouseDelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        //move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;
        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            //lets the projectile fly through the air
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);

            //set the _MainCamera POI
            FollowCam.POI = projectile;

            Instantiate<GameObject>(projLinePrefab, projectile.transform);

            //severs the connection between this instance of the Slingshot script and the projectile GameObject
            projectile = null;
            MissionDemolition.SHOT_FIRED();
        }
    }
}
