using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class TechEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] CapsuleCollider _collider;
    [SerializeField] float _Health = 20;
    [SerializeField] float _movementSpeed;
    public Text _healthText;
    UnityEngine.AI.NavMeshAgent _EnemyAgent;
    [SerializeField] Transform _player;
    [SerializeField] float _detectionRange;
    [SerializeField] float _attackArea;
    [SerializeField] LayerMask _playerMask;
    public EnemyState currentSatate;
    private float attackCooldown = 2;
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

        Retreat
    }

    void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        currentSatate = EnemyState.Chasing;
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
        if(OnRange(_attackArea))
        {
            currentSatate = EnemyState.Attacking;
        }
    }

    void Attacking()
    {
        if(!OnRange(_attackArea))
        {
            currentSatate = EnemyState.Chasing;
        }
        IDamageable damageable = _player.gameObject.GetComponent<IDamageable>();
        if(damageable != null)
        {
            if(timer >= attackCooldown)
            {
                damageable.TakeDamage(10);
                Debug.Log("damage");
                timer = 0;
            }

            timer += Time.deltaTime;
            
        }
    }

    public void TakeDamage(float damage)
    {
        _Health -= damage;
        _healthText.text = _Health.ToString();
        if(_Health <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackArea);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
