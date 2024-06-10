using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float zoomSpeed;

    [SerializeField] Vector2 rangeZoom;

    [SerializeField] private LayerMask layerMaskEvent;
    private Vector3 newPos;

    private float delta = 10f;

    private bool goBackNormal;

    private GameManager gameManager;

    private Camera myCam;
    [SerializeField] private Camera trailCamera;

    [SerializeField] private GameObject rightLimits;
    [SerializeField] private GameObject topLimits;
    [SerializeField] private GameObject bottomLimits;
    [SerializeField] private GameObject leftLimits;
    

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

        transform.position = newPos;
    }

    private bool isLimit(GameObject listLimits)
    {
        bool result = false;
        for (int i = 0; i < listLimits.transform.childCount; i++)
        {
            LevelLimit limit = listLimits.transform.GetChild(i).gameObject.GetComponent<LevelLimit>();
            if (limit.isView)
            {
                result = true;
                break;
            }
        }
        return result;
    }

    private void Zoom()
    {
        if (Input.mouseScrollDelta.y > 0 && myCam.orthographicSize > rangeZoom.x)
        {
            myCam.orthographicSize -= zoomSpeed * Time.unscaledDeltaTime; 
        }
        if (Input.mouseScrollDelta.y < 0 && myCam.orthographicSize < rangeZoom.y && !isLimit(rightLimits) && !isLimit(leftLimits) && !isLimit(topLimits) && !isLimit(bottomLimits))
        {
            myCam.orthographicSize += zoomSpeed * Time.unscaledDeltaTime;
        }
        
        trailCamera.orthographicSize = myCam.orthographicSize;
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (x > 0 && !isLimit(rightLimits))
        {
            newPos += transform.right * moveSpeed * Time.unscaledDeltaTime;
        }
        if (x < 0 && !isLimit(leftLimits))
        {
            newPos -= transform.right * moveSpeed * Time.unscaledDeltaTime;
        }
        if (z > 0 && !isLimit(topLimits))
        {
            newPos += transform.up * moveSpeed * Time.unscaledDeltaTime + transform.forward * moveSpeed * Time.unscaledDeltaTime;
        }
        if (z < 0 && !isLimit(bottomLimits))
        {
            newPos -= transform.up * moveSpeed * Time.unscaledDeltaTime + transform.forward * moveSpeed * Time.unscaledDeltaTime;
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
