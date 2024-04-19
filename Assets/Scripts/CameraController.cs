using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float zoomSpeed;

    [SerializeField] Vector2 rangeZoom;
    [SerializeField] Vector2 rangeMoveX;
    [SerializeField] Vector2 rangeMoveZ;

    [SerializeField] private LayerMask layerMaskEvent;
    private Vector3 newPos;

    private float delta = 10f;

    private bool goBackNormal;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        newPos = transform.position;
        Camera.main.eventMask = layerMaskEvent;
        //Cursor.lockState = CursorLockMode.Confined;
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

        ////RIGHT
        //if (Input.mousePosition.x >= Screen.width - 3*delta)
        //{
        //    newPos.x += moveSpeed * Time.deltaTime;
        //    gameManager.ChangeCursor(gameManager.cursorRight);
        //}
        ////LEFT
        //if (Input.mousePosition.x <= delta)
        //{
        //    newPos.x -= moveSpeed * Time.deltaTime;
        //    gameManager.ChangeCursor(gameManager.cursorLeft);
        //}
        ////UP
        //if (Input.mousePosition.y >= Screen.height - delta)
        //{
        //    newPos.z += moveSpeed * Time.deltaTime;
        //    gameManager.ChangeCursor(gameManager.cursorUp);
        //}
        ////DOWN
        //if (Input.mousePosition.y <= 3*delta)
        //{
        //    newPos.z -= moveSpeed * Time.deltaTime;
        //    gameManager.ChangeCursor(gameManager.cursorDown);
        //}

        //if (Input.mousePosition.x >= Screen.width - 3*delta || Input.mousePosition.x <= delta || Input.mousePosition.y >= Screen.height - delta || Input.mousePosition.y <= 3*delta)
        //{
        //    goBackNormal = false;
        //}
        //else if (!goBackNormal)
        //{
        //    gameManager.ChangeCursor(gameManager.cursorNormal);
        //    goBackNormal = true;
        //}
    }
}
