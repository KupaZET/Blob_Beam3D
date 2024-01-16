using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    public enum EnemyState
    {
        Walk,
        Attack,
        RunAway,
        Dead
    }

    public float walkSpeed = 2f;
    public float runAwaySpeed = 5f;
    public float detectionRange = 5f;

    public static EnemyState enemyState;
    public Transform playerTransform;
    private int hitsTaken;
    private bool isHitCooldown;
    public float hitCooldownDuration = 2f;
    public Rigidbody rb;
    public Transform startPoint;
    public Transform endPoint;
    private Transform target;
    private Animator _animatorController;
    public int corpseDuration;

    void Start()
    {
        _animatorController = GetComponent<Animator>();
        enemyState = EnemyState.Walk;
        hitsTaken = 0;
        rb = GetComponent<Rigidbody>();
        target = endPoint;
    }

    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Walk:
                _animatorController.SetBool("seePlayer", false);
                Walk();
                break;
            case EnemyState.Attack:
                _animatorController.SetBool("seePlayer", true);
                Attack();
                break;
            case EnemyState.RunAway:
                _animatorController.SetTrigger("damage");
                RunAway();
                break;
            case EnemyState.Dead:
                _animatorController.SetTrigger("DeathEnemy");
                Invoke("Dead", corpseDuration);
                break;
        }

        SpeedControl();
    }

    void Walk()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, walkSpeed * Time.deltaTime);
        
        // Check if the enemy has reached the target
        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            // Switch to the other target
            target = (target == startPoint) ? endPoint : startPoint;
        }

        // Check if the player is in front of the enemy to switch to Attack state
        if (Vector3.Distance(transform.position, playerTransform.position) < detectionRange && PlayerMovement.state != PlayerMovement.PlayerState.Dead)
        {
            enemyState = hitsTaken == 1 ? EnemyState.RunAway : EnemyState.Attack;
        }
    }

    void Attack()
    {
        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, walkSpeed * Time.deltaTime);

        // Check if the player is still in range or dead
        if (Vector3.Distance(transform.position, playerTransform.position) > detectionRange || PlayerMovement.state == PlayerMovement.PlayerState.Dead)
        {
            enemyState = EnemyState.Walk;
        }
    }

    void RunAway()
    {
        // Move away from the player
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.position - playerTransform.position).normalized * walkSpeed, walkSpeed * Time.deltaTime);

        // Check if the player is no longer in range, then switch back to Walk
        if (Vector3.Distance(transform.position, playerTransform.position) > detectionRange)
        {
            hitsTaken = 0; // Get healthy if ran away successfully
            enemyState = EnemyState.Walk;
        }
    }

    void Dead()
    {
        Destroy(gameObject);
    }

    void ResetHitCooldown()
    {
        isHitCooldown = false;
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limit velocity if needed
        if (flatVel.magnitude > walkSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * walkSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerTransform.gameObject == other.gameObject && enemyState != EnemyState.Dead && !isHitCooldown)
        {
            hitsTaken++;
            if (hitsTaken == 1)
            {
                enemyState = EnemyState.RunAway;
                isHitCooldown = true;
                Invoke("ResetHitCooldown", hitCooldownDuration);
            }
            else if (hitsTaken >= 2)
            {
                enemyState = EnemyState.Dead;
            }
        }
    }
}
