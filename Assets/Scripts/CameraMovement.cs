using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement")]
    public float PanSpeed;
    public float PanBorderThickness;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minZoom = 0.1f;
    public float maxZoom = 5.0f;

    [Header("Popup")]
    public GameObject Popup;
    public Text InfoText;
    public GameObject DestroyBuilding;
    private GameObject Selected;

    private float zoom;

    Vector2Int LastPosition;
    Vector2Int CurrentPossition;
    WorldComponent World;
    bool isSetup = false;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        Popup.SetActive(false);
        cam = Camera.main;
        LastPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        CurrentPossition = LastPosition;
    }

    public void SetWorld(WorldComponent world)
    {
        World = world;
        World.UpdateCameraPosition(LastPosition);
        isSetup = true;
    }
    void Update()
    {
        if (!isSetup)
        {
            return;
        }
        Movement();
        Zoom();


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                Popup.SetActive(true);
                Selected = hit.collider.gameObject;
                Popup.transform.position = new Vector3(Selected.transform.position.x , Selected.transform.position.y, 1);
                TileComponent tComp = Selected.GetComponent<TileComponent>();

                InfoText.text = tComp.GetText();
                if (tComp.TileData.Level > 0)
                {
                    DestroyBuilding.SetActive(true);
                }
                else
                {
                    DestroyBuilding.SetActive(false);
                }
                //Debug.Log("Target Position: " + go.transform.position);
            }
        }
    }

    public void DestroySelectedHouse()
    {
        World.HandleDestroyHouse((int) Selected.transform.position.x, (int)Selected.transform.position.y);
        World.UpdateCameraPosition(LastPosition);
        Popup.SetActive(false);
    }

    private void Zoom()
    {
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            zoom -= zoomSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            zoom += zoomSpeed * Time.deltaTime;
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            zoom -= zoomSpeed * Time.deltaTime * 10f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoom += zoomSpeed * Time.deltaTime * 10f;
        }

        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        GetComponent<Camera>().orthographicSize = zoom;
    }

    private void Movement()
    {
        Vector3 pos = this.transform.position;
        Vector3 cur = pos;
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - PanBorderThickness && Input.mousePosition.y < Screen.height)
        {
            pos.y += PanSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s") || Input.mousePosition.y <= PanBorderThickness && Input.mousePosition.y > 0)
        {
            pos.y -= PanSpeed * Time.deltaTime;
        }

        if (Input.GetKey("a") || Input.mousePosition.x <= PanBorderThickness && Input.mousePosition.x > 0)
        {
            pos.x -= PanSpeed * Time.deltaTime;
        }

        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - PanBorderThickness && Input.mousePosition.x < Screen.width)
        {
            pos.x += PanSpeed * Time.deltaTime;
        }

        if (cur == pos)
        {
            return;
        }

        CurrentPossition.x = (int)pos.x;
        CurrentPossition.y = (int)pos.y;

        MoveTo(pos);
    }

    private void MoveTo(Vector3 pos)
    {
        if (World.IsOutOfBounds(CurrentPossition.x, CurrentPossition.y))
        {
            return;
        }

        if (CurrentPossition != LastPosition)
        {
            LastPosition = CurrentPossition;
            World.UpdateCameraPosition(LastPosition);
            //return;
        }
        Popup.SetActive(false);
        this.transform.position = pos;
    }

}
