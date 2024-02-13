using UnityEngine;

public class DroneController : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public float turnSpeed = 100f; 
    public float ascentSpeed = 3f; 
    public float descentSpeed = 2f;

    public float moveLerp = 5f;

    public Vector3 currentTargetMovement = Vector3.zero;

    private Rigidbody rb;
    private float sensitivity = 100f;

    private float rotationY;
    private float rotationX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        Vector3 totalMove = Vector3.zero;

        totalMove += MoveDrone();
        totalMove += AdjustAltitude();

        currentTargetMovement = Vector3.Lerp(currentTargetMovement, totalMove, moveLerp * Time.deltaTime);
        rb.MovePosition(transform.position + currentTargetMovement);

        TurnDrone();
    }

    Vector3 MoveDrone()
    {
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            moveVertical = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveVertical = -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal = 1f;
        }

        return (transform.forward * moveVertical + transform.right * moveHorizontal).normalized * moveSpeed * Time.deltaTime;
       
    }

    void TurnDrone()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        rotationY += mouseX;
        rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        rb.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    Vector3 AdjustAltitude()
    {
        float altitudeChange = 0f;

        if (Input.GetKey(KeyCode.Space))
        {
            altitudeChange = ascentSpeed * Time.deltaTime; 
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            altitudeChange = -descentSpeed * Time.deltaTime; 
        }

        return Vector3.up * altitudeChange;
    }
}
