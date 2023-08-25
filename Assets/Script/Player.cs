using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private Transform brickParent;
    [SerializeField] private GameObject twei1;
    [SerializeField] private GameObject jiao;
    [SerializeField] private LayerMask roadLayer;
    [SerializeField] private List<GameObject> list = new();
    private float distance = 1f;
    private bool isCheckRay = false;
    private RaycastHit hit;
    private Vector3 newPosition;
    private PivoteDirection dirInput;
    private void Update()
    {
        GetPivoteCurrent(dirInput);
    }
    public void SetDirInput(int i)
    {
        switch (i)
        {
            case 0:
                dirInput = PivoteDirection.Up;
                break;
            case 1:
                dirInput = PivoteDirection.Down;
                break;
            case 2:
                dirInput = PivoteDirection.Left;
                break;
            case 3:
                dirInput = PivoteDirection.Right;
                break;
        }
        isCheckRay = false;
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
            isCheckRay = true;
            Ray ray = new(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, roadLayer))
            {
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow);
                newPosition = transform.position + GetDir(pivoteDirection) * distance;
                Pivote pivote = hit.collider.gameObject.GetComponent<Pivote>();
                if (pivote.Type == PivoteType.ChangeDirection)
                {
                    int index = (pivoteDirection == PivoteDirection.Up || pivoteDirection == PivoteDirection.Down) ? 1 : 0;
                    newPosition = transform.position + GetDir(pivote.PivoteDirectionList[index]) * distance;
                }
                Handle(pivote.Type);
                pivote.Handle();
            }
        }
        //move
        if (hit.collider != null)
        {
            Pivote pivote = hit.collider.gameObject.GetComponent<Pivote>();
            //PivoteDirection pivoteDirectionTemp = pivote.CheckDirection(pivoteDirection);
            if (pivote.CheckDirection(pivoteDirection))
            {

                var step = 5f * Time.deltaTime;
                Debug.Log(newPosition);

                transform.position = Vector3.MoveTowards(transform.position, newPosition, step);
                if (Vector3.Distance(transform.position, newPosition) < 0.001f)
                {
                    isCheckRay = false;
                }
                //else
                //{
                //}
            }
            else
            {
                // Debug.Log("stop");
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

                break;
        }
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
