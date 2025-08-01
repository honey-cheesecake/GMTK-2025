using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class RopeManager : MonoBehaviour
{
    [SerializeField] float length;
    [SerializeField][Range(2, 100)] int numNodes;
    //[SerializeField][Range(0f, 1f)] float friction;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Camera cam;
    [SerializeField] CatManager catManager;

    Vector3[] positions;
    Vector3[] prevPositions;
    InputAction mousePosAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        positions = new Vector3[numNodes];
        prevPositions = new Vector3[numNodes];
        lineRenderer.positionCount = numNodes;

        mousePosAction = InputSystem.actions.FindAction("MousePos");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRope();
        lineRenderer.SetPositions(positions);

        // catManager.getAllCats()
        // foreach
        // {
        // if encircled
        // cat.OnEncircled()
        // }
    }

    void UpdateRope()
    {
        Vector3 mousePos = mousePosAction.ReadValue<Vector2>();
        mousePos = cam.ScreenToWorldPoint(mousePos);
        mousePos.z = 0;

        //for (int i = numNodes - 2; i >= 0; i--)
        //{
        //    // retain current velocity
        //    Vector3 vel = positions[i] - prevPositions[i]; // velocity of current node
        //    vel *= friction;
        //    newPositions[i] = positions[i] + vel;
        //}

        // Move the tip of the rope
        positions[numNodes - 1] = mousePos;

        // constraint
        for (int idx = numNodes - 2; idx >= 0; idx--)
        {
            Vector3 nextPos = positions[idx + 1];
            Vector3 currPos = positions[idx];

            float targetLength = length / numNodes;
            float actualLength = Vector2.Distance(nextPos, currPos);
            if (actualLength > targetLength)
            {
                Vector3 edgeDir = (currPos - nextPos).normalized;
                positions[idx] = nextPos + edgeDir * targetLength;
            }
        }

        // prepare for next frame
        for (int i = 0; i < numNodes; i++)
        {
            prevPositions[i] = positions[i];
        }
    }

    private void OnDrawGizmos()
    {
        if (positions != null)
        {
            foreach (var pos in positions)
            {
                Gizmos.DrawSphere(pos, 0.1f);
            }
        }
    }
}
