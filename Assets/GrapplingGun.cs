using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GrapplingGun : MonoBehaviourPunCallbacks
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappable;
    public Transform gunTip, camera, player;
    private SpringJoint joint;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        
        DrawRope();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartGrapple();
        }else if (Input.GetKeyUp(KeyCode.Q))
        {
            StopGrapple();
        }

        void StartGrapple()
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.position, camera.forward, out hit, 100f, whatIsGrappable))
            {
                grapplePoint = hit.point;
                joint = player.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint;

                float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

                joint.maxDistance = distanceFromPoint * 0.8f;
                joint.minDistance = distanceFromPoint * 0.25f;

                joint.spring = 4.5f;
                joint.damper = 7f;
                joint.massScale = 4.5f;

                lr.positionCount = 2;
            }
        }

        void DrawRope()
        {
            if(!joint) return;
            

            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);
        }

        void StopGrapple()
        {
            lr.positionCount = 0;
            Destroy(joint);
        }
    }
}
