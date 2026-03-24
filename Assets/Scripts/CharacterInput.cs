using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

//Для инвентаря
[System.Serializable]
public struct Stats
{
    public static Stats Sum(Stats stats1, Stats stats2)
    {
        var result = new Stats();
        //result.speed = stats1.speed + stats2.speed;
        result.EnergyPercent = stats1.EnergyPercent + stats2.EnergyPercent;
        return result;
    }
    public static Stats Subtract(Stats stats1, Stats stats2)
    {
        var result = new Stats();
        //result.speed = stats1.speed - stats2.speed;
        result.EnergyPercent = stats1.EnergyPercent - stats2.EnergyPercent;
        return result;
    }

    //[SerializeField, Range(0, 50)]
    //public float speed;

    [SerializeField]
    public float EnergyPercent;

}

[RequireComponent(typeof(CharacterController))]
[RequireComponent (typeof(CharacterController))]
public class CharacterInput : MonoBehaviour
{
    [SerializeField]
    public Stats stats = new Stats()
    {
        //speed = 3,
        EnergyPercent = 100
    };
    public GameObject phoneImg;
    public static CharacterInput Current { get; private set; } = null;
    [SerializeField]
    private bool activateInput = true;
    [SerializeField]
    private InputActionReference moveAction;
    [SerializeField]
    private InputActionReference lookAction;
    [SerializeField]
    private InputActionReference phoneAction;
    //[SerializeField, Range(0, 100)]
    //private float sence = 10f;
    private PlayerInput _input;
    [SerializeField, Range(0, 2)]
    [Tooltip("Mouse sensitivity for camera rotation")]
    private float characterSense = 0.1f;
    [SerializeField, Range(0, 100)]
    private float speed = 5f;

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI energyText;

    [Space]
    [SerializeField]
    private CinemachineCamera head;

    [Space]
    [SerializeField]
    private GameObject phoneCanvas;

    [SerializeField, Range(0, 50)]
    [Tooltip("Sprint movement speed")]
    private float sprintSpeed = 10;
    private float _controllerHitResetTimeout = 0;
    private float originalTimeScale = 1f;

    private InteractableOutline _currentHighlightedOutline;
    private RaycastHit _currentHit;

    private CharacterController characterController;

    [SerializeField]
    private LayerMask interactableLayer;

    private Vector3 _localMovementAccelerationVector = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _resultMovementDirection = Vector3.zero;
    private Vector3 _movementDirection = Vector3.zero;
    private Vector2 _lookDirection = Vector2.zero;

    private bool _sprintState = false;
    private bool isPhoneOpen = false;
    public bool IsGrounded { get; private set; } = false;

    /// <summary>
    /// Starts player control and locks cursor.
    /// </summary>
    public void StartControlling()
    {
        if (!isPhoneOpen)
        {
            _input.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Stops player control and unlocks cursor.
    /// </summary>
    public void StopControlling()
    {
        if (!isPhoneOpen)
        {
            _input.enabled = false;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void Start()
    {
        UpdateEnergyText();
    }
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();

        StartControlling();
        Current = this;
    }
    private void OnEnable()
    {
        characterController = GetComponent<CharacterController>();

        if (activateInput)
        {
            moveAction.action.Enable();
            lookAction.action.Enable();
            phoneAction.action.Enable();
        }
        phoneAction.action.performed += OnPhonePerformed;



        if (phoneCanvas != null)
        {
            phoneCanvas.SetActive(false);
        }
    }
    private void OnDisable()
    {
        phoneAction.action.performed -= OnPhonePerformed;
        StartControlling();
    }

    private void OnPhonePerformed(InputAction.CallbackContext context)
    {
        TogglePhone();
    }

    private void TogglePhone()
    {
        isPhoneOpen = !isPhoneOpen;

        if (phoneCanvas != null)
        {
            phoneCanvas.SetActive(isPhoneOpen);

            if (isPhoneOpen)
            {
                StopControlling();
                originalTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                StartControlling();
                Time.timeScale = originalTimeScale;
            }
        }
    }
    private void FixedUpdate()
    {
        if (!head.IsLive || isPhoneOpen)
            return;
        ResetCollisionData();
        CalculateVelocity(ref _velocity);

        _resultMovementDirection = _velocity * 5f + CalculateMovementDirection();
    }

    private void LateUpdate()
    {
        if (!head.IsLive || isPhoneOpen)
            return;

        UpdateInteractionRaycast();
        float timescale = Time.deltaTime * 20f;

        transform.rotation =
            Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, _lookDirection.x, 0),
                timescale);

        head.transform.localRotation =
            Quaternion.Lerp(
                head.transform.localRotation,
                Quaternion.Euler(-_lookDirection.y, 0, 0),
                timescale);

        characterController.Move(_resultMovementDirection * Time.deltaTime);
    }
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !isPhoneOpen)
        {
            TryInteractWithCurrentHit();
        }
    }
    private void UpdateEnergyText()
    {
        if (energyText != null)
        {
            energyText.text = $"{stats.EnergyPercent:F0}";
        }
    }
    private void TryInteractWithCurrentHit()
    {
        if (_currentHit.collider != null)
        {
            GameObject hitObject = _currentHit.collider.gameObject;

            if (hitObject.CompareTag("Phone") ||
                (hitObject.transform.parent != null && hitObject.transform.parent.CompareTag("Phone")))
            {
                if (phoneImg != null)
                {
                    phoneImg.gameObject.SetActive(true);
                    hitObject.SetActive(false);
                }
            }
            else if (hitObject.CompareTag("Energy") ||
                     (hitObject.transform.parent != null && hitObject.transform.parent.CompareTag("Energy")))
            {
                ItemContainer itemContainer = hitObject.GetComponent<ItemContainer>();
                if (itemContainer == null && hitObject.transform.parent != null)
                {
                    itemContainer = hitObject.transform.parent.GetComponent<ItemContainer>();
                }

                if (itemContainer != null && itemContainer.item is Energy energyItem)
                {
                    float energyAmount = energyItem.stats.EnergyPercent;

                    stats.EnergyPercent += energyAmount;

                    if (stats.EnergyPercent > 100)
                        stats.EnergyPercent = 100;
                    UpdateEnergyText();

                    Destroy(hitObject);

                    Debug.Log($"Energy added! +{energyAmount} energy. Current energy: {stats.EnergyPercent}");
                }
            }
            else
            {
                Interactable interactable = _currentHit.collider.GetComponent<Interactable>();
                if (interactable == null && _currentHit.collider.transform.parent != null)
                {
                    interactable = _currentHit.collider.transform.parent.GetComponent<Interactable>();
                }

                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
    private void ResetCollisionData()
    {
        _controllerHitResetTimeout -= Time.fixedDeltaTime;

        if (_controllerHitResetTimeout < 0)
        {
            IsGrounded = false;
        }
    }
    private Vector3 CalculateMovementDirection()
    {
        if (head.IsLive)
        {
            _localMovementAccelerationVector = Vector3.Lerp(_localMovementAccelerationVector, transform.rotation * _movementDirection * (_sprintState ? sprintSpeed : speed), (IsGrounded ? 10 : 1) * Time.fixedDeltaTime);
        }
        else
        {
            _localMovementAccelerationVector = Vector3.zero;
        }

        return _localMovementAccelerationVector;
    }

    private void CalculateVelocity(ref Vector3 velocity)
    {
        velocity = Vector3.Lerp(velocity, Physics.gravity, Time.fixedDeltaTime);
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _controllerHitResetTimeout = 0.1f;

        IsGrounded = Vector3.Angle(hit.normal, Vector3.up) <= 35;

        Quaternion normalAngle = Quaternion.FromToRotation(hit.normal, Vector3.down);

        Vector3 deltaVelocity = normalAngle * _velocity;
        deltaVelocity.y = Mathf.Min(0, deltaVelocity.y);

        if (IsGrounded)
        {
            deltaVelocity.x = 0;
            deltaVelocity.z = 0;
        }

        _velocity = Quaternion.Inverse(normalAngle) * deltaVelocity;
    }
    private void OnMove(InputValue inputValue)
    {
        Vector2 input = inputValue.Get<Vector2>();
        _movementDirection = new Vector3(input.x, 0, input.y);
    }

    private void OnLook(InputValue inputValue)
    {
        _lookDirection += inputValue.Get<Vector2>() * characterSense;

        _lookDirection.y = Mathf.Clamp(_lookDirection.y, -89, 89);
    }

    private void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed && IsGrounded)
        {
            _velocity = Vector3.up * 3;
        }
    }
    private void UpdateInteractionRaycast()
    {
        Ray ray = new Ray(head.transform.position, head.transform.forward);

        if (_currentHighlightedOutline != null)
        {
            _currentHighlightedOutline.DisableHighlight();
            _currentHighlightedOutline = null;
        }

        if (Physics.Raycast(ray, out _currentHit, 5f, interactableLayer))
        {
            InteractableOutline outline = FindInteractableOutline(_currentHit.collider.gameObject);

            if (outline != null)
            {
                _currentHighlightedOutline = outline;
                _currentHighlightedOutline.EnableHighlight();
            }
        }
    }
    private InteractableOutline FindInteractableOutline(GameObject hitObject)
    {
        if (hitObject == null) return null;

        InteractableOutline outline = hitObject.GetComponent<InteractableOutline>();
        if (outline != null)
            return outline;

        if (hitObject.transform.parent != null)
        {
            outline = hitObject.transform.parent.GetComponent<InteractableOutline>();
            if (outline != null)
                return outline;
        }

        Transform current = hitObject.transform.parent;
        while (current != null)
        {
            outline = current.GetComponent<InteractableOutline>();
            if (outline != null)
                return outline;
            current = current.parent;
        }

        outline = hitObject.GetComponentInChildren<InteractableOutline>();
        if (outline != null)
            return outline;

        return null;
    }
}