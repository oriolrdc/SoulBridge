using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    #region Variables
    //Components
    private CharacterController _controller;
    //Inputs
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _changeAction;
    private InputAction _dashAction;
    private Vector2 _lookInput;
    private Vector2 _moveInput;
    //Stats
    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private float _smoothTime = 0.1f;
    //Cedric
    [SerializeField] GameObject _Cedric;
    [SerializeField] bool _CAct;
    //Thalya
    [SerializeField] GameObject _Thalya;
    [SerializeField] bool _TAct;
    //Dash
    [SerializeField] private float _dashSpeed = 20;
    private bool _dashing = false;
    float _dashTimer;
    [SerializeField] float _dashDuration = 0.5f;
    //Dependences
    private bool _isChanging;
    private float _turnSmoothVelocity;
    private float targetAngle;
    //Gravity
    [SerializeField] private Transform _groundSensor;
    [SerializeField] private float _groundSensorRadius;
    [SerializeField] private LayerMask _groundLayer;
    private float _gravity = -9.81f;
    Vector3 _playerGravity;
    
    #endregion
    #region Awake

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _moveAction = InputSystem.actions["Move"];
        _lookAction = InputSystem.actions["Look"];
        _changeAction = InputSystem.actions["Change"];
        _dashAction = InputSystem.actions["Dash"];
    }

    #endregion
    #region Update

    void Update()
    {
        //Inputs
        _moveInput = _moveAction.ReadValue<Vector2>();
        _lookInput = _lookAction.ReadValue<Vector2>();
        //Functions
        Movement();

        if(_changeAction.WasPressedThisFrame() && _isChanging == false)
        {
            _isChanging = true;
            StartCoroutine(Change());
        }
        if(_dashAction.WasPressedThisFrame() && _dashing == false)
        {
            StartCoroutine(Dash());
        }
    }

    #endregion
    #region FixedUpdate (Physics)

    void FixedUpdate()
    {
        Gravity();
    }

    #endregion
    #region IsGrounded

    bool IsGrounded()
    {
        return Physics.CheckSphere(_groundSensor.position, _groundSensorRadius, _groundLayer);
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
    #region Change

    IEnumerator Change()
    {
        if(_TAct)
        {
            _TAct = false;
            _CAct = true;
            _Thalya.SetActive(false);
            _Cedric.SetActive(true);
            yield return new WaitForSeconds(1);
            _isChanging = false;
        }
        else if(_CAct)
        {
            _CAct = false;
            _TAct = true;
            _Cedric.SetActive(false);
            _Thalya.SetActive(true);
            yield return new WaitForSeconds(1);
            _isChanging = false;
        }
    }

    #endregion
    #region Dash

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

    #endregion
}
