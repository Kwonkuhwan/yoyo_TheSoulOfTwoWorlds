using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTigger : MonoBehaviour
{
    [SerializeField] int targetLayerInt;

    [SerializeField] ControllerHand controllerHand = ControllerHand.None;
    [SerializeField] Camera handCamera;
    InputBridge input;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float maxDistance = 1.0f;

    private void Awake()
    {
        targetLayerInt = LayerMask.NameToLayer("DoorOpenColl");
        input = InputBridge.Instance;
    }

    private void Update()
    {
        RaycastCheck();

        Debug.DrawRay(handCamera.transform.position,
              handCamera.transform.TransformDirection(Vector3.forward) * maxDistance,
              Color.red);
    }

    void RaycastCheck()
    {
        if (input.RightGripDown || input.LeftGripDown)
        {
            if (controllerHand == ControllerHand.None) return;
            if (controllerHand == ControllerHand.Left && input.RightGripDown) return;
            if (controllerHand == ControllerHand.Right && input.LeftGripDown) return;

            RaycastHit hit;
            if (Physics.Raycast(handCamera.transform.position, handCamera.transform.TransformDirection(Vector3.forward), out hit, maxDistance))
            {
                if (hit.collider.gameObject.GetComponent<SimpleOpenClose>())
                {
                    Debug.Log("Object with SimpleOpenClose script found");
                    hit.collider.gameObject.BroadcastMessage("ObjectClicked");
                }

                else
                {
                     Debug.Log("Object doesn't have script SimpleOpenClose attached");

                }
                // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                // Debug.Log("Did Hit");
            }
            else
            {
                // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                //   Debug.Log("Did not Hit");


            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == targetLayerInt)
        {
            Debug.Log("OnCollisionStay");
        }
    }
}

