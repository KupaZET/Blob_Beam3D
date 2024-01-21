using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    public class Enemy
    {
        public string id;
        public EnemyState state;
        public int hitsTaken;

        public Enemy(string id, EnemyState state)
        {
            this.id = id;
            this.state = state;
            hitsTaken = 0;
        }
    }

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

    public Transform playerTransform;
    private bool isHitCooldown;
    public float hitCooldownDuration = 2f;
    public Transform startPoint;
    public Transform endPoint;
    private Transform target;
    private Animator _animatorController;
    public int corpseDuration;
    public static List<Enemy> enemies = new List<Enemy>();

    void Start()
    {
        _animatorController = GetComponent<Animator>();
        target = endPoint;
        var enemy = new Enemy(gameObject.name, EnemyState.Walk);
        enemies.Add(enemy);
    }

    void Update()
    {
        var enemy = enemies.FirstOrDefault(x => x.id == gameObject.name);
        if (enemy != null)
        {
            switch (enemy.state)
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
        }
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
        if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRange && PlayerMovement.state != PlayerMovement.PlayerState.Dead)
        {
            var enemy = enemies.FirstOrDefault(x => x.id == gameObject.name);
            if(enemy != null)
            {
                enemy.state = enemy.hitsTaken == 1 ? EnemyState.RunAway : EnemyState.Attack;
            }
        }
    }

    void Attack()
    {
        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, walkSpeed * Time.deltaTime);
        
        // Check if the player is still in range or dead
        if (PlayerMovement.state == PlayerMovement.PlayerState.Dead)
        {
            var enemy = enemies.FirstOrDefault(x => x.id == gameObject.name);
            if(enemy != null)
            {
                enemy.state = EnemyState.Walk;
            }
        }
    }

    void RunAway()
    {
        // Move away from the player
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.position - playerTransform.position).normalized * walkSpeed, walkSpeed * Time.deltaTime);

        // Check if the player is no longer in range, then switch back to Walk
        if (Vector3.Distance(transform.position, playerTransform.position) > detectionRange)
        {
            var enemy = enemies.FirstOrDefault(x => x.id == gameObject.name);
            if(enemy != null)
            {
                enemy.hitsTaken = 0; // Get healthy if ran away successfully
                enemy.state = EnemyState.Walk;
            }
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

    public void OnTriggerEnter(Collider other)
    {
        var enemy = enemies.FirstOrDefault(x => x.id == gameObject.name);
        if (other == null || enemy == null)
        {
            return;
        }

        if (playerTransform.gameObject == other.gameObject && enemy.state != EnemyState.Dead && !isHitCooldown)
        {
            enemy.hitsTaken++;
            if (enemy.hitsTaken == 1)
            {
                enemy.state = EnemyState.RunAway;
                isHitCooldown = true;
                Invoke("ResetHitCooldown", hitCooldownDuration);
            }
            else if (enemy.hitsTaken >= 2)
            {
                enemy.state = EnemyState.Dead;
            }
        }
    }
}
