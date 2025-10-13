using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public GameObject player;

    public bool flip;

    public float speed;

    public float jumpPower;
    public bool isJumping;

    public float jumpCooldown;

    public float health = 20f;
    public GameObject bloodEffect;

    private float dazedTime;
    public float startDazedTime;

    private float timeBtwAttackEnemy;
    public float startTimeBtwAttackEnemy;
    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRange;
    public int damage;
    public float knockbackForce = 5f;



    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // die when health reaches zero
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        // handles dazing the enemy
        if (dazedTime <= 0)
        {
            speed = 0.4f;
        } else
        {
            speed = 0;
            dazedTime -= Time.deltaTime;
        }

            Vector3 scale = transform.localScale;

        if (player.transform.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x) * -1 * (flip ? -1 : 1);
            transform.Translate(speed * Time.deltaTime, 0, 0);
        } else
        {
            scale.x = Mathf.Abs(scale.x) * (flip ? -1 : 1);
            transform.Translate(speed * Time.deltaTime * -1, 0, 0);
        }

        transform.localScale = scale;

        if (Time.time > jumpCooldown && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isJumping = true;
            jumpCooldown = Time.time + 3;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJumping = false;


        EnemyAttack();
    }

    public void TakeDamage(int damage)
    {
        // want to add knockback

        // will daze the enemy when hit
        dazedTime = startDazedTime;
        // play a hurt sound

        // blood
        Instantiate(bloodEffect, transform.position, Quaternion.identity);
        health -= damage;
        Debug.Log("Damage TAKEN !");
    }

    void EnemyAttack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
    // gives visual of the size of the enemys attack
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
