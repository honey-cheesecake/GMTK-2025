using UnityEngine;
using System.Collections.Generic;

public class CatManager : MonoBehaviour
{
    [SerializeField] RopeManager ropeManager;
    [SerializeField] Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<CatAI> cats = getAllCats();
        Debug.Log("total number of cats: " + cats.Count);

        foreach (CatAI cat in cats)
        {
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

    public Vector3 GetPointInCamera()
    {
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        Vector3 bottom_left = cam.transform.position - new Vector3(width, height, 0) / 2;

        return bottom_left + new Vector3(Random.Range(0, width), Random.Range(0, height), 0);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    float height = 2f * cam.orthographicSize;
    //    float width = height * cam.aspect;


    //    Vector3 bl = cam.transform.position - new Vector3(width, height, 0) / 2;
    //    Vector3 tr = cam.transform.position + new Vector3(width, height, 0) / 2;

    //    //Gizmos.DrawWireCube(cam.transform.position, new Vector3(width, height, 0));
    //    Gizmos.DrawLine(bl, tr);
    //    //Gizmos.DrawLine(cam.transform.position);
    //}
}
