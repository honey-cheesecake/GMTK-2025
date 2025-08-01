using UnityEngine;
using System.Collections.Generic;
using System.Numerics;

public class CatManager : MonoBehaviour
{
    [SerializeField] RopeManager ropeManager;
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
}
