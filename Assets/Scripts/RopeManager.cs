using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class RopeManager : MonoBehaviour
{
    [SerializeField] float length;
    [SerializeField][UnityEngine.Range(2, 100)] int numNodes;
    [SerializeField][UnityEngine.Range(1, 10)] int constraintSteps;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Camera cam;
    [SerializeField] CatManager catManager;

    Vector3[] positions;
    InputAction mousePosAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        positions = new Vector3[numNodes];
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

        // Move the tip of the rope
        positions[numNodes - 1] = mousePos;

        for (int i = 0; i < constraintSteps; i++)
        {
            for (int edgeIdx = 0; edgeIdx < numNodes - 1; edgeIdx++)
            {
                int nextIdx = edgeIdx + 1;
                int currIdx = edgeIdx;

                Vector3 nextPos = positions[nextIdx];
                Vector3 currPos = positions[currIdx];

                float targetLength = length / numNodes;
                float actualLength = Vector2.Distance(nextPos, currPos);
                float lengthDelta = targetLength - actualLength;
                Vector3 stickDir = (nextPos - currPos).normalized;

                //if (currIdx != 0)
                {
                    positions[currIdx] -= stickDir * lengthDelta / 2;
                }
                if (nextIdx != numNodes - 1)
                {
                    positions[nextIdx] += stickDir * lengthDelta / 2;
                }
            }
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
