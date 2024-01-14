using UnityEngine;
using Unity.Netcode;
using System.Collections;
public class PlayerControler : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    private CharacterController _characterControler;
    public int inGameId;
    public Room currentRoom;
    private bool isLoaded = false;

    [SerializeField] private float aimDistance = 2;
    private int interactableObjectsMaskId;
    private void Awake()
    {
        PlayersManager.Singleton.AddPlayer(this);
    }
    private void Start()
    {
        _characterControler = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnDestroy()
    {
        PlayersManager.Singleton.RemovePlayer(this);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            transform.position = new Vector3(0, 1.05f, 0);
        }
        else
        {
            StartCoroutine(LoadPlayer());
        }
    }

    private IEnumerator LoadPlayer()
    {
        pl_Camera = Camera.main;
        pl_Camera.transform.parent = transform;
        pl_Camera.transform.localPosition = new Vector3(0, 0.74f, 0);
        transform.position = new Vector3(0, 1f, 0); //TODO: delete this line
        screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        interactableObjectsMaskId = LayerMask.NameToLayer("interactableObject");
        yield return new WaitForSeconds(1);

        isLoaded = true;
    }

    private void Update()
    {
        if (IsOwner && isLoaded)
        {
            ChekGround();
            MouseMove();
            Move();
            ApplayGravity();
            objectInFocus = GetObjectInFocus();
        }
    }

    private Vector3 screenCenter;
    private GameObject objectInFocus;
    private GameObject GetObjectInFocus()
    {
        RaycastHit hit;
        GameObject objInFocus = null;
        Ray aimRay = pl_Camera.ScreenPointToRay(screenCenter);
        // Debug.DrawRay(aimRay.origin, aimRay.direction, Color.red);

        if (Physics.Raycast(aimRay, out hit, aimDistance))
        {
            if (hit.collider.gameObject.layer == interactableObjectsMaskId) objInFocus = hit.collider.gameObject;
        }

        return objInFocus;
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

    [SerializeField] private Camera pl_Camera;
    public float sensitivity = 100f;
    private float xRotation;
    private void MouseMove()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        pl_Camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Room>(out Room foundedRoom))
        {
            if (currentRoom == null || currentRoom != foundedRoom)
            {
                currentRoom = foundedRoom;
                Debug.Log($"Player {inGameId} moved to room: {currentRoom}");
            }
        }
    }
}
