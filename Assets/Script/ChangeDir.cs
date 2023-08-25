using UnityEngine;

public class ChangeDir : Pivote
{
    [SerializeField] private GameObject brickUI;
    public override void Handle()
    {
        brickUI.SetActive(false);
    }
}
