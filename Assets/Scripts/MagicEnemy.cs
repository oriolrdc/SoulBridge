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
    [SerializeField] float _StopDistance;
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

        Attacking
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

    void Chasing()
{
    if (_player == null) 
    {
        currentSatate = EnemyState.Waiting;
        return;
    }
    // 1. LÓGICA DE MOVIMIENTO
    if (OnRange(_StopDistance)) 
    {
        // Ya llegó a su posición ideal de disparo: FRENAR
        _EnemyAgent.isStopped = true;
        _EnemyAgent.velocity = Vector3.zero;
        return;
    }
    else if (OnRange(_detectionRange))
    {
        // Está lejos: PERSEGUIR
        _EnemyAgent.isStopped = false;
        _EnemyAgent.SetDestination(_player.position);
    }

    // 2. LÓGICA DE CAMBIO DE ESTADO (ATAQUE)
    // Si la distancia es menor al área de daño, disparamos 
    // (independientemente de si se está moviendo o no)
    if (OnRange(_damageArea))
    {
        currentSatate = EnemyState.Attacking;
    }
}
    /*void Chasing()
    {
        if(_player == null)
        {
            currentSatate = EnemyState.Waiting;
        }
        
        if(OnRange(_StopDistance))
        {
            _EnemyAgent.isStopped = true;
            _EnemyAgent.velocity = Vector3.zero;
        }
        else if(OnRange(_detectionRange))
        {
            _EnemyAgent.SetDestination(_player.position);
        }
        
        if(OnRange(_damageArea))
        {
            currentSatate = EnemyState.Attacking;
        }
    }*/

    void Attacking()
    {
        if(!OnRange(_damageArea))
        {
            currentSatate = EnemyState.Chasing;
            return;
        }
             
        if(timer >= attackCooldown)
        {
            /*GameObject bullet = PoolManager.Instance.GetPooledObject("BalasThalya", _shooter.position, _shooter.rotation);
            IDamageable damageable = _player.gameObject.GetComponent<IDamageable>();
            if(damageable != null)
            {
            
                damageable.TakeDamage(5);
                Debug.Log("damage");
                timer = 0;
            }*/
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
        Gizmos.DrawWireSphere(transform.position, _StopDistance);
    }
}
