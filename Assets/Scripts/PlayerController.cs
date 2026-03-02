using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    #region Variables
    //Components
    private CharacterController _controller;
    private Animator _animator;
    //Inputs
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _attackAction;
    private InputAction _dashAction;
    private InputAction _interactAction;
    private InputAction _changeAction;
    //InputDependences
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    //Gravity
    [SerializeField] private Transform _sensorPosition;
    [SerializeField] private float _sensorRadius;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] Vector3 _playerGravity;
    //Stats
    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private float _smoothTime = 0.1f;
    private float _turnSmoothVelocity;
    //Camera
    private Transform _mainCamera;
    float targetAngle;
    //CharacterChange
    [SerializeField] private GameObject _Chico;
    [SerializeField] private GameObject _Chica;
    [SerializeField] private bool _ChicoActive = false;
    [SerializeField] private bool _ChicaActive = true;
    private bool _isChanging = false;
    //Dash
    [SerializeField] private float _dashSpeed = 20;
    private bool _dashing = false;
    float _dashTimer;
    [SerializeField] float _dashDuration = 0.5f;
    //ThalyaAttack
    //
    //CedricAttack
    //
    
    #endregion
    #region Awake
    
    void Awake()
    {
        _mainCamera = Camera.main.transform;
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _moveAction = InputSystem.actions["Move"];
        _lookAction = InputSystem.actions["Look"];
        _changeAction = InputSystem.actions["Change"];
        _attackAction = InputSystem.actions["Attack"];
        _dashAction = InputSystem.actions["Jump"];
        _interactAction = InputSystem.actions["Interact"];
    }

    #endregion
    #region Start

    void Start()
    {
        _dashing = false;
    }

    #endregion
    #region Update

    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        _lookInput = _lookAction.ReadValue<Vector2>();
        
        Movement();
        Gravity();
    }

    #endregion
    #region Movement

    void Movement()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        Ray ray = Camera.main.ScreenPointToRay(_lookInput);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
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

    #endregion
    #region Gravity

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

    #endregion
    #region Grounded

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);
    }

    #endregion


}
