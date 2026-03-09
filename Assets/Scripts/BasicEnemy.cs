using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] CapsuleCollider _collider;
    [SerializeField] float _Health = 20;
    [SerializeField] float _movementSpeed;
    [SerializeField] Text _healthText;
    NavMeshAgent _EnemyAgent;
    [SerializeField] Transform _player;
    [SerializeField] float _detectionRange;
    [SerializeField] float _damageArea;
    [SerializeField] LayerMask _playerMask;
    public EnemyState currentSatate;

    
    void Awake()
    {
        _EnemyAgent = GetComponent<NavMeshAgent>();
    }
    
    public enum EnemyState
    {
        Waiting,

        Chasing,

        Attacking
    }
    void Start()
    {
        currentSatate = EnemyState.Chasing;
        _healthText.text = "20";
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
            damageable.TakeDamage(5);
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
        Gizmos.DrawWireSphere(transform.position, _damageArea);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
