using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    enum MovementState { Grounded, Airborne, Dashing, Wallrunning, Vaulting, Grappling }

    [SerializeField]
    MovementState currentState;
    [Range(0.0f, 50.0f)]
    public float jumpModifier;
    [Range(10.0f, 100.0f)]
    public float moveModifier;
    [Range(0.0f, 100.0f)]
    public float gravityModifier;
    [Range(0.0f, 1f)]
    public float movementSmoothing;
    [Range(0.0f, 100.0f)]
    public float mouseSensitivity;
    public int jumpBuffer;
    public Transform playerChamera;
    
    public Vector3 Velocity = Vector3.zero;
    public InputActionAsset i_asset;
    public CharacterController p_characterController;
    private InputAction i_moveAction, i_lookAction, i_jumpAction;
    private int fixedFrameCounter;
    private float yMouseRotation;
    private Vector3 currentVector = Vector3.zero;
    private Vector2 movementVector, lookVector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        i_moveAction = i_asset.FindAction("Move");
        i_lookAction = i_asset.FindAction("Look");
        i_jumpAction = i_asset.FindAction("Jump");

        
    }
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        pJump();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        readMovementsVectors();
        pLook();
        pMovement();

    }

    private void pJump()
    {
        if (i_jumpAction.WasPressedThisFrame())
        {
            switch (currentState)
            {
                case MovementState.Grounded:                
                    Velocity = Vector3.up * jumpModifier;
                    currentState = MovementState.Airborne;
                    break;
                case MovementState.Airborne:
                    if (fixedFrameCounter <= jumpBuffer)
                    {
                        Velocity = Vector3.up * jumpModifier;
                        currentState = MovementState.Airborne;
                    }
                break; 
                
            }
            //p_rigidbody.AddForceAtPosition(new Vector3(0, jumpModifier, 0), transform.position, ForceMode.Impulse);
            //Debug.Log($"Move:{i_moveAction.ReadValue<Vector2>()} | Look:{i_lookAction.ReadValue<Vector2>()} | Jump:{i_jumpAction.WasPressedThisFrame()}");
        }

    }
    private void pLook()
    {
        float mouseX = lookVector.x * mouseSensitivity * Time.fixedDeltaTime;
        float mouseY = lookVector.y * mouseSensitivity * Time.fixedDeltaTime;

        transform.Rotate(Vector3.up * mouseX);

        yMouseRotation -= mouseY;
        yMouseRotation = Mathf.Clamp(yMouseRotation, -30, 60);
        //playerChamera. = Quaternion.Euler(new Vector3(yMouseRotation, 0f, 0f));
        // Berechne neue Kameraposition
        Vector3 offset = Quaternion.Euler(yMouseRotation, transform.eulerAngles.y, 0) * new Vector3(0, 0, -4);
        playerChamera.position = transform.position + offset;
        playerChamera.LookAt(transform.position + Vector3.up * 1.5f);

    }
    private void pMovement()
    {

        if (!p_characterController.isGrounded)
        {
            fixedFrameCounter++;
            Velocity.y += -9.81f * gravityModifier * Time.fixedDeltaTime;
            Velocity.y = Mathf.Clamp(Velocity.y, -50f, 50f);
            currentState = MovementState.Airborne;
        }
        else if (Velocity.y < -1f)
        {
            fixedFrameCounter = 0;
            Velocity.y = -1f;
            currentState = MovementState.Grounded;
        }

        //Vector3 targetMove = new Vector3(movementVector.x, 0, movementVector.y) * moveModifier;
        //Vector3 smoothedMove = Vector3.Lerp(currentVector, targetMove, movementSmoothing);
        //currentVector = smoothedMove;

        Vector3 inputDir = new Vector3(movementVector.x, 0f, movementVector.y);
        Vector3 targetMove = inputDir * moveModifier;

        // Optional mit Blickrichtung:
        targetMove = transform.TransformDirection(targetMove);

        // Sanftes Angleichen:
        currentVector = Vector3.Lerp(currentVector, targetMove, movementSmoothing);

        p_characterController.Move((currentVector + Velocity) * Time.fixedDeltaTime);


        //p_characterController.Move(Velocity * Time.fixedDeltaTime);
        //p_rigidbody.MovePosition(p_rigidbody.position + transform.forward * movementVector.y * moveModifier * Time.deltaTime);
    }

    private void readMovementsVectors()
    {
        movementVector = i_moveAction.ReadValue<Vector2>();
        lookVector = i_lookAction.ReadValue<Vector2>();
    }
}
