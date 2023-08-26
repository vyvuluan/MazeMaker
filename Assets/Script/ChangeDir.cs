using UnityEngine;

public class ChangeDir : Pivote
{
    [SerializeField] private GameObject brickUI;
    [SerializeField] private Animator animator;
    public override void Handle()
    {
        brickUI.SetActive(false);
        animator.SetTrigger("Push");
    }
}
