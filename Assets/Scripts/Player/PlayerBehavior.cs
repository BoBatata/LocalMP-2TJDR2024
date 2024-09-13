using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehavior : MonoBehaviour
{
    private InputActionAsset inputAction;
    private InputActionMap playerActionMap;
    private InputAction moveAction;

    [SerializeField] private float velocity = 10;
    [SerializeField] private float rotationvelocity = 10;
    [SerializeField] private float moveSpeed = 10;

    private float initialVelocity;

    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 inputData => moveAction.ReadValue<Vector2>();


    private void Awake()
    {
        inputAction = GetComponent<PlayerInput>().actions;
        playerActionMap = inputAction.FindActionMap("Player");
        moveAction = playerActionMap.FindAction("Move");
        initialVelocity = velocity;
        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        HandleMove();
    }

    private void HandleMove()
    {
        
        moveDirection.x = inputData.x;
        moveDirection.z = inputData.y;
        Vector3 cameraRelativeMovement =
           ConvertMoveDirectionToCameraSpace(moveDirection);

        characterController.SimpleMove(cameraRelativeMovement *
                                 velocity);
        RotatePlayerAccordingToInput(cameraRelativeMovement);
    }

    private void RotatePlayerAccordingToInput(Vector3 cameraRelativeMovement)
    {
        Vector3 pointToLookAt;
        pointToLookAt.x = cameraRelativeMovement.x;
        pointToLookAt.y = 0;
        pointToLookAt.z = cameraRelativeMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(pointToLookAt);

            transform.rotation =
                Quaternion.Slerp(currentRotation,
                                 targetRotation,
                                 rotationvelocity * Time.deltaTime);
        }

    }

    private Vector3 ConvertMoveDirectionToCameraSpace(Vector3 moveDirection)
    {
        if(Camera.main == null) return Vector3.zero;
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        Vector3 cameraForwardZProduct = cameraForward * moveDirection.z;
        Vector3 cameraRightXProduct = cameraRight * moveDirection.x;

        Vector3 directionToMovePlayer =
            cameraForwardZProduct + cameraRightXProduct;

        return directionToMovePlayer;
    }
}
