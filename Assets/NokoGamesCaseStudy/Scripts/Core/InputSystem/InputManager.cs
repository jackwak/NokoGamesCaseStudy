using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public Vector2 MovementDirection { get; private set; }

    private PlayerInput _input;
    private Vector2 _touchStartPos;
    private bool _isTouching;

    [Header("Settings")]
    [SerializeField] private float _deadZone = 10f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        _input = new PlayerInput();
    }

    private void OnEnable()
    {
        _input.Enable();

        _input.Player.TouchContact.started += OnTouchStarted;
        _input.Player.TouchContact.canceled += OnTouchCanceled;
    }

    private void OnDisable()
    {
        _input.Player.TouchContact.started -= OnTouchStarted;
        _input.Player.TouchContact.canceled -= OnTouchCanceled;
        _input.Disable();
    }

    private void OnTouchStarted(InputAction.CallbackContext ctx)
    {
        _isTouching = true;
        _touchStartPos = _input.Player.TouchPosition.ReadValue<Vector2>();
    }

    private void OnTouchCanceled(InputAction.CallbackContext ctx)
    {
        _isTouching = false;
        MovementDirection = Vector2.zero;
    }

    private void Update()
    {
        if (!_isTouching)
        {
            MovementDirection = Vector2.zero;
            return;
        }

        Vector2 current = _input.Player.TouchPosition.ReadValue<Vector2>();
        Vector2 delta = current - _touchStartPos;

        MovementDirection = (delta.magnitude < _deadZone) ? Vector2.zero : delta.normalized;
    }
}
