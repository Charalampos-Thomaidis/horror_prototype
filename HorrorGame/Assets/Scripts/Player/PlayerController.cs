using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float walkSpeed = 5f;
    [SerializeField]
    private float walkFootstepDistance = 15f;
    [SerializeField]
    public float runSpeed = 9f;
    [SerializeField, Range(0f, 1f)]
    private float runStepLenghten = 1f;
    [SerializeField]
    private float runFootstepDistance = 30f;
    [SerializeField]
    public bool canCrouch = true;
    [SerializeField]
    private float crouchHeight = 1f;
    [SerializeField]
    private float crouchSpeed = 1f;
    [SerializeField]
    private float crouchSpeedMultiplier = 0.5f;
    [SerializeField]
    private float stickToGroundForce = 10f;
    [SerializeField]
    private float gravityMultiplier = 2f;
    [SerializeField]
    public MouseLook mouseLook;
    [SerializeField]
    private CurveControlledBob headBob = new CurveControlledBob();
    [SerializeField]
    private float stepInterval = 5f;
    [SerializeField]
    private AudioClip[] footstepSounds;

    public bool IsPlayerInCloset { get; private set; }

    private Camera _camera;
    private CharacterController characterController;
    private AudioSource audioSource;
    private CollisionFlags collisionFlags;
    private Vector2 input;
    private Vector3 moveDir = Vector3.zero;
    private Vector3 originalCameraPosition;
    private float stepCycle;
    private float nextStep;
    private bool isWalking;
    private bool HeadBob;
    private bool lerpCrouch = false;
    private bool isCrouching = false;
    private float originalHeight;
    private float crouchTimer = 0;
    private bool isLookingBehind = false;
    private float lookBehindSpeed = 0.25f;
    private Quaternion initialRotation;
    private Enemy enemy;
    private Inventory inventory;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        _camera = Camera.main;
        originalCameraPosition = _camera.transform.localPosition;
        originalHeight = characterController.height;
        headBob.Setup(_camera, stepInterval);
        stepCycle = 0f;
        nextStep = stepCycle / 2f;
        audioSource = GetComponent<AudioSource>();
        mouseLook.Init(transform, _camera.transform);
        initialRotation = _camera.transform.localRotation;
        enemy = GameManager.Instance.Enemies.FirstOrDefault();
        inventory = Inventory.Instance;
    }

    private void Update()
    {
        RotateView();

        if (canCrouch)
        {
            HandleCrouchInput();
        }

        else if (isCrouching)
        {
            isCrouching = false;
            lerpCrouch = true;
            crouchTimer = 0;
        }

        if (Input.GetButton("LookBehind"))
        {
            if (!isLookingBehind)
            {
                isLookingBehind = true;
                mouseLook.SetLookBehind(isLookingBehind);
                StopAllCoroutines();
                StartCoroutine(RotateCameraTo(Quaternion.Euler(0f, 180f, 0f)));
            }
        }
        else
        {
            if (isLookingBehind)
            {
                isLookingBehind = false;
                mouseLook.SetLookBehind(isLookingBehind);
                StopAllCoroutines();
                StartCoroutine(RotateCameraTo(initialRotation));
            }
        }

        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime * crouchSpeed;
            float newHeight = isCrouching ? crouchHeight : originalHeight;
            characterController.height = Mathf.Lerp(characterController.height, newHeight, crouchTimer);

            Vector3 cameraPosition = _camera.transform.localPosition;
            float targetCameraY = isCrouching ? 0.5f : originalCameraPosition.y;
            cameraPosition.y = Mathf.Lerp(cameraPosition.y, targetCameraY, crouchTimer);
            _camera.transform.localPosition = cameraPosition;

            if (Mathf.Abs(characterController.height - newHeight) < 0.01f && Mathf.Abs(cameraPosition.y - targetCameraY) < 0.01f)
            {
                characterController.height = newHeight;
                cameraPosition.y = targetCameraY;
                _camera.transform.localPosition = cameraPosition;
                lerpCrouch = false;
            }
        }
    }

    private void FixedUpdate()
    {
        float speed;
        GetInput(out speed);

        Vector3 desiredMove = transform.forward * input.y + transform.right * input.x;

        RaycastHit hit;
        Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hit,
            characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hit.normal).normalized;

        moveDir.x = desiredMove.x * speed;
        moveDir.z = desiredMove.z * speed;

        if (characterController.isGrounded)
        {
            moveDir.y = -stickToGroundForce;
        }
        else
        {
            moveDir += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
        }

        collisionFlags = characterController.Move(moveDir * Time.fixedDeltaTime);

        ProgressStepCycle(speed);
        UpdateCameraPosition(speed);

        mouseLook.UpdateCursorLock();
    }

    private void ProgressStepCycle(float speed)
    {
        if (characterController.velocity.sqrMagnitude > 0 && (input.x != 0 || input.y != 0))
        {
            stepCycle += (characterController.velocity.magnitude + (speed * (isWalking ? 1f : runStepLenghten))) * Time.fixedDeltaTime;
        }

        if (stepCycle > nextStep)
        {
            nextStep = stepCycle + stepInterval;
            PlayFootstepAudio();
        }
    }

    private void PlayFootstepAudio()
    {
        if (!characterController.isGrounded || isCrouching)
        {
            return;
        }

        int number = Random.Range(0, footstepSounds.Length);
        audioSource.clip = footstepSounds[number];
        audioSource.PlayOneShot(audioSource.clip);
        footstepSounds[number] = footstepSounds[0];
        footstepSounds[0] = audioSource.clip;

        foreach (var e in GameManager.Instance.Enemies)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
            {
                e.NotifyEnemy(transform.position, runFootstepDistance);
            }
            else
            {
                e.NotifyEnemy(transform.position, walkFootstepDistance);
            }
        }
    }

    private void UpdateCameraPosition(float speed)
    {
        Vector3 newCameraPosition;

        if (!HeadBob || isCrouching)
        {
            return;
        }

        if (characterController.velocity.magnitude > 0 && characterController.isGrounded)
        {
            _camera.transform.localPosition = headBob.DoHeadBob(characterController.velocity.magnitude + (speed * (isWalking ? 1f : runStepLenghten)));
            newCameraPosition = _camera.transform.localPosition;
        }
        else
        {
            newCameraPosition = _camera.transform.localPosition;
        }
        _camera.transform.localPosition = newCameraPosition;
    }

    private void GetInput(out float speed)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool wasWalking = isWalking;

        if (isCrouching && Input.GetKey(KeyCode.LeftShift) && vertical >= 0)
        {
            isCrouching = false;
            lerpCrouch = true;
        }

        if (isCrouching)
        {
            isWalking = true;
            speed = walkSpeed * crouchSpeedMultiplier;
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift) && vertical >= 0)
            {
                isWalking = false;
            }
            else
            {
                isWalking = true;
            }
            speed = isWalking ? walkSpeed : runSpeed;
        }

        if (isLookingBehind)
        {
            horizontal = 0;
            vertical = Mathf.Clamp(vertical, 0, 1);
        }

        input = new Vector2(horizontal, vertical);

        if (input.sqrMagnitude > 1)
        {
            input.Normalize();
        }

        HeadBob = !isCrouching && input.sqrMagnitude > 0 && characterController.height == originalHeight;
    }

    private void HandleCrouchInput()
    {
        if (!canCrouch) 
        {
            return;
        }

        LayerMask cantCrouchLayer = LayerMask.GetMask("CantUncrouch");

        if (!isWalking && Input.GetAxis("Vertical") > 0)
        {
            return;
        }

        if (Input.GetButtonDown("Crouch") || Input.GetKeyDown(KeyCode.LeftControl))
        {

            if (!characterController.isGrounded)
            {
                return;
            }

            if (isCrouching)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, characterController.height, cantCrouchLayer))
                {
                    return;
                }
            }
            isCrouching = !isCrouching;
            crouchTimer = 0;
            lerpCrouch = true;
        }
    }

    private void RotateView()
    {
        if (!isLookingBehind)
        {
            mouseLook.LookRotation(transform, _camera.transform);
        }
    }

    private IEnumerator RotateCameraTo(Quaternion targetRotation)
    {
        Quaternion startRotation = _camera.transform.localRotation;
        float elapseTime = 0f;

        while (elapseTime < lookBehindSpeed)
        {
            _camera.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, elapseTime / lookBehindSpeed);
            elapseTime += Time.deltaTime;
            yield return null;
        }
        _camera.transform.localRotation = targetRotation;

        inventory.gameObject.SetActive(!isLookingBehind);
    }

    public void EnterCloset()
    {
        IsPlayerInCloset = true;
        canCrouch = false;
        walkSpeed = 0f;
        runSpeed = 0f;
    }

    public void ExitCloset()
    {
        IsPlayerInCloset = false;
        canCrouch = true;
        walkSpeed = 4f;
        runSpeed = 8f;
    }
}