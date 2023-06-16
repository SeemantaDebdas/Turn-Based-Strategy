using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotateSpeed = 100f;
    [SerializeField] float zoomSpeed = 5f;

    const float MIN_FOLLOW_Y_OFFSET = 1f;
    const float MAX_FOLLOW_Y_OFFSET = 15f;

    public static CameraController Instance { get; private set; }

    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachineTransposer transposer;
    Vector3 followOffset;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        followOffset = transposer.m_FollowOffset;
    }

    private void Update()
    {
        MoveCamera();
        RotateCamera();
        ZoomCamera();
    }
    private void MoveCamera()
    {
        Vector3 input = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        moveSpeed = 10f;

        Vector3 moveVector = transform.forward * input.z + transform.right * input.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void RotateCamera()
    {
        float rotateSpeedMult = 0f;

        if (Input.GetKey(KeyCode.Q))
            rotateSpeedMult = rotateSpeed;
        else if (Input.GetKey(KeyCode.E))
            rotateSpeedMult = -rotateSpeed;

        transform.Rotate(transform.up * rotateSpeedMult * Time.deltaTime);
    }
    void ZoomCamera()
    {
        float zoomAmount = 1f;

        if (Input.mouseScrollDelta.y > 0f)
        {
            followOffset.y += zoomAmount;
        }
        else if(Input.mouseScrollDelta.y < 0f)
        {
            followOffset.y -= zoomAmount;
        }

        followOffset.y = Mathf.Clamp(followOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET); 
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, followOffset, zoomSpeed * Time.deltaTime);   
    }

    public float GetCameraHeight() => followOffset.y;
}
