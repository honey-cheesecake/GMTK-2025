using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CatManager : MonoBehaviour
{
    [SerializeField] RopeManager ropeManager;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] Camera cam;
    [SerializeField] [Range(1, 100)] int numCats;
    [SerializeField] GameObject catPrefab;
    [SerializeField] CatStats[] catStats;

    InputAction mousePosAction;
    List<CatAI> catchableCats; // cats do a lil animation before despawning, and we shouldn't allow the player to double dip

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mousePosAction = InputSystem.actions.FindAction("MousePos");

        catchableCats = new List<CatAI>();
        SpawnCats();
    }

    // Update is called once per frame  cat.name cat.name
    void Update()
    {
        
    }

    void SpawnCats()
    {
        catchableCats.Clear();
        int totalWeight = 0;
        foreach (var cat in catStats)
        {
            totalWeight += cat.GetSpawnWeight();
        }

        for (int i = 0; i < numCats; i++)
        {
            // spawn cat
            GameObject catInstance = Instantiate(catPrefab, GetRandomPointInCamera() * 0.5f, Quaternion.identity);
            CatAI catAI = catInstance.GetComponent<CatAI>();
            Debug.Assert(catAI != null, "couldn't find CatAI");

            // book keeping
            catchableCats.Add(catAI);

            // choose which cat to spawn (weighted random)
            CatStats randomCat = null;
            int r = Random.Range(0, totalWeight);
            foreach (var cat in catStats)
            {
                if (r < cat.GetSpawnWeight())
                {
                    randomCat = cat;
                    break;
                }
                r -= cat.GetSpawnWeight();
            }
            catAI.Setup(randomCat, this, scoreManager);
        }
        Debug.Log("total number of cats: " + catchableCats.Count);
    }

    // TODO turn this into a delegate that we pass into catAI.Setup as a callback
    public void SetCatUncatchable(CatAI cat)
    {
        catchableCats.Remove(cat);
    }

    public List<CatAI> getCatchableCats()
    {
        return catchableCats;
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
