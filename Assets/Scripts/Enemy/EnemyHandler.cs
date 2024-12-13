using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    public EnemyData enemyData;

    private EnemyMovement movement;
    private EnemySpawner spawner;
    private DropRateManager drop;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public float currentDamage, currentHealth;
    private CircleCollider2D collide;
    private bool isDead = false; // Trạng thái để kiểm soát khi kẻ địch chết


    void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        spawner = FindObjectOfType<EnemySpawner>();
        drop = GetComponent<DropRateManager>();
        collide = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentDamage = enemyData.Damage;
        currentHealth = enemyData.MaxHealth;
    }

    void FixedUpdate()
    {
        CheckDeathAnimation();
        // Only check on Objects layer to reduce the number of unncessary cheks
        LayerMask mask = LayerMask.GetMask("Objects");
        // Check for nearby colliders within collider radius
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, collide.radius, mask);

        // Track processed pairs to avoid redundant checks
        // HashSet is used instead of List because it avoid duplicates
        // Which is good because we don't want to add the same pair
        HashSet<(Collider2D, Collider2D)> processedPairs = new HashSet<(Collider2D, Collider2D)>();

        foreach (Collider2D obj1 in nearbyObjects)
        {
            foreach (Collider2D obj2 in nearbyObjects)
            {
                // Skip self-collision and already processed pairs
                if (obj1 == obj2 || processedPairs.Contains((obj1, obj2)) || processedPairs.Contains((obj2, obj1)))
                    continue;

                // Add the pair to the processed set
                processedPairs.Add((obj1, obj2));

                // Resolve overlap between obj1 and obj2
                HandleOverlap(obj1, obj2);
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        movement.Knockback(5f, 0.2f); // Đẩy lùi khi nhận sát thương
        DamagePopUp.Create(transform.position, dmg);
        ScoreBoard.Instance.totalDamage += dmg;
        if (currentHealth <= 0)
        {
            Die();
        }
        
    }

    private void Die()
    {
        isDead = true; // Đánh dấu kẻ địch đã chết
        currentHealth = 0;

        // Dừng di chuyển
        if (movement != null) movement.enabled = false;

        // Phát hoạt ảnh chết
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        // Rớt đồ
        if (drop != null) drop.DropPickUp();

        // Vô hiệu hóa collider
        if (collide != null) collide.enabled = false;

        //Tính số lượng quái giết
        ScoreBoard.Instance.enemyKilled++;
    }

    private void CheckDeathAnimation()
    {
        if (animator != null)
        {
            // Kiểm tra nếu hoạt ảnh chết đã hoàn tất
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(enemyData.deathAnimName) && stateInfo.normalizedTime > 1 && !animator.IsInTransition(0)) // Hoạt ảnh "Die" kết thúc
            {
                ReturnToPool();
            }
        }
    }

    private void ReturnToPool()
    {

        ObjectPools.EnqueueObject(this, enemyData.name);
        spawner.enemiesAlive--;
        ResetEnemy();
    }

    void HandleOverlap(Collider2D obj1, Collider2D obj2)
    {
        // Determine if either object is the player
        bool isPlayer1 = obj1.CompareTag("Player");
        bool isPlayer2 = obj2.CompareTag("Player");

        Vector2 direction = (Vector2)(obj1.transform.position - obj2.transform.position);
        float distance = direction.magnitude;

        float combinedRadius = obj1.bounds.extents.x + obj2.bounds.extents.x; // Assuming circular objects

        // Calculate the overlap amount
        float overlap = combinedRadius - distance;

        if (overlap > 0) // Resolve overlap if necessary
        {
            // Split the resolution equally between the two objects
            Vector3 halfOverlap = (overlap / 2) * direction.normalized;

            if (!isPlayer1 && !isPlayer2)
            {
                obj1.transform.position += halfOverlap;
                obj2.transform.position -= halfOverlap;
            }
            else if (isPlayer1)
            {
                // Only move obj2 if obj1 is the player
                obj2.transform.position -= (Vector3)(overlap * direction.normalized);
            }
            else
            {
                // Only move obj1 if obj2 is the player
                obj1.transform.position += (Vector3)(overlap * direction.normalized);
            }
        }
    }


    void ResetEnemy()
    {
        isDead = false;
        currentHealth = enemyData.MaxHealth;
        
        // Resetting enemy's opacity
        Color color = spriteRenderer.color;
        color.a = 1f; // Fully visible
        spriteRenderer.color = color;

        // Kích hoạt lại collider
        if (collide != null) collide.enabled = true;

        // Kích hoạt lại di chuyển
        if (movement != null) movement.enabled = true;

        // Đặt trạng thái animator về mặc định
        if (animator != null) animator.SetBool("isDead", false);

    }
}


    

