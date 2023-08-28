using Cinemachine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    private const float dragDistance = 1f;
    private const float distance = 1f;
    [SerializeField] private Transform brickParent;
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private GameObject twei1;
    [SerializeField] private GameObject jiao;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask roadLayer;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private List<GameObject> list = new();
    private bool isCheckRay = false;
    private bool canInput = true;
    private bool isAnimating = false;
    private bool isWin = false;
    private float startTime;
    private float startRotationY;
    private float startFov;
    private Vector3 mouseDownPos, mouseUpPos;
    private RaycastHit hit;
    private Vector3 newPosition;
    private PivoteDirection dirInput;
    private void Start()
    {
        startRotationY = virtualCamera.transform.eulerAngles.y;
        startFov = virtualCamera.m_Lens.FieldOfView;
    }
    private void Update()
    {
        if (canInput)
        {
            InputSystem();
            canInput = false;
        }
        GetPivoteCurrent(dirInput);
        PerformEffects();
    }
    private void PerformEffects()
    {
        if (isAnimating)
        {
            float elapsed = Time.time - startTime;
            float t = Mathf.Clamp01(elapsed / 1f);

            float newRotationY = Mathf.Lerp(startRotationY, 90, t);
            virtualCamera.transform.eulerAngles = new Vector3(virtualCamera.transform.eulerAngles.x, newRotationY, virtualCamera.transform.eulerAngles.z);

            float newFov = Mathf.Lerp(startFov, 30, t);
            virtualCamera.m_Lens.FieldOfView = newFov;

            if (t >= 1.0f)
            {
                isAnimating = false;
            }
        }
    }
    public void StartAnimation()
    {
        startTime = Time.time;
        isAnimating = true;
        isWin = true;
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
                        dirInput = PivoteDirection.Right;
                        isCheckRay = false;
                    }
                    else
                    { //Left move  
                        dirInput = PivoteDirection.Left;
                        isCheckRay = false;
                    }
                }
                else
                {
                    if (mouseUpPos.y > mouseDownPos.y)
                    {
                        //Up move  
                        dirInput = PivoteDirection.Up;
                        isCheckRay = false;
                    }
                    else
                    {
                        //Down move  
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
        Debug.Log("Add");
        GameObject newBrick = Instantiate(brickPrefab, list.Count > 0 ? list[list.Count - 1].transform.position + new Vector3(0, 0.2f, 0) : brickParent.position, brickPrefab.transform.rotation);
        list.Add(newBrick);
        newBrick.transform.SetParent(brickParent);
        twei1.transform.position += new Vector3(0, 0.2f, 0);
        jiao.transform.position += new Vector3(0, 0.2f, 0);
        animator.SetInteger("state", 1);
        if (list.Count > 18)
        {
            float targetFOV = virtualCamera.m_Lens.FieldOfView + 3f;
            DOTween.To(() => virtualCamera.m_Lens.FieldOfView, x => virtualCamera.m_Lens.FieldOfView = x, targetFOV, 0.3f)
           .OnComplete(() =>
           {
               // Zoom effect is complete
           });
        }
    }
    private void GetPivoteCurrent(PivoteDirection pivoteDirection)
    {
        if (!isCheckRay)
        {
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

                    var step = 10f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, newPosition, step);
                    if (Vector3.Distance(transform.position, newPosition) < 0.001f)
                    {
                        animator.SetInteger("state", 0);
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
                Debug.Log("luan");
                if (!isWin)
                    StartAnimation();
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
            if (list.Count > 18)
            {
                float targetFOV = virtualCamera.m_Lens.FieldOfView - 3f;
                DOTween.To(() => virtualCamera.m_Lens.FieldOfView, x => virtualCamera.m_Lens.FieldOfView = x, targetFOV, 0.3f)
               .OnComplete(() =>
               {
                   // Zoom effect is complete
               });
            }
        }
        else
        {
            Debug.Log("game over");
        }

    }

}
