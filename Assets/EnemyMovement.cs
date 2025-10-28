using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    GameObject player;

    public AudioSource jumpSource;
    public AudioClip jumpClip;
    //public AudioSource slimeSource;
    //public AudioClip slimeClip;

    public bool flip;

    public float speed = 1f;

    public float jumpPower;
    public bool isJumping;

    float jumpCooldown;

    private float health = 50f;

    public GameObject bloodEffect;

    private float dazedTime;
    public float startDazedTime;

    private float timeBtwAttackEnemy;
    public float startTimeBtwAttackEnemy;
    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRange;
    public int damage = 26;

    float knockbackForceX;
    float knockbackForceY;

    //public Waves Waves;

    Rigidbody2D rb;
    void Start()
    {
        //slimeSource.PlayOneShot(slimeClip);
        // find player in scene
        player = GameObject.FindWithTag("Player");

        rb = GetComponent<Rigidbody2D>();
        //Healthbar.SetHealth(health, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        // die when health reaches zero
        if (health <= 0)
        {
            Destroy(gameObject);
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
            jumpSource.PlayOneShot(jumpClip);
            jumpPower = Random.Range(6, 8);
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isJumping = true;
            jumpCooldown = Time.time + Random.Range(1, 5);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJumping = false;


        EnemyAttack();
    }


    public void TakeDamage(int damage, Transform playerTransform)
    {
        // add knockback
        knockbackForceX = Random.Range(3, 5);
        knockbackForceY = Random.Range(2, 4);
        Vector2 knockbackDirection = (transform.position - playerTransform.position).normalized;
        rb.AddForce(knockbackDirection * knockbackForceX, ForceMode2D.Impulse);
        rb.AddForce(Vector2.up * knockbackForceY, ForceMode2D.Impulse);

        // will daze the enemy when hit
        dazedTime = startDazedTime;
        // play a hurt sound

        // blood
        //Instantiate(bloodEffect, transform.position, Quaternion.identity);
        GameObject bloodEffectCopy = Instantiate(bloodEffect, transform.position, Quaternion.identity);
        StartCoroutine(DestroyBloodAfterDelay(bloodEffectCopy));
        health -= damage;
        Debug.Log("Damage TAKEN !");
    }
    IEnumerator DestroyBloodAfterDelay(GameObject bloodEffectCopy)
    {
        yield return new WaitForSeconds(2f);
        Destroy(bloodEffectCopy);
    }

    void EnemyAttack()
    {
        
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<PlayerController>().TakeDamage(damage, transform);
        }
    }
    // gives visual of the size of the enemys attack
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
