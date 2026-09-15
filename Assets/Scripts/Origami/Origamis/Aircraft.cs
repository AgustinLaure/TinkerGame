using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Aircraft : Origami
{
    [Header("References")]
    [SerializeField] private GameObject renderer;
    [SerializeField] private GameObject collider;
    [SerializeField] private AreaCollider areaCollider;
    private Camera mainCamera;
    private Rigidbody rb;
    private EventBus eventBus;
    private bool aimAnimEnded = false;
    private bool throwAnimEnded = false;

    [Header("Config")]
    [SerializeField] private float impulse;
    [SerializeField] private float accel;
    [SerializeField] private float accelDuration;

    private Vector3 direction;
    private bool shouldLaunch = false;

    private Coroutine throwCoroutine = null;

    private float accelDurationTimer = 0f;

    private Vector2 mousePosition = Vector2.zero;

    private readonly Quaternion facingLeftRotation = Quaternion.Euler(0f, 0, 0f);
    private readonly Quaternion facingRightRotation = Quaternion.Euler(0f, 180f, 0f);
    private Plane levelPlane = new Plane(Vector3.forward, Vector3.zero);

    private const ForceMode launchForceMode = ForceMode.Impulse;
    private const ForceMode accelForceMode = ForceMode.Acceleration;

    private int baseLayer = 0;
    private int noPlayerColLayer = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        areaCollider.OnColliderEntered += HandleCollision;

        crumpleColliderSize = 0.06f;
    }

    private void Start()
    {
        baseLayer = LayerMask.NameToLayer("Prop");
        noPlayerColLayer = LayerMask.NameToLayer("NoPlayerCol");
       
        mainCamera = Camera.main;

        eventBus = ServiceLocator.Instance.GetService<EventBus>();
        eventBus.Subscribe<OnPlayerAimAnimFinished>((Action)HandleAimAnimFinish);
        eventBus.Subscribe<OnPlayerThrowAnimFinished>((Action)HandleThrowAnimFinish);
    }

    private void Update()
    {
        if (throwCoroutine != null)
        {
            mousePosition = Input.mousePosition;

            HandleRotation();
        }

        if (!isCrumpled)
        {
            if (rb.linearVelocity.x > 0.01f || rb.linearVelocity.y > 0.01f)
            {
                Vector3 linear = rb.linearVelocity.normalized;
                Vector3 newUp = Vector3.Cross(linear, Vector3.forward);
                rb.rotation = Quaternion.LookRotation(linear, newUp);
            }
        }
    }

    private void FixedUpdate()
    {
        if (shouldLaunch)
        {
            rb.AddForce(direction * impulse, launchForceMode);
            shouldLaunch = false;
        }

        accelDurationTimer += Time.deltaTime;

        if (accelDurationTimer < accelDuration)
        {
            float thisFrameAccel = accel - (accelDurationTimer / accelDuration);

            rb.AddForce(direction * thisFrameAccel, accelForceMode);
        }
    }

    private void HandleCollision(Collision collision)
    {
        if (!isCrumpled)
        {
            Crumple();
        }
    }

    private void Launch()
    {
        shouldLaunch = true;
        transform.parent = null;
    }

    public override void Action()
    {
        if (throwCoroutine == null)
        {
            throwCoroutine = StartCoroutine(ThrowCoroutine());
        }
    }

    public override void Enable()
    {
        renderer.SetActive(true);
        EnableRigidBody(rb);
    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector2 screenPos = mousePosition;

        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (levelPlane.Raycast(ray, out float enterDistance))
        {
            return ray.GetPoint(enterDistance);
        }

        return Vector3.zero;
    }

    private void HandleRotation()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        Quaternion desiredRotation = Quaternion.identity;

        if (mouseWorldPosition.x > transform.position.x)
        {
            desiredRotation = facingLeftRotation;
        }
        else
        {
            desiredRotation = facingRightRotation;
        }

        eventBus.Raise<OnRotatePlayer>(desiredRotation);
    }

    public override void Disable()
    {
        renderer.SetActive(false);
        DisableRigidbody(rb);
    }

    public void HandleAimAnimFinish()
    {
        aimAnimEnded = true;
    }

    public void HandleThrowAnimFinish()
    {
        throwAnimEnded = true;
    }

    public IEnumerator ThrowCoroutine()
    {
        aimAnimEnded = false;
        eventBus.Raise<OnPlayerAim>();

        yield return new WaitUntil(() => aimAnimEnded);

        bool hasClicked = false;
        while (!hasClicked)
        {
            yield return null;
            hasClicked = Input.GetMouseButtonDown(0);
        }

        Vector3 mousePos = GetMouseWorldPosition();

        eventBus.Raise<OnPlayerThrow>();
        yield return new WaitUntil(() => throwAnimEnded);

        direction = Vector3.Normalize(mousePos - transform.position);

        collider.layer = noPlayerColLayer;

        Enable();
        Launch();

        eventBus.Raise<OnAircraftLaunched>();

        throwCoroutine = null;
    }

    private void OnDestroy()
    {
        areaCollider.OnColliderEntered -= HandleCollision;
    }
}


