using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterController : MonoBehaviour
{
    [Header(" References ")]
    [SerializeField] private PlayerCollectController _collectController;
    [SerializeField] private Animator _animator;

    [Header(" AI Settings ")]
    [SerializeField] private BaseItemHolderArea _collectArea;
    [SerializeField] private BaseItemHolderArea _dropArea;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _workDuration = 3f;
    [SerializeField] private float _checkInterval = 1f;
    [SerializeField] private float _aiStoppingDistance = 1f;
    [SerializeField] private List<ItemType> _allowedItemTypes = new List<ItemType>();

    private IAIState _currentState;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private int _isWalkingHash;
    private Coroutine _stateCoroutine;

    public PlayerCollectController CollectController => _collectController;
    public BaseItemHolderArea CollectArea => _collectArea;
    public BaseItemHolderArea DropArea => _dropArea;
    public float WorkDuration => _workDuration;
    public float CheckInterval => _checkInterval;
    public Vector3 InitialPosition => _initialPosition;

    private void Awake()
    {
        _isWalkingHash = Animator.StringToHash("IsRunning");
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    private void Start()
    {
        ChangeState(new IdleState());
    }

    public void ChangeState(IAIState newState)
    {
        if (_stateCoroutine != null)
        {
            StopCoroutine(_stateCoroutine);
        }

        _currentState?.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
        _stateCoroutine = StartCoroutine(_currentState.Execute(this));
    }

    public bool CanCollectFromArea()
    {
        if (_collectArea == null || _collectArea.ItemCount <= 0)
            return false;

        Item areaItem = _collectArea.GetLastItem();
        if (areaItem == null)
            return false;

        if (_allowedItemTypes.Count == 0)
            return true;

        return _allowedItemTypes.Contains(areaItem.ItemType);
    }

    public bool CanDropToArea()
    {
        if (_dropArea == null || _collectController.ItemCount <= 0)
            return false;

        Vector3? availablePosition = _dropArea.GetAvaiblePosition();
        if (availablePosition == null)
            return false;

        Item myItem = _collectController.GetTopItem();
        if (myItem != null && !_dropArea.IsCorrectArea(myItem.ItemType))
            return false;

        return true;
    }

    public IEnumerator MoveToArea(BaseItemHolderArea area)
    {
        if (area == null) yield break;

        Vector3 targetPosition = area.transform.position;

        while (Vector3.Distance(transform.position, targetPosition) > _aiStoppingDistance)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * _moveSpeed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }

    public IEnumerator ReturnToIdlePosition()
    {
        SetWalkingState(true);

        while (Vector3.Distance(transform.position, _initialPosition) > 0.1f)
        {
            Vector3 direction = (_initialPosition - transform.position).normalized;
            transform.position += direction * _moveSpeed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }

            yield return null;
        }

        transform.position = _initialPosition;

        float rotationTimer = 0f;
        float rotationDuration = 0.5f;
        Quaternion startRotation = transform.rotation;

        while (rotationTimer < rotationDuration)
        {
            rotationTimer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, _initialRotation, rotationTimer / rotationDuration);
            yield return null;
        }

        transform.rotation = _initialRotation;
        SetWalkingState(false);
    }

    public IEnumerator WorkAtArea(BaseItemHolderArea area, float duration)
    {
        if (area == null) yield break;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (area.ItemHolderType == ItemHolderType.Collect)
            {
                if (!CanCollectFromArea() || _collectController.ItemCount >= _collectController.MaxItemCollectCount)
                    break;
            }
            else if (area.ItemHolderType == ItemHolderType.Drop)
            {
                if (_collectController.ItemCount <= 0)
                    break;
            }

            yield return null;
        }
    }

    public void SetWalkingState(bool isWalking)
    {
        _animator?.SetBool(_isWalkingHash, isWalking);
    }
}