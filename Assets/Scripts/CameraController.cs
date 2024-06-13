using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float zoomSpeed;

    [SerializeField] Vector2 rangeZoom;

    [SerializeField] private LayerMask layerMaskEvent;
    private Vector3 newPos;

    private GameManager gameManager;

    private Camera myCam;
    [SerializeField] private Camera trailCamera;

    [SerializeField] Vector2 rangeX;
    [SerializeField] Vector2 rangeZ;
    private float factorHorizontal = 1.8f;
    private float factorVertical = 1.4f;


    private void Start()
    {
        gameManager = GameManager.Instance;
        newPos = transform.position;
        Camera.main.eventMask = layerMaskEvent;
        //Cursor.lockState = CursorLockMode.Confined;

        myCam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (gameManager.isPause) return;

        Zoom();
        
        Move();
    }

    private void Zoom()
    {
        if (Input.mouseScrollDelta.y > 0 && myCam.orthographicSize > rangeZoom.x)
        {
            myCam.orthographicSize -= zoomSpeed * Time.unscaledDeltaTime; 
        }
        if (Input.mouseScrollDelta.y < 0 && myCam.orthographicSize < rangeZoom.y
            && transform.position.x < rangeX.y - factorHorizontal * myCam.orthographicSize
            && transform.position.x > rangeX.x + factorHorizontal * myCam.orthographicSize
            && transform.position.z < rangeZ.y - factorVertical * myCam.orthographicSize
            && transform.position.z > rangeZ.x + factorVertical * myCam.orthographicSize)
        {
            myCam.orthographicSize += zoomSpeed * Time.unscaledDeltaTime;
        }
        
        if (Input.mouseScrollDelta.y != 0) trailCamera.orthographicSize = myCam.orthographicSize;
    }

    public void CenterTo(Vector3 pos)
    {
        myCam.orthographicSize = 7;
        newPos = new Vector3(pos.x , transform.position.y , pos.z - 45);
        transform.position = newPos;

    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (x > 0 && transform.position.x < rangeX.y - factorHorizontal * myCam.orthographicSize)
        {
            newPos += transform.right * moveSpeed * Time.unscaledDeltaTime;
        }
        if (x < 0 && transform.position.x > rangeX.x + factorHorizontal * myCam.orthographicSize)
        {
            newPos -= transform.right * moveSpeed * Time.unscaledDeltaTime;
        }
        if (z > 0 && transform.position.z < rangeZ.y - factorVertical * myCam.orthographicSize)
        {
            newPos += transform.up * moveSpeed * Time.unscaledDeltaTime + transform.forward * moveSpeed * Time.unscaledDeltaTime;
        }
        if (z < 0 && transform.position.z > rangeZ.x + factorVertical * myCam.orthographicSize)
        {
            newPos -= transform.up * moveSpeed * Time.unscaledDeltaTime + transform.forward * moveSpeed * Time.unscaledDeltaTime;
        }
        if (x != 0 || z != 0) transform.position = newPos;

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
