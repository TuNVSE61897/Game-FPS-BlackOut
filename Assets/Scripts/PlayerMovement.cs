using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script handles all aspect of teh players movement
//Currently: Running
//           Sprinting   
//           Jumping
//           Crouching   
//           Dodging   
//           Vaulting
//
//

public class PlayerMovement : MonoBehaviour
{
   public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    //public Vector3 curPos; 

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    public float Xrotation = 0;

    //vaulting Vars
    private bool isVaulting = false;
    private Vector3 vaultStart;
    private Vector3 vaultEnd;
    private float vaultSpeed = 5f;
    private float vaultLength;
    private float vaultLenthFraction;
    private float vaultStartTime;

    //Dodgeroll Vars
    private float rollDist = 2; //Expected Roll Distance
    private float tempRollDist = 2; //Actual Roll Distance
    private float minRollDist = 0.6f;
    private float rollSpeed = 10;
    private bool isRolling = false;
    private Vector3 rollStart;
    public Vector3 rollEnd;
    private float rollLengthFraction;
    private float rollStartTime;
    private KeyCode[] MovementKeys = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    private KeyCode lastkeyPressed;
    private float doubleTapTimer = 0.25f;
    private float doubleTapStartTime;


    [HideInInspector]
    public bool canMove = true; //If any action is taking place this should be false to prevent the player from moving durring animations or Lerps

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //curPos = new Vector3(transform.position.x, transform.position.y, transform.position.z); //Players current Position for Testing purposes
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        //Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }else
        {
            if (!isVaulting)
            {
                moveDirection.y = movementDirectionY;
            }

        }
        
        //Vaulting    
        if (Input.GetButton("Jump") && canMove)
        {
            VaultCheck();
            Debug.Log("Attempted to vault");
        }
        //Rolling, detects if any movement key is double tapped
        if (Input.anyKey) {
            foreach (KeyCode key in MovementKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    if (key == lastkeyPressed && ((Time.time - doubleTapStartTime) < doubleTapTimer ))
                    {
                        Debug.Log("DoubleTapped: " + key.ToString());
                        RollCheck(key.ToString());
                        lastkeyPressed = KeyCode.None;
                    }
                    else {
                        lastkeyPressed = key;
                        doubleTapStartTime = Time.time;
                    }
                    
                }
            }
        }

        


        //Crouching
        if (Input.GetKey(KeyCode.LeftControl))
        {
            GetComponent<CharacterController>().height = 1f;
            this.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        }
        else {
            GetComponent<CharacterController>().height = 2f;
            this.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            //Old method
            /* Yrotation += Input.GetAxis("Mouse X") * lookSpeed;
             Xrotation -= Input.GetAxis("Mouse Y") * lookSpeed;


             Xrotation = Mathf.Clamp(Xrotation, -90, 90);

             currentX = Mathf.SmoothDamp(currentX, Xrotation,ref XrotationV, lookSmoothDamp);
             currentY = Mathf.SmoothDamp(currentY, Yrotation, ref YrotationV, lookSmoothDamp);
             transform.rotation = Quaternion.Euler(currentX, currentY, 0);
            */
            Xrotation += -Input.GetAxis("Mouse Y") * lookSpeed;
            Xrotation = Mathf.Clamp(Xrotation, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(Xrotation, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            
        }
        if (isVaulting) //Moving the player along a lerp determined by VaultCheck()
        {
            float disCovered = (Time.time - vaultStartTime) * vaultSpeed;
            vaultLenthFraction = disCovered / vaultLength;

            Vector3 targetPos = Vector3.Lerp(vaultStart, vaultEnd, vaultLenthFraction);
            transform.position = targetPos;
            if (transform.position == vaultEnd)
            {
                isVaulting = false;
            }

        }


        else if (isRolling) { //Moving the player along a lerp determined by RollCheck() 
            float disCovered = (Time.time - rollStartTime) * rollSpeed;
            rollLengthFraction = disCovered / rollDist;
            Vector3 targetPos = Vector3.Lerp(rollStart, rollEnd, rollLengthFraction);
            transform.position = targetPos;

            if (transform.position == rollEnd) {
                playerCamera.transform.localPosition = new Vector3(0, 0, 0);
                isRolling = false;
            }
        }

        
    }
    private void VaultCheck() { //Checks to see if a vault is possible, ei. facing a wall and the wall is not to high to vault
        RaycastHit hit;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
        Vector3 direction = transform.forward;
        float dis = 1;
        Debug.DrawRay(origin, direction * dis);
        if (Physics.Raycast(origin, direction, out hit, dis)) { //detect if facing a wall
            Vector3 origin2 = origin;
            origin2.y += 0.5f;
            Debug.DrawRay(origin2, direction * dis);
            if (Physics.Raycast(origin2, direction, out hit, dis)) // detecting if can see over wall
            {
                //Cant see over wall, therefore wont vault
            }
            else {
                Vector3 origin3 = origin2 + (direction * dis); 
                Debug.DrawRay(origin3, -Vector3.up * origin2.y);
                if (Physics.Raycast(origin3, -Vector3.up, out hit, origin2.y)) { //finding new ground point to vault to
                    //canMove = false;
                    vaultStart = new Vector3(transform.position.x, transform.position.y, transform.position.z);//Init the start and end positions for the lerp
                    vaultEnd =  new Vector3(hit.point.x, hit.point.y + 1f, hit.point.z);
                    vaultLength = Vector3.Distance(vaultStart, vaultEnd);
                    vaultStartTime = Time.time;
                    isVaulting = true;
                    
                }
            }
        }
        
         
    }

    private void RollCheck(string RollDir) { //Check to make sure a roll is possible, ei. not to close to something to start a roll and check how far the roll can travel
        RaycastHit hit;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        Vector3 direction = Vector3.forward;
        switch (RollDir) {
            case ("W"): //Forward
                direction = transform.forward;
                break;
            case ("S"): //Back
                direction = -transform.forward;
                break;
            case ("A"): //Left
                direction = -transform.right;
                break;
            case ("D"): //Right
                direction = transform.right;
                break;
            default:
                Debug.Log("Invalid input in RollCheck SwitchStatment");
                break;
        }
        Debug.DrawRay(origin, direction * rollDist);
        if (Physics.Raycast(origin, direction, out hit, rollDist))
        {
            tempRollDist = hit.distance - 0.1f;
        }
        else {
            tempRollDist = rollDist;
        }
        if (tempRollDist <= minRollDist) {
            Debug.Log("Roll Failed. Too Short");
            return;
        }
        rollStart = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rollEnd = rollStart + (direction * tempRollDist);
        rollStartTime = Time.time;
        isRolling = true;
        playerCamera.transform.localPosition = new Vector3(0, -0.5f, 0);



    }
}
    

