using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float zoomSpeed;

    [SerializeField] Vector2 rangeZoom;
    [SerializeField] Vector2 rangeMoveX;
    [SerializeField] Vector2 rangeMoveZ;

    Vector3 newPos;

    private void Start()
    {
        newPos = transform.position;
    }

    private void Update()
    {
        Zoom();
        
        Move();

        transform.position = newPos;
    }

    private void Zoom()
    {
        if (Input.mouseScrollDelta.y > 0 && transform.position.y > rangeZoom.x)
        {
            newPos += transform.forward * zoomSpeed * Time.deltaTime;
        }
        if (Input.mouseScrollDelta.y < 0 && transform.position.y < rangeZoom.y)
        {
            newPos -= transform.forward * zoomSpeed * Time.deltaTime;
        }
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (x > 0 && transform.position.x < rangeMoveX.y)
        {
            newPos.x += moveSpeed * Time.deltaTime;
        }
        if (x < 0 && transform.position.x > rangeMoveX.x)
        {
            newPos.x -= moveSpeed * Time.deltaTime;
        }
        if (z > 0 && transform.position.z < rangeMoveX.y)
        {
            newPos.z += moveSpeed * Time.deltaTime;
        }
        if (z < 0 && transform.position.z > rangeMoveZ.x)
        {
            newPos.z -= moveSpeed * Time.deltaTime;
        }
    }
}
