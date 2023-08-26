using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float dragDistance = 1f;
    private const float distance = 1f;
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private Transform brickParent;
    [SerializeField] private GameObject twei1;
    [SerializeField] private GameObject jiao;
    [SerializeField] private LayerMask roadLayer;
    [SerializeField] private List<GameObject> list = new();
    private bool isCheckRay = false;
    private bool canInput = true;
    private Vector3 mouseDownPos, mouseUpPos;
    private RaycastHit hit;
    private Vector3 newPosition;
    private PivoteDirection dirInput;
    private void Update()
    {
        if (canInput)
        {
            InputSystem();
            canInput = false;
        }
        GetPivoteCurrent(dirInput);
    }
    public void InputSystem()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mouseUpPos = Input.mousePosition;
            if (Mathf.Abs(mouseUpPos.x - mouseDownPos.x) > dragDistance || Mathf.Abs(mouseUpPos.y - mouseDownPos.y) > dragDistance)
            {
                if (Mathf.Abs(mouseUpPos.x - mouseDownPos.x) > Mathf.Abs(mouseUpPos.y - mouseDownPos.y))
                {
                    if (mouseUpPos.x > mouseDownPos.x)
                    { //Right move  
                        Debug.Log("right");
                        dirInput = PivoteDirection.Right;
                        isCheckRay = false;
                    }
                    else
                    { //Left move  
                        Debug.Log("Left");
                        dirInput = PivoteDirection.Left;

                        isCheckRay = false;
                    }
                }
                else
                {
                    if (mouseUpPos.y > mouseDownPos.y)
                    {
                        //Up move  
                        Debug.Log("Up");
                        dirInput = PivoteDirection.Up;
                        isCheckRay = false;
                    }
                    else
                    {
                        //Down move  
                        Debug.Log("Down");
                        dirInput = PivoteDirection.Down;
                        isCheckRay = false;
                    }
                }
            }
            else
            {
                Debug.Log("Tapping");
            }
        }
    }
    public void AddBrick()
    {
        GameObject newBrick = Instantiate(brickPrefab, list.Count > 0 ? list[list.Count - 1].transform.position + new Vector3(0, 0.2f, 0) : brickParent.position, brickPrefab.transform.rotation);
        list.Add(newBrick);
        newBrick.transform.SetParent(brickParent);
        twei1.transform.position += new Vector3(0, 0.2f, 0);
        jiao.transform.position += new Vector3(0, 0.2f, 0);
    }
    private void GetPivoteCurrent(PivoteDirection pivoteDirection)
    {
        if (!isCheckRay)
        {
            Debug.Log("check ray");
            Ray ray = new(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, roadLayer))
            {
                Pivote pivote = hit.collider.gameObject.GetComponent<Pivote>();
                newPosition = transform.position + GetDir(pivoteDirection) * distance;
                if (pivote.Type == PivoteType.ChangeDirection)
                {
                    int index = (pivoteDirection == PivoteDirection.Up || pivoteDirection == PivoteDirection.Down) ? 1 : 0;
                    newPosition = transform.position + GetDir(pivote.PivoteDirectionList[index]) * distance;
                    dirInput = pivote.PivoteDirectionList[index];
                }
                Handle(pivote.Type);
                pivote.Handle();
            }
            isCheckRay = true;

        }
        else
        {
            //move
            if (hit.collider != null)
            {
                Pivote pivote = hit.collider.gameObject.GetComponent<Pivote>();
                //PivoteDirection pivoteDirectionTemp = pivote.CheckDirection(pivoteDirection);
                if (pivote.CheckDirection(pivoteDirection))
                {

                    var step = 5f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, newPosition, step);
                    if (Vector3.Distance(transform.position, newPosition) < 0.001f)
                    {
                        isCheckRay = false;
                    }
                }
                else
                {
                    // Debug.Log("stop");
                    canInput = true;
                }
            }
        }




    }
    public Vector3 GetDir(PivoteDirection pivoteDirection)
    {
        switch (pivoteDirection)
        {
            case PivoteDirection.Left:
                return Vector3.left;
            case PivoteDirection.Right:
                return Vector3.right;
            case PivoteDirection.Up:
                return Vector3.forward;
            case PivoteDirection.Down:
                return Vector3.back;
            default: return Vector3.zero;
        }
    }
    public void Handle(PivoteType type)
    {
        switch (type)
        {
            case PivoteType.Normal:
                break;
            case PivoteType.ChangeDirection:
            case PivoteType.Brick:
                AddBrick();
                break;
            case PivoteType.NeedBrick:
                NeedBrick();
                break;
            case PivoteType.Win:
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    RemoveBrick(list[i]);
                }
                break;
        }
    }
    private void RemoveBrick(GameObject go)
    {
        twei1.transform.position += new Vector3(0, -0.2f, 0);
        jiao.transform.position += new Vector3(0, -0.2f, 0);
        Destroy(go);
        list.RemoveAt(list.Count - 1);
    }
    public void NeedBrick()
    {
        if (list.Count > 0)
        {
            GameObject go = list[list.Count - 1];
            twei1.transform.position += new Vector3(0, -0.2f, 0);
            jiao.transform.position += new Vector3(0, -0.2f, 0);
            Destroy(go);
            list.RemoveAt(list.Count - 1);
        }
        else
        {
            Debug.Log("game over");
        }

    }

}
