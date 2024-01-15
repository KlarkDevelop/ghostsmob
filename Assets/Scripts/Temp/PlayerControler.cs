using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
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
        // pl_Camera = Camera.main;
        // pl_Camera.transform.parent = transform;
        // pl_Camera.transform.localPosition = new Vector3(0, 0.74f, 0);
        transform.position = new Vector3(0, 1f, 0); //TODO: delete this lines
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
            if (Input.GetKeyDown(KeyCode.E) && objectInFocus != null) PickUpObject(objectInFocus);
            if (Input.GetKeyDown(KeyCode.Q) && inventory.Count != 0) SwitchItemsInInventory();
            if (Input.GetKeyDown(KeyCode.F)) DropItemInHand();
            if (Input.GetKeyDown(KeyCode.Mouse1) && itemInHand != null) TogglObject(itemInHand);
            if (Input.GetKeyDown(KeyCode.Mouse0) && objectInFocus != null) TogglObject(objectInFocus);
        }
    }

    private void TogglObject(GameObject obj)
    {
        if (obj.TryGetComponent<iToggleable>(out iToggleable item)) item.Toggl();
    }

    [SerializeField] private float dropForce;
    private void DropItemInHand()
    {
        if (itemInHand != null)
        {
            inventory.Remove(itemInHand);
            itemInHand.transform.parent = null;
            if (itemInHand.TryGetComponent<Rigidbody>(out Rigidbody rg))
            {
                rg.isKinematic = false;
                rg.AddForce(pl_Camera.transform.forward * dropForce, ForceMode.Impulse);
            }
            itemInHand = null;
        }
    }

    private int currentSlot = 0;
    private void SwitchItemsInInventory()
    {
        if (maxInventoryItems == 1) return;

        if (currentSlot < maxInventoryItems - 1) currentSlot++;
        else currentSlot = 0;

        if (itemInHand != null)
        {
            itemInHand.SetActive(false);
        }

        if (currentSlot < inventory.Count)
        {
            itemInHand = inventory[currentSlot];
            itemInHand.SetActive(true);
        }
        else
        {
            itemInHand = null;
        }
    }

    [SerializeField] private int maxInventoryItems;
    private List<GameObject> inventory = new List<GameObject>();
    private GameObject itemInHand;
    [SerializeField] private Transform itemsPositionPoint;
    private void PickUpObject(GameObject obj)
    {
        if (inventory.Count < maxInventoryItems)
        {
            if (obj.TryGetComponent<Rigidbody>(out Rigidbody rg))
            {
                rg.isKinematic = true;
            }
            obj.transform.parent = itemsPositionPoint;
            obj.transform.SetLocalPositionAndRotation(Vector3.zero, new Quaternion(0, 0, 0, 0));
            inventory.Add(obj);
            if (itemInHand == null)
            {
                itemInHand = obj;
            }
            else
            {
                obj.SetActive(false);
            }
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
