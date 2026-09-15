using UnityEngine;

public class Aircraft : Origami
{
    [Header("References")]
    [SerializeField] private GameObject renderer;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Action()
    {
        Debug.Log("hola");
    }

    public override void Enable()
    {
        renderer.SetActive(true);
        EnableRigidBody(rb);
    }

    public override void Disable()
    {
        renderer.SetActive(false);
        DisableRigidbody(rb);
    }
}

    
