using UnityEngine;

public class CatAI : MonoBehaviour
{
    //Should probably be talking to ScoreManager to tell when a cat is removed
    [SerializeField] ScoreManager scoreManager;

    // waawaawaw
    [Header("Cat speed variables")]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float speedVariance = 2f;
    [SerializeField] float changeDirectionInterval = 2.0f;

    [Header("Cat position variables")]
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float timeSinceLastDirectionChange;
    //private float waitTimer = 0f;

    //if cat has hit wall
    private bool hitWall = false;

    //private float timeSinceLastDirectionChange;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveRandomly();
        Debug.Log(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        MoveRandomly();
    }

    public void OnEncircled()
    {
        scoreManager.score = scoreManager.score + 10;
        Destroy(this.gameObject);
    }   

    private void MoveRandomly()
    {

        if (hitWall)
        {
            PickTargetAwayFromWall();
            //return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if it's time to change direction
        timeSinceLastDirectionChange += Time.deltaTime;

        if (timeSinceLastDirectionChange > changeDirectionInterval)
        {
            UpdateTargetPosition();
        }
        
    }

    private void UpdateTargetPosition()
    {
        // Generate a new random target position within the bounds of your game world
        float randomX = Random.Range(-20f, 20f); // 
        float randomY = Random.Range(-10f, 10f); // 
        targetPosition = new Vector3(randomX, randomY, 0f);

        // Calculate the angle in degrees
        float angle2 = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;

        //speed variance
        float randomSpeed = Random.Range(-(speedVariance), speedVariance);
        moveSpeed += randomSpeed;


        // Reset the timer for the next direction change
        timeSinceLastDirectionChange = 0.0f;

        hitWall = false;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Cat"))
        {
            hitWall = true;
            //Debug.Log("Hit wall - stopping movement");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Cat"))
        {
            // Object moved away from wall, can resume movement
            hitWall = false;
        }
    }

    
    private void PickTargetAwayFromWall()
    {
        Vector3 currentPos = transform.position;
        Vector3 newTarget;
        int attempts = 0;
        int maxAttempts = 10;

        do
        {
            // Generate random target
            float randomX = Random.Range(-20f, 20f);
            float randomY = Random.Range(-10f, 10f);
            newTarget = new Vector3(randomX, randomY, 0f);
            attempts++;
        }
        while (Vector3.Distance(currentPos, newTarget) < 3f && attempts < maxAttempts); // Ensure target is reasonably far away

        targetPosition = newTarget;

        // Calculate the angle in degrees
        float angle2 = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;

        // Reset the timer for the next direction change
        timeSinceLastDirectionChange = 0.0f;

        Debug.Log("Moving away from wall to new target");
    }

}
