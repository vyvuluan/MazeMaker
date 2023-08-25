using UnityEngine;

public class Win : Pivote
{
    [SerializeField] private ParticleSystem particleSystem1;
    [SerializeField] private GameObject openGO;
    [SerializeField] private GameObject closeGO;
    public override void Handle()
    {
        particleSystem1.Play();
        openGO.SetActive(true);
        closeGO.SetActive(false);
    }
}
