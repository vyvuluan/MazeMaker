using UnityEngine;

public class Brick : Pivote
{
    [SerializeField] private GameObject brickUI;
    public override void Handle()
    {
        brickUI.SetActive(false);
        type = PivoteType.Normal;
    }
}
