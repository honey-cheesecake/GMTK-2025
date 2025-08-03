using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.XR.CoreUtils;
//using static UnityEditor.PlayerSettings;
using System.Drawing;

public class RopeManager : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] float length;
    [SerializeField][Range(2, 100)] int numNodes;
    //[SerializeField][Range(0f, 1f)] float friction;

    [Header("Visuals")]
    [SerializeField][Range(0f, 1f)] float liftStart;
    [SerializeField][Range(0f, 1f)] float liftMin;
    [SerializeField][Range(0f, 1f)] float liftMax;
    [SerializeField] LineRenderer ropeRenderer;
    [SerializeField] LineRenderer shadowRenderer;

    [Header("References")]
    [SerializeField] Camera cam;
    [SerializeField] CatManager catManager;

    Vector3[] positions;
    Vector3[] prevPositions;
    InputAction mousePosAction;

    // Physics cache
    Vector3 aabbMin;
    Vector3 aabbMax;
    List<Vector3> loopPositions; // subset of positions with XZ swizzle. empty if no loop present.


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        positions = new Vector3[numNodes];
        prevPositions = new Vector3[numNodes];
        ropeRenderer.positionCount = numNodes;
        shadowRenderer.positionCount = numNodes;

        loopPositions = new();

        mousePosAction = InputSystem.actions.FindAction("MousePos");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRope();
        PrepareCollision();
        HandleCats();
        UpdateVisuals();
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

    void UpdateVisuals()
    {
        shadowRenderer.SetPositions(positions);

        Vector3[] visualPositions = new Vector3[numNodes];
        for (int i = 0; i < numNodes; i++)
        {
            visualPositions[i] = positions[i];
            if (i >= (int)(numNodes * liftStart))
            {
                visualPositions[i] += Vector3.up * Remap(i, numNodes * liftStart, numNodes, liftMin, liftMax);
            }
            else
            {
                visualPositions[i] += Vector3.up * liftMin;
            }
        }
        ropeRenderer.SetPositions(visualPositions);
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    void AABB(ref Vector3 min, ref Vector3 max)
    {
        min = positions[0];
        max = positions[0];
        foreach (var pos in positions)
        {
            min = Vector3.Min(min, pos);
            max = Vector3.Max(max, pos);
        }
    }

    void PrepareCollision()
    {
        // construct a new sublit with proper swizzle -----------------------------------------
        loopPositions.Clear();
        int small = 0;
        int big = 0;
        if (!FindClosedLoop(out small, out big))
        {
            // no loop; nothing to do
            return;
        }

        for (int i = small; i <= big; i++)
        {
            loopPositions.Add(XYtoXZ(positions[i]));
        }

        // find bounding box -----------------------------------------
        AABB(ref aabbMin, ref aabbMax);
        aabbMin = XYtoXZ(aabbMin);
        aabbMax = XYtoXZ(aabbMax);
    }
    void HandleCats()
    {
        if (loopPositions.Count() == 0)
        {
            // no loop; nothing to do
            return;
        }

        List<CatAI> cats = catManager.getCatchableCats();
        //List<Vector3> cats = new();
        //for (int x = -10; x < 10; x++)
        //{
        //    for (int y = -10; y < 10; y++)
        //    {
        //        cats.Add(new Vector3(x, y, 0));
        //    }
        //}

        // calling cat.OnEncircled modifies the catchablecats list while we're iterating, which is undefined behaviour.
        // we collect the list of cats to notify, and call them after the loop
        List<CatAI> catsToNotify = new();
        foreach (var cat in cats)
        {
            Vector3 catPos = XYtoXZ(cat.transform.position);
            //Vector3 catPos = XYtoXZ(cat);
            if (catPos.x < aabbMin.x || catPos.x > aabbMax.x || catPos.z < aabbMin.z || catPos.z > aabbMax.z)
            {
                // coarse AABB check failed
                //Debug.DrawLine(cat, cat + new Vector3(0.5f, 0.5f, 0), Color.rebeccaPurple);
                continue;
            }
            if (GeometryUtils.PointInPolygon(catPos, loopPositions))
            {
                catsToNotify.Add(cat);
                //Debug.DrawLine(cat, cat + new Vector3(0.5f, 0.5f, 0), Color.red);
            }
            else
            {
                //Debug.DrawLine(cat, cat + new Vector3(0.5f, 0.5f, 0), Color.yellow);
            }
        }
        foreach (var cat in catsToNotify)
        {
            cat.OnEncircled();
        }
    }

    Vector3 XYtoXZ(Vector3 v)
    {
        // change swizzle
        return new Vector3(v.x, 0, v.y);
    }

    // segment 1: a-b
    // segment 2: c-d
    bool DoSegmentsIntersect(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        return (Area2(a, b, c) * Area2(a, b, d) < 0) && (Area2(c, d, a) * Area2(c, d, b) < 0);
    }

    // https://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect
    // points "a" and "b" forms the anchored segment.
    // point "c" is the evaluated point
    bool IsOnLeft(Vector3 a, Vector3 b, Vector3 c)
    {
        return Area2(a, b, c) > 0;
    }

    bool IsOnRight(Vector3 a, Vector3 b, Vector3 c)
    {
        return Area2(a, b, c) < 0;
    }

    bool IsCollinear(Vector3 a, Vector3 b, Vector3 c)
    {
        return Area2(a, b, c) == 0;
    }

    // calculates the triangle's size (formed by the "anchor" segment and additional point)
    float Area2(Vector3 a, Vector3 b, Vector3 c)
    {
        return (b.x - a.x) * (c.y - a.y) -
               (c.x - a.x) * (b.y - a.y);
    }

    // returns the indices which form a polygon
    bool FindClosedLoop(out int small, out int big)
    {
        // forward
        for (int i = 1; i < numNodes; i++)
        {
            // backwards
            for (int j = numNodes - 1; j > i + 1; j--)
            {
                if (DoSegmentsIntersect(positions[i], positions[i - 1],
                    positions[j], positions[j - 1]))
                {
                    Debug.DrawLine(positions[i], positions[i - 1]);
                    Debug.DrawLine(positions[j], positions[j - 1]);
                    small = i;
                    big = j;
                    return true;
                }
            }
        }
        small = -1;
        big = -1;
        return false;
    }

    public bool OverlapCircle(Vector3 center, float radius)
    {
        for (int i = 0; i < numNodes - 1; i++)
        {
            //GeometryUtils.ClosestPointOnLineSegment(point, positions[i], positions[i + 1]);
            if (Vector3.Distance(positions[i], center) <= radius)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        if (positions == null)
        {
            return;
        }

        foreach (var pos in positions)
        {
            Gizmos.DrawSphere(pos, 0.1f);
        }
    }
}
