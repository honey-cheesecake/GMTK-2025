using UnityEngine;
using UnityEngine.InputSystem;

public class CatAI : MonoBehaviour
{
    //Should probably be talking to ScoreManager to tell when a cat is removed
    [SerializeField] ScoreManager scoreManager;

    // waawaawaw
    [Header("Cat speed variables")]
    [SerializeField] float minMoveSpeed = 5.0f;
    [SerializeField] float maxMoveSpeed = 10.0f;
    [SerializeField] float changeDirectionInterval = 2.0f;

    [Header("Cat position variables")]
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float timeSinceLastDirectionChange;

    [Header("Collision avoidance")]
    [SerializeField] float circleCastRadius;
    [SerializeField] LayerMask layerMask;

    private CatManager catManager;
    private float moveSpeed;

    //private float timeSinceLastDirectionChange;

    public void Setup(CatManager catManager)
    {
        this.catManager = catManager;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateTargetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargetPositionIfNeeded();
        MoveTowardTarget();
    }

    public void OnEncircled()
    {
        scoreManager.score = scoreManager.score + 10;
        Destroy(this.gameObject);
    }   

    private void UpdateTargetPositionIfNeeded()
    {
        // Check if it's time to change direction
        timeSinceLastDirectionChange += Time.deltaTime;
        if (timeSinceLastDirectionChange > changeDirectionInterval)
        {
            UpdateTargetPosition();
            return;
        }

        if (Vector3.Distance(transform.position, catManager.MousePosition()) <= circleCastRadius)
        {
            FleeFromMouse();
            return;
        }

        // Check if it's about to hit a cat
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleCastRadius, targetPosition - transform.position, moveSpeed * Time.deltaTime, layerMask);
        if (hit.collider)
        {
            UpdateTargetPosition();
            return;
        }
    }

    private void MoveTowardTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void UpdateTargetPosition()
    {
        // Generate a new random target position within the bounds of your game world
        int attempts = 0;
        int maxAttempts = 10;

        do
        {
            targetPosition = catManager.GetRandomPointInCamera();
            attempts++;
        } while (catManager.CircleCastRope(transform.position, targetPosition, circleCastRadius) && attempts < maxAttempts);

        //speed variance
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);

        // Reset the timer for the next direction change
        timeSinceLastDirectionChange = 0.0f;
    }

    private void FleeFromMouse()
    {
        // Generate a new random target position within the bounds of your game world
        int attempts = 0;
        int maxAttempts = 10;
        Vector3 dirToMouse = catManager.MousePosition() - transform.position;

        do
        {
            targetPosition = catManager.GetRandomPointInCamera();
            attempts++;
        } while (Vector3.Dot(dirToMouse, targetPosition - transform.position) > 0 && attempts < maxAttempts);

        //speed variance
        moveSpeed = maxMoveSpeed;

        // Reset the timer for the next direction change
        timeSinceLastDirectionChange = 0.0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blueViolet;
        Gizmos.DrawWireSphere(transform.position, circleCastRadius);

        Gizmos.DrawLine(transform.position, targetPosition);
    }

}
