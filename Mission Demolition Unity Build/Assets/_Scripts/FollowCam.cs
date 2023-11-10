using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    static private FollowCam S;
    static public GameObject POI; //the static point of interest

    public enum eView { none, slingshot, castle, both };


    [Header("Inscribed")]
    public float easing = 0.05f;
    public Vector2 minXY = Vector2.zero;
    public GameObject viewBothGO;

    [Header("Dynamic")]
    public float camZ; //the desired Z pos of the camera
    public eView nextView = eView.slingshot;

    void Awake()
    {
        S = this;

        camZ = this.transform.position.z;
    }

    void FixedUpdate()
    {
        Vector3 destination = Vector3.zero;

        if (POI != null)
        {
            //if the POI has a Rigidbody, check to see if it is sleeping
            Rigidbody poiRigid = POI.GetComponent<Rigidbody>();
            if ((poiRigid != null) && poiRigid.IsSleeping())
            {
                POI = null;
            }
        }

        if (POI != null)
        {
            destination = POI.transform.position;
        }

        //limit the minimum values of destination.x and destination.y
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);

        //interpolate from the current Camera position towards the destination
        destination = Vector3.Lerp(transform.position, destination, easing);

        //force destination.z to be camZ to keep the camera far enough away
        destination.z = camZ;

        //set the camera to the destination
        transform.position = destination;

        //set the orthographicSize of the Camera to keep Ground in view
        Camera.main.orthographicSize = destination.y + 10;
    }

    public void SwitchView(eView newView)
    {
        if (newView == eView.none)
        {
            newView = nextView;
        }
        switch (newView)
        {
            case eView.slingshot:
                POI = null;
                nextView = eView.castle;
                break;

            case eView.castle:
                POI = MissionDemolition.GET_CASTLE();

                nextView = eView.both;
                break;

            case eView.both:
                POI = viewBothGO;
                nextView = eView.slingshot;
                break;
        }
    }

    public void SwitchView()
    {
        SwitchView(eView.none);
    }

    static public void SWITCH_VIEW(eView newView)
    {
        S.SwitchView(newView);
    }
}
