using UnityEngine;

public class NeedBrick : Pivote
{
    [SerializeField] private GameObject brickUI;
    public override void Handle()
    {
        brickUI.SetActive(true);
        type = PivoteType.Normal;
    }
}
