using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    
    [Header("References")]
    [SerializeField] private Animator _animator;
    
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    
    private void Update()
    {
        Vector2 input = InputManager.Instance.MovementDirection;
        
        UpdateAnimator(input);
        
        if (input == Vector2.zero) return;
        
        Move(input);
        Rotate(input);
    }
    
    private void UpdateAnimator(Vector2 input)
    {
        bool isMoving = input != Vector2.zero;
        _animator.SetBool(IsRunning, isMoving);
    }
    
    private void Move(Vector2 input)
    {
        Vector3 movement = new Vector3(input.x, 0f, input.y) * (_moveSpeed * Time.deltaTime);
        transform.position += movement;
    }
    
    private void Rotate(Vector2 input)
    {
        Vector3 direction = new Vector3(input.x, 0f, input.y);
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        
        transform.rotation = Quaternion.Lerp(
            transform.rotation, 
            targetRotation, 
            _rotationSpeed * Time.deltaTime
        );
    }
}