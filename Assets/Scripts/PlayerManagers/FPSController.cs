using UnityEngine;

public class FPSController : MonoBehaviour
{
    //public properties
    [Header("Properties")]
    [SerializeField]float speed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float crouchSpeed;

    [SerializeField] float crouchHeight;
    float normalHeight;
    float heightDiff;
    Vector3 groundCheckPos;
    [SerializeField]Vector3 groundCheckCrouchPos;

    [SerializeField]float sensivity;
    [SerializeField] float jumpForce;

    [Header("Setup")]
    CharacterController controller;
    CapsuleCollider col;
    Transform playerTransform;
    [SerializeField]Transform FPSCamera;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundCheckMask;
    [SerializeField] float groundCheckDistance;
    [SerializeField] float moveForce;
    [SerializeField] float gravityForce;

    //input variables
    Vector3 moveInput;//vectorul miscare
    Vector2 lookInput;//vectorul camerei
    float lookValY;

    float yMovement;
    bool groundedPlayer;
    float currentSpeed;

    float horizontalAxis;
    float verticalAxis;

    public bool crouching;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        col = GetComponent<CapsuleCollider>();
        playerTransform = transform;
        normalHeight = controller.height;
        heightDiff = col.height - controller.height;

        Cursor.lockState = CursorLockMode.Locked;

        groundCheckPos = groundCheck.localPosition;
    }

    private void Update()
    {
        processInput();
    }

    void processInput() {
        processCamera();
        processMovement();
    }

    void processCamera() {
        lookInput = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * sensivity;

        lookValY -= lookInput.y;

        //rotim playerul stanga dreapta
        transform.Rotate(new Vector3(0f, lookInput.x, 0f));
        //rotim camera sus jos

        lookValY = Mathf.Clamp(lookValY, -90f, 90f);

        FPSCamera.localEulerAngles = new Vector3(lookValY, 0f, 0f);

    }
    

    void processMovement() {
        //preia inputul si adauga miscarea la player
        horizontalAxis = 0;
        verticalAxis = 0;
        if (Input.GetKey(Keybindings.right)) {
            horizontalAxis += 1;
        }
        if (Input.GetKey(Keybindings.left))
        {
            horizontalAxis -= 1;
        }
        if (Input.GetKey(Keybindings.forward))
        {
            verticalAxis += 1;
        }
        if (Input.GetKey(Keybindings.backward))
        {
            verticalAxis -= 1;
        }

        moveInput = playerTransform.right * horizontalAxis + playerTransform.forward * verticalAxis;

        //limitam inputul de miscare in cazul mersului diagonal pentru a nu obtine o viteza
        //mult mai mare
        moveInput.Normalize();

        //proceseaza saritul
        groundedPlayer = performGroundCheck();

        //daca e la sol, yMovementul trebuie sa fie 0
        yMovement += gravityForce * Time.deltaTime;
        if (groundedPlayer)
        {
            if (Input.GetKeyDown(Keybindings.jump))
            {
                yMovement = jumpForce;
                if (crouching) {
                    setCrouch(false);
                }
            }
        }


        //proceseaza alergatul
        if (Input.GetKeyDown(Keybindings.crouch))
        {
            setCrouch(!crouching);
        }
        if (Input.GetKey(Keybindings.sprint))
        {
            currentSpeed = sprintSpeed;
            if (crouching)
            {
                setCrouch(false);
            }
        } else if (!crouching) {
            currentSpeed = speed;
        }

        moveInput *= currentSpeed * moveForce;
        moveInput.y = yMovement;

            //LA FINAL: Dupa toate calculele, se adauga movementul
        controller.Move(moveInput * Time.deltaTime);

    }

    bool performGroundCheck() {
        //daca loveste ceva, este grounded
        return Physics.Raycast(groundCheck.position, Vector3.down * groundCheckDistance, groundCheckDistance, groundCheckMask);
    }

    void setCrouch(bool state) {
        crouching = state;
        if (state)
        {
            controller.height = crouchHeight;
            col.height = crouchHeight + heightDiff;
            currentSpeed = crouchSpeed;
            groundCheck.localPosition = groundCheckCrouchPos;
        }
        else {
            controller.height = normalHeight;
            col.height = controller.height + heightDiff;
            currentSpeed = speed;
            groundCheck.localPosition = groundCheckPos;
        }
    }

    public void resetCamPos() {
        FPSCamera.localPosition = Vector3.zero;
    }
}
