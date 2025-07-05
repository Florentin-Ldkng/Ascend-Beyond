using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Range(0.0f, 50.0f)]
    public float jumpModifier;
    [Range(10.0f, 100.0f)]
    public float moveModifier;

    public InputActionAsset i_asset;
    public CharacterController p_characterController;
    private InputAction i_moveAction, i_lookAction, i_jumpAction;

    private Vector2 movementVector, lookVector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        i_moveAction = i_asset.FindAction("Move");
        i_lookAction = i_asset.FindAction("Look");
        i_jumpAction = i_asset.FindAction("Jump");

    }

    void Update()
    {
        pJump();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        readMovementsVectors();
        pMovement();

    }

    private void pJump()
    {
        if (i_jumpAction.WasPressedThisDynamicUpdate())
        {
            p_characterController.Move(Vector3.up * 10);
            //p_rigidbody.AddForceAtPosition(new Vector3(0, jumpModifier, 0), transform.position, ForceMode.Impulse);
            //Debug.Log($"Move:{i_moveAction.ReadValue<Vector2>()} | Look:{i_lookAction.ReadValue<Vector2>()} | Jump:{i_jumpAction.WasPressedThisFrame()}");
        }

    }
    private void pMovement()
    {
        //p_rigidbody.MovePosition(p_rigidbody.position + transform.forward * movementVector.y * moveModifier * Time.deltaTime);
    }

    private void readMovementsVectors()
    {
        movementVector = i_moveAction.ReadValue<Vector2>();
        lookVector = i_lookAction.ReadValue<Vector2>();
    }
}
