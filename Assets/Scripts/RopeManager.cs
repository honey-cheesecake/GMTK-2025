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
    [SerializeField] [UnityEngine.Range(2,100)] int numNodes;
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

        if (Vector3.Distance(transform.position, mousePos) > length)
        {
            // Too far; straighten as much as possible and give up
            for (int i = 0; i < numNodes; i++)
            {
                positions[i] = (mousePos - transform.position) / numNodes * i;
            }
            return;
        }

        // Move the tip of the rope
        positions[numNodes - 1] = mousePos;
        DoConstraint(numNodes - 2);
    }

    void DoConstraint(int idx)
    {
        if (idx == 0)
        {
            return; // anchor shouldn't move. If we reach here we did something wrong tbh
        }

        float edgeLength = length / numNodes;

        Vector2 nextPos = positions[idx + 1];
        Vector2 currPos = positions[idx];

        if (Vector3.Distance(currPos, nextPos) > edgeLength)
        {
            positions[idx] = nextPos + (currPos - nextPos).normalized * edgeLength;
            DoConstraint(idx - 1);
        }
    }
}
