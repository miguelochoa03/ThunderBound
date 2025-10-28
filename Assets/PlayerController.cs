using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private string currentState = "PlayerIdleAnim";
    private bool walking = false;
    private bool attacking = false;
    private bool crushing = false;

    public TextMeshProUGUI HPtext;

    float horizontalInput;
    float moveSpeed = 5f;
    float origMoveSpeed = 5f;
    bool isFacingLeft = false;
    float jumpPower = 6f;
    bool isJumping = false;

    Rigidbody2D rb;
    private Animator animator;

    public GameObject bloodEffect;
    public float health = 100f;


    private float timeBtwAttack;
    public float startTimeBtwAttack;

    public Transform attackPos;
    public Transform crushPos;
    public LayerMask whatIsEnemies;
    public float attackRange;
    public float crushRange;
    public int damage;

    float sprintSpeed = 10f;

    // wamt to add cam shake and knockback to enemies
    // need to stop movement when attacking
    // camera follows player

    //bool isDoingAction = attacking && crushing && walking;
    bool isDoingAction;

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
            Attack();
        }
        if (timeBtwAttack <= 0 && Input.GetKey(KeyCode.R))
        {
            Crush();
        }
        // update animation state
        var state = GetState();
        if (state.Equals(currentState)) return;
        currentState = state;
        animator.CrossFade(currentState, 0f, 0);

        isDoingAction = attacking && crushing && walking;
        if (isDoingAction == false)
        {
            FlipSprite();
        }
    }
    // logic for attacking and attack anim
    void Attack()
    {
        attacking = true;
        timeBtwAttack = startTimeBtwAttack;

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        StartCoroutine(DelayAttackDamage(enemiesToDamage));
        StartCoroutine(ResetAttack());
    }
    IEnumerator DelayAttackDamage(Collider2D[] enemiesToDamage)
    {
        yield return new WaitForSeconds(0.4f);

        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<EnemyMovement>().TakeDamage(damage, transform);
        }
    }
    IEnumerator ResetAttack()
    {
        moveSpeed = 0;
        yield return new WaitForSeconds(0.5f);
        moveSpeed = origMoveSpeed;
        attacking = false;
    }
    // logic for different attack and its anim
    void Crush()
    {
        // make size change multiple times to make crush attacking much better

        crushing = true;
        timeBtwAttack = startTimeBtwAttack;

        // downward force
        //rb.velocity = new Vector2(rb.velocity.x, -60f);

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(crushPos.position, crushRange, whatIsEnemies);
        StartCoroutine(DelayCrushDamage(enemiesToDamage));
        StartCoroutine(ResetCrush());
    }
    IEnumerator DelayCrushDamage(Collider2D[] enemiesToDamage)
    {
        yield return new WaitForSeconds(0.4f);

        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<EnemyMovement>().TakeDamage(damage, transform);
        }
    }
    IEnumerator ResetCrush()
    {
        moveSpeed = 0;
        yield return new WaitForSeconds(0.6f);
        moveSpeed = origMoveSpeed;
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
        Gizmos.DrawWireSphere(crushPos.position, crushRange);
    }


    // used to play the correct animation
    private string GetState()
    {
        if (crushing) return "PlayerCrushAnim";
        if (attacking) return "PlayerAttackAnim";
        if (walking) return "PlayerWalkAnim";
        return "PlayerIdleAnim";
    }
    //public void ApplyKnockback(Vector2 force)
    //{
    //    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    //    if (rb != null)
    //    {
    //        rb.AddForce(force, ForceMode2D.Impulse);
    //    }
    //}

    // player takes damage
    public void TakeDamage(int damage, Transform enemyTransform)
    {
        // want to add knockback

        // play a hurt sound

        // blood
        //Instantiate(bloodEffect, transform.position, Quaternion.identity);
        GameObject bloodEffectCopy = Instantiate(bloodEffect, transform.position, Quaternion.identity);
        StartCoroutine(DestroyBloodAfterDelay(bloodEffectCopy));
        health -= damage;
        HPtext.text = $"HP {health}";
        if (health < 0)
        {
            //Destroy(gameObject);
            SceneManager.LoadScene("Start");
        }
        Debug.Log("Player Damage TAKEN !");
    }
    IEnumerator DestroyBloodAfterDelay(GameObject bloodEffectCopy)
    {
        yield return new WaitForSeconds(2f);
        Destroy(bloodEffectCopy);
    }
}
