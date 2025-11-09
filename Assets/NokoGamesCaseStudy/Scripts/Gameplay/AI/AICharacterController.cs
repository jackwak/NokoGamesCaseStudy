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

    [Header(" Datas ")]
    private AIState _currentState;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private int _isWalkingHash;

    void Awake()
    {
        _currentState = AIState.Idle;
        _isWalkingHash = Animator.StringToHash("IsRunning");
        
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    void Start()
    {
        StartCoroutine(AILoop());
    }

    private IEnumerator AILoop()
    {
        while (true)
        {
            switch (_currentState)
            {
                case AIState.Idle:
                    HandleIdleState();
                    break;

                case AIState.MovingToCollect:
                    yield return MoveToArea(_collectArea);
                    _currentState = AIState.Collecting;
                    break;

                case AIState.Collecting:
                    yield return WorkAtArea(_collectArea, _workDuration);
                    _currentState = AIState.Idle;
                    break;

                case AIState.MovingToDrop:
                    yield return MoveToArea(_dropArea);
                    _currentState = AIState.Dropping;
                    break;

                case AIState.Dropping:
                    yield return WorkAtArea(_dropArea, _workDuration);
                    _currentState = AIState.Idle;
                    break;

                case AIState.WaitingAtIdle:
                    yield return WaitAtIdlePosition();
                    break;
            }

            yield return null;
        }
    }

    private void HandleIdleState()
    {
        if (_collectController.ItemCount > 0)
        {
            if (CanDropToArea())
            {
                _currentState = AIState.MovingToDrop;
            }
            else
            {
                _currentState = AIState.WaitingAtIdle;
            }
            return;
        }

        if (CanCollectFromArea())
        {
            _currentState = AIState.MovingToCollect;
            return;
        }
    }

    private bool CanCollectFromArea()
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

    private bool CanDropToArea()
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

    private IEnumerator MoveToArea(BaseItemHolderArea area)
    {
        if (area == null) yield break;

        SetWalkingState(true);
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

        SetWalkingState(false);
    }

    private IEnumerator WaitAtIdlePosition()
    {
        if (Vector3.Distance(transform.position, _initialPosition) > 0.5f)
        {
            yield return ReturnToIdlePosition();
        }

        while (true)
        {
            yield return new WaitForSeconds(_checkInterval);

            if (CanDropToArea())
            {
                _currentState = AIState.Idle;
                yield break;
            }
        }
    }

    private IEnumerator ReturnToIdlePosition()
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

    private IEnumerator WorkAtArea(BaseItemHolderArea area, float duration)
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

    private void SetWalkingState(bool isWalking)
    {
        _animator?.SetBool(_isWalkingHash, isWalking);
    }
}

public enum AIState
{
    Idle,
    MovingToCollect,
    Collecting,
    MovingToDrop,
    Dropping,
    WaitingAtIdle
}