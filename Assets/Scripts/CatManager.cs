using UnityEngine;
using System.Collections.Generic;
using System.Numerics;

public class CatManager : MonoBehaviour
{
    [SerializeField] RopeManager ropeManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<CatAI> getAllCats()
    {
        return new();
    }
}
