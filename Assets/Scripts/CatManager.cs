using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CatManager : MonoBehaviour
{
    [SerializeField] RopeManager ropeManager;
    [SerializeField] Camera cam;

    InputAction mousePosAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mousePosAction = InputSystem.actions.FindAction("MousePos");

        List<CatAI> cats = getAllCats();
        Debug.Log("total number of cats: " + cats.Count);

        foreach (CatAI cat in cats)
        {
            cat.Setup(this);
            Debug.Log("Cat Name and cat Pos: " + cat.name + " " + cat.transform.position);
        }

    }

    // Update is called once per frame  cat.name cat.name
    void Update()
    {
        
    }

    public List<CatAI> getAllCats()
    {
        //finds everything with "Cat" tag and makes a list for these "Cat" tagged objects
        GameObject[] cats = GameObject.FindGameObjectsWithTag("Cat");
        List<CatAI> catList = new List<CatAI>();

        //Makes list of "Cat" tagged objects 
        foreach (GameObject cat in cats)
        {
            CatAI catAI = cat.GetComponent<CatAI>();
            if (catAI != null)
            {
                catList.Add(catAI);
            }
        }

        //returs list of "Cat" tagged objects
        return catList;
    
    }

    public Vector3 GetRandomPointInCamera()
    {
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        Vector3 bottom_left = cam.transform.position - new Vector3(width, height, 0) / 2;
        bottom_left.z = 0;

        return bottom_left + new Vector3(Random.Range(0, width), Random.Range(0, height), 0);
    }

    public Vector3 MousePosition()
    {
        Vector3 mousePos = mousePosAction.ReadValue<Vector2>();
        mousePos = cam.ScreenToWorldPoint(mousePos);
        mousePos.z = 0;

        return mousePos;
    }

    public bool CircleCastRope(Vector3 origin, Vector3 target, float radius)
    {
        Vector3 step = (target - origin).normalized * radius;

        int numIter = (int)(Vector3.Distance(target, origin) / radius);
        for (int i = 0; i < numIter; i++)
        {
            Vector3 samplePos = origin + step * i;
            if (ropeManager.OverlapCircle(samplePos, radius))
            {
                return true;
            }
        }

        if (ropeManager.OverlapCircle(target, radius))
        {
            return true;
        }

        return false;
    }
}
