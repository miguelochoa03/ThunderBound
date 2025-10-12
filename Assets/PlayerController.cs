using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private string currentState = "PlayerIdleAnim";
    private bool walking = false;
    private bool attacking = false;
    private bool crushing = false;


    float horizontalInput;
    float moveSpeed = 5f;
    bool isFacingLeft = false;
    float jumpPower = 4f;
    bool isJumping = false;

    Rigidbody2D rb;
    private Animator animator;


    private float timeBtwAttack;
    public float startTimeBtwAttack;

    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRange;
    public int damage;

    // wamt to add cam shake and knockback to enemies

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        walking = Mathf.Abs(horizontalInput) > 0f;
        FlipSprite();
        // press space to make the character jump
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isJumping = true;
        }
        // handle attack cooldown
        if (timeBtwAttack > 0)
        {
            timeBtwAttack -= Time.deltaTime;
        }
        // handle attack input
        if (timeBtwAttack <= 0 && Input.GetKey(KeyCode.E))
        {
            //Attack();
            Crush();
        }
        // update animation state
        var state = GetState();
        if (state.Equals(currentState)) return;
        currentState = state;
        animator.CrossFade(currentState, 0f, 0);
    }
    // logic for attacking and attack anim
    void Attack()
    {
        attacking = true;
        timeBtwAttack = startTimeBtwAttack;

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<EnemyMovement>().TakeDamage(damage);
        }
        StartCoroutine(ResetAttack());
    }
    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.25f);
        attacking = false;
    }
    // logic for different attack and its anim
    void Crush()
    {
        crushing = true;
        timeBtwAttack = startTimeBtwAttack;

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<EnemyMovement>().TakeDamage(damage);
        }
        StartCoroutine(ResetCrush());
    }
    IEnumerator ResetCrush()
    {
        yield return new WaitForSeconds(0.4f);
        crushing = false;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }
    // correct face the character
    void FlipSprite()
    {
        if (isFacingLeft && horizontalInput > 0f || !isFacingLeft && horizontalInput < 0f)
        {
            isFacingLeft = !isFacingLeft;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    // when it collides, sets jumping to false
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJumping = false;
    }
    // gives visual of the size of the players attack
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }


    // used to play the correct animation
    private string GetState()
    {
        if (crushing) return "PlayerCrushAnim";
        if (attacking) return "PlayerAttackAnim";
        if (walking) return "PlayerWalkAnim";
        return "PlayerIdleAnim";
    }
}
