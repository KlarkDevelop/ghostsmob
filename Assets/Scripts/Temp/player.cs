using UnityEngine;
using Unity.Netcode;
public class player : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    private CharacterController _characterControler;

    private void Start()
    {
        _characterControler = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        ChekGround();
        ApplayGravity();
        MouseMove();
        Move();
    }

    public float speed = 10f;
    private Vector3 movingV;
    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        movingV = (transform.right * x + transform.forward * y) * speed * Time.deltaTime;

        _characterControler.Move(movingV);
        _animator.SetFloat("Speed", movingV.magnitude);
    }

    [SerializeField] private Transform pl_Camera;
    public float sensitivity = 100f;
    private float xRotation;
    private void MouseMove()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        pl_Camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

    }

    [SerializeField] private float g = Physics.gravity.y;
    private Vector3 fallVel;
    private void ApplayGravity()
    {
        if (!isGrounded)
        {
            fallVel.y += g * Time.deltaTime * Time.deltaTime;
        }
        else
        {
            fallVel.y = -2f;
        }
        _characterControler.Move(fallVel);
    }

    [SerializeField] private Transform groundChaker;
    [SerializeField] private float chkerDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;
    private void ChekGround()
    {
        isGrounded = Physics.CheckSphere(groundChaker.position, chkerDistance, groundMask);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) enabled = false;
    }
}
