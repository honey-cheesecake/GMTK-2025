using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class CatAI : MonoBehaviour
{
    // waawaawaw
    [Header("Cat speed variables")]
    [SerializeField] float changeDirectionInterval = 2.0f;

    [Header("Cat position variables")]
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float timeSinceLastDirectionChange;

    [Header("Collision avoidance")]
    [SerializeField] float circleCastRadius;
    [SerializeField] LayerMask layerMask;

    [Header("Component References")]
    [SerializeField] Animator anim;
    [SerializeField] SpriteLibrary spriteLibrary;

    private CatManager catManager;
    ScoreManager scoreManager;
    private float moveSpeed;
    private CatStats stats;

    //private float timeSinceLastDirectionChange;

    // called by the CatManager when this cat is instantiated
    public void Setup(CatStats stats, CatManager catManager, ScoreManager scoreManager)
    {
        this.stats = stats;
        spriteLibrary.spriteLibraryAsset = stats.SpriteLibraryAsset;

        this.catManager = catManager;
        this.scoreManager = scoreManager;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim.SetBool("IsRunning", false);
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
        scoreManager.AddToScore(10);
        catManager.SetCatUncatchable(this);
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

        // flee from mouse if it's too close
        if (Vector3.Distance(transform.position, catManager.MousePosition()) <= circleCastRadius)
        {
            FleeFromMouse();
            return;
        }

        // Check if it's about to hit another cat
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleCastRadius, targetPosition - transform.position, moveSpeed * Time.deltaTime, layerMask);
        if (hit.collider)
        {
            UpdateTargetPosition();
            return;
        }

        // we don't need to check for walls because we pick the targetPosition to be within the screen bounds
    }

    private void MoveTowardTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void UpdateTargetPosition()
    {
        anim.SetBool("IsRunning", false);
        // Generate a new random target position within the bounds of your game world
        int attempts = 0;
        int maxAttempts = 10;

        do
        {
            targetPosition = catManager.GetRandomPointInCamera();
            attempts++;
        } while (catManager.CircleCastRope(transform.position, targetPosition, circleCastRadius) && attempts < maxAttempts);

        //speed variance
        moveSpeed = Random.Range(stats.MinMoveSpeed, stats.MaxMoveSpeed);

        // Reset the timer for the next direction change
        timeSinceLastDirectionChange = 0.0f;
    }

    private void FleeFromMouse()
    {
        anim.SetBool("IsRunning", true);
        // Generate a new random target position within the bounds of your game world
        int attempts = 0;
        int maxAttempts = 10;
        Vector3 mousePos = catManager.MousePosition();
        Vector3 dirToMouse = mousePos - transform.position;

        do
        {
            targetPosition = catManager.GetRandomPointInCamera();
            attempts++;
        } while (Vector3.Dot(dirToMouse, targetPosition - transform.position) > 0
            && Vector3.Distance(mousePos, targetPosition) < 3f
            && attempts < maxAttempts);

        //speed variance
        moveSpeed = stats.RunSpeed;

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
