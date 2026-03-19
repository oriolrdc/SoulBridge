using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class MagicEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] CapsuleCollider _collider;
    [SerializeField] float _Health = 20;
    [SerializeField] float _movementSpeed;
    public Text _healthText;
    NavMeshAgent _EnemyAgent;
    [SerializeField] Transform _player;
    [SerializeField] float _detectionRange;
    [SerializeField] float _damageArea;
    [SerializeField] float _retreatDistance;
    [SerializeField] LayerMask _playerMask;
    public EnemyState currentSatate;
    private float attackCooldown = 1;
    private float timer;
    [SerializeField] Transform _shooter;

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
        //_healthText.text = "20";
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

            case EnemyState.Retreat:
                Retreat();
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
        //animaciones
        if(_player != null)
        {
            currentSatate = EnemyState.Chasing;
        }
    }

    void Retreat()
    {
        _EnemyAgent.isStopped = false;
        if(!OnRange(_retreatDistance))
        {
            currentSatate = EnemyState.Chasing;
        }
        
        Vector3 directionToPlayer = transform.position - _player.position;
        Vector3 RetreatPoint = transform.position + directionToPlayer.normalized * 5f;
        _EnemyAgent.SetDestination(RetreatPoint);

        if(transform.position == RetreatPoint)
        {
            currentSatate = EnemyState.Attacking;
        }
    }

    void Chasing()
    {
        _EnemyAgent.isStopped = false;

        if (OnRange(_damageArea))
        {
            currentSatate = EnemyState.Attacking;
        }

        if (OnRange(_retreatDistance)) 
        {
            currentSatate = EnemyState.Retreat;
        }
        
        if (OnRange(_detectionRange))
        {
            _EnemyAgent.SetDestination(_player.position);
        }
    }

    void Attacking()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0; 
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (OnRange(_retreatDistance)) 
        {
            currentSatate = EnemyState.Retreat;
        }

        if(!OnRange(_damageArea))
        {
            currentSatate = EnemyState.Chasing;
            return;
        }

        _EnemyAgent.isStopped = true;
             
        if(timer >= attackCooldown)
        {
            GameObject bullet = PoolManager.Instance.GetPooledObject("BalasEnemigos", _shooter.position, _shooter.rotation);
            bullet.SetActive(true);
            Debug.Log("shoot");
            timer = 0;
        }

        timer += Time.deltaTime;
        
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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _retreatDistance);
    }
}
