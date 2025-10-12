using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private string currentState = "Idle";
    private bool walking = false;
    private bool attacking = false;


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

    //public Animator camAnim;
    //public Animator playerAnim;

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
            Attack();
        }
        // update animation state



        //if (timeBtwAttack <= 0)
        //{
        //    // then you can attack
        //    if (Input.GetKey(KeyCode.E))
        //    {
        //        //camAnim.SetTrigger("shake");
        //        attacking = true;

        //        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        //        for (int i = 0; i < enemiesToDamage.Length; i++)
        //        {
        //            enemiesToDamage[i].GetComponent<EnemyMovement>().TakeDamage(damage);
        //        }
        //    } 
        //    //else
        //    //{
        //    //    attacking = false;
        //    //}

        //    timeBtwAttack = startTimeBtwAttack;

        //} else
        //{
        //    timeBtwAttack -= Time.deltaTime;
        //    attacking = false;
        //}

        // update animation state
        var state = GetState();
        if (state.Equals(currentState)) return;
        currentState = state;
        animator.CrossFade(currentState, 0f, 0);
    }

    void Attack()
    {
        attacking = true;
        timeBtwAttack = startTimeBtwAttack;

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<EnemyMovement>().TakeDamage(damage);
        }
        // 
        StartCoroutine(ResetAttack());
    }
    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.25f);
        attacking = false;
    }


    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJumping = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    private string GetState()
    {
        if (attacking) return "PlayerAttackAnim";
        if (walking) return "PlayerWalkAnim";
        return "PlayerIdleAnim";
    }
}
