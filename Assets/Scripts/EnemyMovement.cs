using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private EnemyState currentState;
    public Transform playerTransform;
    private Vector3 initialPosition;
    private int hitsTaken;
    private bool isHitCooldown;
    public float hitCooldownDuration = 2f;
    public Rigidbody rb;
    public Transform startPoint;
    public Transform endPoint;
    private Transform target;

    void Start()
    {
        currentState = EnemyState.Walk;
        initialPosition = transform.position;
        hitsTaken = 0;
        rb = GetComponent<Rigidbody>();
        target = endPoint;
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Walk:
                Walk();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.RunAway:
                RunAway();
                break;
            case EnemyState.Dead:
                Dead();
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
        if (Vector3.Distance(transform.position, playerTransform.position) < detectionRange)
        {
            currentState = hitsTaken == 1 ? EnemyState.RunAway : EnemyState.Attack;
        }
    }

    void Attack()
    {
        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, walkSpeed * Time.deltaTime);

        // Check if the player is still in range
        if (Vector3.Distance(transform.position, playerTransform.position) > detectionRange)
        {
            currentState = EnemyState.Walk;
        }

        PlayerAttacked();
    }

    void RunAway()
    {
        // Move away from the player
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.position - playerTransform.position).normalized * walkSpeed, walkSpeed * Time.deltaTime);

        // Check if the player is no longer in range, then switch back to Walk
        if (Vector3.Distance(transform.position, playerTransform.position) > detectionRange)
        {
            hitsTaken = 0; // Get healthy if ran away successfully
            currentState = EnemyState.Walk;
        }

        PlayerAttacked();
    }

    void Dead()
    {
        Destroy(gameObject);
    }

    bool IsPlayerJumpingOnEnemy()
    {
        Rigidbody playerRigidbody = playerTransform.GetComponent<Rigidbody>();

        if (playerRigidbody != null && 
            playerTransform.position.y > transform.position.y &&
            playerRigidbody.velocity.y < 0f &&
            Mathf.Abs(playerTransform.position.x - transform.position.x) < 1.0f &&
            Mathf.Abs(playerTransform.position.z - transform.position.z) < 1.0f)
        {
                return true;
        }

        return false;
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

    private void PlayerAttacked()
    {
        // Check if the player hits the enemy
        if (IsPlayerJumpingOnEnemy() && currentState != EnemyState.Dead && !isHitCooldown)
        {
            hitsTaken++;

            if (hitsTaken == 1)
            {
                currentState = EnemyState.RunAway;
                isHitCooldown = true;
                Invoke("ResetHitCooldown", hitCooldownDuration);
            }
            else if (hitsTaken >= 2)
            {
                currentState = EnemyState.Dead;
            }
        }
    }
}
