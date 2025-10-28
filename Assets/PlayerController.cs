using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip dirtStepClip;
    public float footstepInterval = 0.4f;
    private float footstepTimer = 0f;

    public AudioSource attackSource;
    public AudioClip attackClip;
    public AudioSource crushSource;
    public AudioClip crushClip;

    public AudioSource dangerSource;
    public AudioClip dangerClip;

    public TextMeshProUGUI HPtext;
    Rigidbody2D rb;
    private Animator animator;
    public GameObject bloodEffect;

    private string currentState = "PlayerIdleAnim";
    private bool walking = false;
    private bool attacking = false;
    private bool crushing = false;    
    float horizontalInput;
    float moveSpeed = 5f;
    float origMoveSpeed = 5f;
    bool isFacingLeft = false;
    float jumpPower = 6f;
    bool isJumping = false;    
    public float health = 100f;
    private float timeBtwAttack;
    public float startTimeBtwAttack;
    public Transform attackPos;
    public Transform crushPos;
    public LayerMask whatIsEnemies;
    public float attackRange;
    public float crushRange;
    public int damage;
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
        // horizontal movement
        horizontalInput = Input.GetAxis("Horizontal");
        walking = Mathf.Abs(horizontalInput) > 0f;

        // plays walking dirt steps
        if (walking && IsGrounded())
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                footstepSource.PlayOneShot(dirtStepClip);
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            // reset when not walking
            footstepSource.Stop();
            footstepTimer = 0f;
        }

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
        // handle crush input
        if (timeBtwAttack <= 0 && Input.GetKey(KeyCode.R))
        {
            Crush();
        }
        // update animation state
        var state = GetState();
        if (state.Equals(currentState)) return;
        currentState = state;
        animator.CrossFade(currentState, 0f, 0);

        // make sure can't flip sprite when attacking
        isDoingAction = attacking && crushing && walking;
        if (isDoingAction == false)
        {
            FlipSprite();
        }
    }
    private bool IsGrounded()
    {
        return !isJumping;
    }

    // logic for attacking and attack anim
    void Attack()
    {
        attackSource.PlayOneShot(attackClip);
        attacking = true;
        timeBtwAttack = startTimeBtwAttack;
        // damage enemies in radius
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        // align the animation and damage
        StartCoroutine(DelayAttackDamage(enemiesToDamage));
        // makes sure to add cooldown and change move speed
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
        crushSource.PlayOneShot(crushClip);
        crushing = true;
        timeBtwAttack = startTimeBtwAttack;
        // damage enemies in radius
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(crushPos.position, crushRange, whatIsEnemies);
        // align the animation and damage
        StartCoroutine(DelayCrushDamage(enemiesToDamage));
        // makes sure to add cooldown and change move speed
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

    // player takes damage
    public void TakeDamage(int damage, Transform enemyTransform)
    {
        // blood
        GameObject bloodEffectCopy = Instantiate(bloodEffect, transform.position, Quaternion.identity);
        StartCoroutine(DestroyBloodAfterDelay(bloodEffectCopy));
        health -= damage;
        // updates the ui
        HPtext.text = $"HP {health}";
        if (health < 0)
        {
            // restart bring back to start menu
            SceneManager.LoadScene("Start");
        }
    }
    // give some time for the blood effect before disappearing
    IEnumerator DestroyBloodAfterDelay(GameObject bloodEffectCopy)
    {
        yield return new WaitForSeconds(2f);
        Destroy(bloodEffectCopy);
    }
}
