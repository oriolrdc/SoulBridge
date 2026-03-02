using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerControllerTemp : MonoBehaviour
{
    //Componentes
    private CharacterController _controller;
    [SerializeField] private CapsuleCollider _collider;
    private Animator _animator;
    //Inputs 
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _lookAction;
    private InputAction _AttackAction;
    private InputAction _dashAction;
    private InputAction _interactAction;
    private InputAction _SpecialAction;
    private InputAction _UltiAction;
    [SerializeField] private Vector2 _lookInput;
    private InputAction _grabAction;
    private Vector2 _moveInput;
    //Variables
    [SerializeField] private float _jumpHeight = 2;
    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private float _smoothTime = 0.1f;
    private float _turnSmoothVelocity;
    //Camera
    private Transform _mainCamera;
    [SerializeField] float _speedChangeRate = 10;
    float _speed;
    float _animationSpeed;
    float sprintSpeed = 10;
    bool isSprinting = false;
    float targetAngle;
    //Cambio de personajes
    [SerializeField] private GameObject _Chico;
    [SerializeField] private GameObject _Chica;
    private InputAction _changeAction;
    [SerializeField] private bool _ChicoActive = false;
    [SerializeField] private bool _ChicaActive = true;
    private bool _isChanging = false;
    //DASH
    [SerializeField] private float _dashSpeed = 20;
    private bool _dashing = false;
    float _dashTimer;
    [SerializeField] float _dashDuration = 0.5f;
    //AttackChica
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform _shooter;
    private float ShootTimer;
    private float ShootColdown = 0.2f;
    private bool canShoot = true;
    //AttackChico
    [SerializeField] Transform _RangeSensor;
    [SerializeField] float _RangeRadius = 2;
    [SerializeField] LayerMask _enemyMask;
    private float SwingTimer;
    private float SwingColdown = 0.5f;
    private bool canSwing = true;
    //HabilityChica
    private float SpinTimer;
    private float SpinColdown = 10;
    private bool canSpin = true;
    [SerializeField] private float SpinRange;
    [SerializeField] private Transform _SpinSensor;
    //HabilidadChico
    private bool canInvoke = true;
    private float InvokeTimer;
    private float InvokeColdown = 10;
    [SerializeField] private GameObject _spiritPrefab;
    [SerializeField] private Transform _invoker;
    //UltiChica
    [SerializeField] private GameObject UltiPrefab;
    private float _UltiTimer;
    private float _UltiColdow = 100;
    private bool canUltiChica = true;
    //UltiChico
    private float _C_UltiTimer;
    private float _C_UltiColdow = 100;
    private bool canCedricUlti = true;
    [SerializeField] private float _UltiRange = 10;
    [SerializeField] float CedricUltiDuration = 5;
    [SerializeField] bool _C_UltiFinished = true;
    //Gravity
    [SerializeField] private Transform _sensorPosition;
    [SerializeField] private float _sensorRadius;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] Vector3 _playerGravity;
    //Interact
    [SerializeField] Transform _interactSensor;
    [SerializeField] float _interactRadius = 3.5f;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _moveAction = InputSystem.actions["Move"];
        _lookAction = InputSystem.actions["Look"];
        _changeAction = InputSystem.actions["Change"];
        _AttackAction = InputSystem.actions["Attack"];
        _dashAction = InputSystem.actions["Jump"];
        _interactAction = InputSystem.actions["Interact"];
        _SpecialAction = InputSystem.actions["SpecialAttack"];
        _UltiAction = InputSystem.actions["Ulti"];
        _mainCamera = Camera.main.transform;
        _animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        _dashing = false;
    }

    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        _lookInput = _lookAction.ReadValue<Vector2>();
        
        Movimiento();
        Gravity();

        if(_changeAction.WasPressedThisFrame() && _isChanging == false)
        {
            _isChanging = true;
            StartCoroutine(Change());
        }
        if(_AttackAction.WasPressedThisFrame())
        {
            StartCoroutine(Attack());
        }
        if(_SpecialAction.WasPressedThisFrame())
        {
            StartCoroutine(SpecialAttack());
        }
        if(_UltiAction.WasPressedThisFrame())
        {
            StartCoroutine(Ulti());
        }

        if(_dashAction.WasPressedThisFrame() && _dashing == false)
        {
            StartCoroutine(Dash());
        }

        if(_interactAction.WasPressedThisFrame())
        {
            Interact();
        }
    }

    void Movimiento()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        Ray ray = Camera.main.ScreenPointToRay(_lookInput);
        RaycastHit hit;

        if(_C_UltiFinished != true)
        {
            return;
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 playerForward = hit.point - transform.position;
            playerForward.y = 0;
            transform.forward = playerForward;
        }

        if (direction != Vector3.zero)
        {
            _controller.Move(direction.normalized * _movementSpeed * Time.deltaTime);
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
        }
    }

    IEnumerator Attack()
    {
        if(_ChicaActive && canShoot == true)
        {
            if(canShoot)
            {
                Instantiate(projectilePrefab, _shooter.position, _shooter.rotation);
                canShoot = false;
            }

            while (ShootTimer < ShootColdown)
            {
                ShootTimer += Time.deltaTime;
                yield return null;
            }

            ShootTimer = 0;
            canShoot = true;
        }
        else if(_ChicoActive && canSwing == true)
        {
            canSwing = false;
            Collider[] enemiesInRange = Physics.OverlapSphere(_RangeSensor.position, _RangeRadius, _enemyMask);
            foreach (Collider enemy in enemiesInRange)
            {
                IDamageable damageable = enemy.gameObject.GetComponent<IDamageable>();
                if(damageable != null)
                {
                    damageable.TakeDamage(5);
                }
            }

            while (SwingTimer < SwingColdown)
            {   
                SwingTimer += Time.deltaTime;
                yield return null;
            }

            SwingTimer = 0;
            canSwing = true;
        }
    }

    IEnumerator SpecialAttack()
    {
        if(_ChicaActive && canSpin == true)
        {
            canSpin = false;
            Collider[] enemiesInRange = Physics.OverlapSphere(_SpinSensor.position, SpinRange, _enemyMask);
            Debug.Log(enemiesInRange.Length);
            foreach (Collider enemy in enemiesInRange)
            {
                IDamageable damageable = enemy.gameObject.GetComponent<IDamageable>();
                if(damageable != null)
                {
                    damageable.TakeDamage(10);
                }
            }

            while (SpinTimer < SpinColdown)
            {   
                SpinTimer += Time.deltaTime;
                yield return null;
            }

            SpinTimer = 0;
            canSpin = true;
        }

        if(_ChicoActive && canInvoke == true)
        {
            canInvoke = false;
            Instantiate(_spiritPrefab, _invoker.position, _invoker.rotation);

            while (InvokeTimer < InvokeColdown)
            {   
                InvokeTimer += Time.deltaTime;
                yield return null;
            }

            InvokeTimer = 0;
            canInvoke = true;
            
        }
    }

    IEnumerator Ulti()
    {
        if(_ChicaActive && canUltiChica == true)
        {
            if(canUltiChica)
            {
                Debug.Log("Ulti");
                Instantiate(UltiPrefab, _shooter.position, _shooter.rotation);
                canUltiChica = false;
                Debug.Log("Instantiate");
            }

            while (_UltiTimer < _UltiColdow)
            {
                _UltiTimer += Time.deltaTime;
                yield return null;
            }

            ShootTimer = 0;
            canUltiChica = true;
        }
        if(_ChicoActive && canCedricUlti == true)
        {
            canCedricUlti = false;
            _C_UltiFinished = false;
            InputSystem.actions.FindActionMap("Player").Disable();
            while (_C_UltiTimer < CedricUltiDuration)
            {
                _C_UltiTimer += Time.deltaTime;
                Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, _UltiRange, _enemyMask);
                foreach (Collider enemy in enemiesInRange)
                {
                    IDamageable damageable = enemy.gameObject.GetComponent<IDamageable>();
                    if(damageable != null)
                    {
                        damageable.TakeDamage(5);
                    }
                }

                yield return null;
            }
            _C_UltiFinished = true;
            InputSystem.actions.FindActionMap("Player").Enable();
            yield return new WaitForSeconds(_C_UltiColdow);
            _C_UltiTimer = 0;
            canCedricUlti = true;
        }
    }

    IEnumerator Dash()
    {
        _dashing = true;
        Physics.IgnoreLayerCollision(0, 6, true);

        Vector3 dashDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;

        while (_dashTimer < _dashDuration)
        {
            
            _dashTimer += Time.deltaTime;
            _controller.Move(dashDirection.normalized * _dashSpeed * Time.deltaTime);
            yield return null;
        }

        Physics.IgnoreLayerCollision(0, 6, false);
        _dashTimer = 0;
        _dashing = false;
    }

    IEnumerator Change()
    {
        if(_ChicaActive)
        {
            _ChicaActive = false;
            _ChicoActive = true;
            _Chica.SetActive(false);
            _Chico.SetActive(true);
            yield return new WaitForSeconds(1);
            _isChanging = false;
        }
        else if(_ChicoActive)
        {
            _ChicoActive = false;
            _ChicaActive = true;
            _Chico.SetActive(false);
            _Chica.SetActive(true);
            yield return new WaitForSeconds(1);
            _isChanging = false;
        }
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);
    }

    void Gravity()
    {
       if(!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if(IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = _gravity;
        }
        _controller.Move(_playerGravity * Time.deltaTime);
    }

    void Interact()
    {
        Collider[] objectsToInteract = Physics.OverlapSphere(_interactSensor.position, _interactRadius);
        foreach (Collider item in objectsToInteract)
        {
            if(item.gameObject.tag == "Interactable")
            {
                Debug.Log("Interacted");
                return;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_sensorPosition.position, _sensorRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_interactSensor.position, _interactRadius);

        if(_ChicaActive)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_SpinSensor.position, SpinRange);
        }
        if(_ChicoActive)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_RangeSensor.position, _RangeRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _UltiRange);
        }
    }
}
