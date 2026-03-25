using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class BasicEnemy : MonoBehaviour, IDamageable, IKnockbackable
{
    [SerializeField] CapsuleCollider _collider;
    [SerializeField] Rigidbody _rb;
    [SerializeField] float _Health = 20;
    [SerializeField] float _movementSpeed;
    [SerializeField] Text _healthText;
    NavMeshAgent _EnemyAgent;
    [SerializeField] Transform _player;
    [SerializeField] float _detectionRange;
    [SerializeField] float _damageArea;
    [SerializeField] LayerMask _playerMask;
    public EnemyState currentSatate;
    private float attackCooldown = 1;
    private float timer;


    
    void Awake()
    {
        _EnemyAgent = GetComponent<NavMeshAgent>();
    }
    
    public enum EnemyState
    {
        Waiting,

        Chasing,

        Attacking,

        Knockback
    }
    void Start()
    {
        currentSatate = EnemyState.Chasing;
        _healthText.text = "20";
        timer = 1;
    }
    
    void Update()
    {
        switch(currentSatate)
        {
            case EnemyState.Waiting:
                Waiting();
            break;

            case EnemyState.Chasing:
                Chasing();
            break;

            case EnemyState.Attacking:
                Attacking();
            break;

            case EnemyState.Knockback:
                StartCoroutine(GetKnockback());
            break;

            default:
                Chasing();
            break;
        }
    }
    
    bool OnRange(float distance)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        if(distanceToPlayer <= distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    void Waiting()
    {
        if(_player != null)
        {
            currentSatate = EnemyState.Chasing;
        }
    }
    
    void Chasing()
    {
        if(_player == null)
        {
            currentSatate = EnemyState.Waiting;
        }
        if(OnRange(_detectionRange))
        {
            _EnemyAgent.SetDestination(_player.position);
        }
        if(OnRange(_damageArea))
        {
            currentSatate = EnemyState.Attacking;
        }
    }
    
    void Attacking()
    {
        if(!OnRange(_damageArea))
        {
            currentSatate = EnemyState.Chasing;
        }
        IDamageable damageable = _player.gameObject.GetComponent<IDamageable>();
        if(damageable != null)
        {
            if(timer >= attackCooldown)
            {
                damageable.TakeDamage(5);
                Debug.Log("damage");
                timer = 0;
            }

            timer += Time.deltaTime;
            
        }
    }

    public IEnumerator GetKnockback() 
    {
        _EnemyAgent.enabled = false;
        _rb.useGravity = true;
        _rb.isKinematic = false;
        _rb.AddForce(-transform.forward * 20, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);
        _rb.linearVelocity = Vector3.zero;
        _rb.useGravity = false;
        _rb.isKinematic = true;
        _EnemyAgent.enabled = true;
        currentSatate = EnemyState.Chasing;
    }
    
    
    public void TakeDamage(float damage)
    {
        _Health -= damage;
        _healthText.text = _Health.ToString();
        if(_Health <= 0)
        {
            Death();
        }
        currentSatate = EnemyState.Knockback;
    }

    void Death()
    {
        Destroy(gameObject);
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _damageArea);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
