using UnityEngine;

public class MouseController : MonoBehaviour
{

    [SerializeField] Texture2D cursor;
    [SerializeField] Texture2D cursorClicked;

    private Camera mainCamera;
    private CursorControls controls;
    private GameObject selectedGameObject = null;

    delegate void RightClickAction();
    RightClickAction CurrenRightClickAction;

    Vector3 lastMousePosition; //From Input.mousePosition
    float cameraSpeed = 10f;

    private void Awake()
    {
        controls = new CursorControls();
        ChangeCursor(cursor);
        Cursor.lockState = CursorLockMode.Confined;
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        controls.Mouse.leftClick.performed += _ => LeftClick();     //Lors de l'appui sur le clic gauche
        controls.Mouse.rightClick.started += _ => StartRightClick(); //Lors de l'appui sur le clic droit
        controls.Mouse.rightClick.performed += _ => EndRightClick(); //Lors du relachement du clic gauche

        CurrenRightClickAction = DoNothing;
    }

    // Update is called once per frame
    void Update()
    {
        CurrenRightClickAction();
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    void MoveCamera()
    {
        Vector3 movement = lastMousePosition - Input.mousePosition;
        if (movement != Vector3.zero)
        {
            movement.z = movement.y;
            if (mainCamera.transform.position.z+movement.z > 37.5f)
            {
                movement.z = 37.5f - mainCamera.transform.position.z;
            }
            else if (mainCamera.transform.position.z + movement.z < -7)
            {
                movement.z = -7 -  mainCamera.transform.position.z;
            }
            
            movement.y = 0;
            //Left button is being held down and the mouse move, that's the camera drag !
            mainCamera.transform.Translate(movement*cameraSpeed*Time.deltaTime, Space.World);
        }
        lastMousePosition = Input.mousePosition;
    }

    private void StartRightClick()
    {
        ChangeCursor(cursorClicked);
        if (selectedGameObject == null)
        {
            lastMousePosition = Input.mousePosition;
            CurrenRightClickAction = MoveCamera;
        }
    }

    private void EndRightClick()
    {
        ChangeCursor(cursor);
        if (selectedGameObject == null)
        {
            CurrenRightClickAction = DoNothing;
        }
        else
        {
            Ray ray = mainCamera.ScreenPointToRay(controls.Mouse.Position.ReadValue<Vector2>());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    selectedGameObject.GetComponent<IClick>().OnRightClickAction(hit.collider.gameObject);
                }
            }
        }
    }

    private void ChangeCursor(Texture2D cursorType)
    {
        Cursor.SetCursor(cursorType, Vector2.zero, CursorMode.Auto);
    }

    private void DoNothing()
    {

    }

    private void LeftClick()
    {
        ChangeCursor(cursor);
        DetectObject();
    }

    private void DetectObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(controls.Mouse.Position.ReadValue<Vector2>());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                if(selectedGameObject == null)
                {
                    selectedGameObject = hit.collider.gameObject;
                    selectedGameObject.GetComponent<IClick>().OnLeftClickAction();

                    HexMap_Continent hexMapContinent = GameObject.Find("HexMap").GetComponent<HexMap_Continent>();
                    //If selected object is a tile
                    if (selectedGameObject.GetComponent<HexComponent>() != null)
                    {
                        //TODO: Display interface for construction if tile empty
                        // details of the tile (name, costOfMovement ...)
                        // if there is a building, show what it does
                        // if it's the town center : display a button to add an unit in production and the list of production

                        
                        Hex hex = hexMapContinent.GetHexFromDictionnary(selectedGameObject);
                        if(hex.GetBuilding() != null)
                        {
                            //TODO:Show details of building
                            GameObject building = Instantiate(hexMapContinent.GetMineGO(), selectedGameObject.transform);
                            
                            Debug.Log("test");
                        }
                    }

                    //If selected object is an unit
                    else if (selectedGameObject.GetComponent<UnitView>() != null)
                    {
                        Hex hex = hexMapContinent.GetUnitFromDictionnary(selectedGameObject).getHex();
                        GameObject hexGO = hexMapContinent.GetHexeGameobjectFromDictionnary(hex);
                        if(hex.GetBuilding() == null)
                        {
                            //TODO : Open menu to create a building
                            //than use the function build of HexMap
                        }
                    }
                }
                else
                {
                    selectedGameObject.GetComponent<IClick>().OnLeftClickOnOtherAction();
                    selectedGameObject = null;
                }
            }
        }
    }

    public void UnselectAtEndTurn()
    {
        if(selectedGameObject != null)
        {
            selectedGameObject.GetComponent<IClick>().OnLeftClickOnOtherAction();
            selectedGameObject = null;
        }
    }
}
