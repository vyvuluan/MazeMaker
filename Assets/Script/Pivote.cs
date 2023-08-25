using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PivoteDirection
{
    None, Up, Down, Left, Right
}
public enum PivoteType
{
    Normal, Brick, NeedBrick, ChangeDirection, Win
}
public class Pivote : MonoBehaviour
{
    [SerializeField] private List<PivoteDirection> pivoteDirectionList;
    [SerializeField] protected PivoteType type;

    public PivoteType Type { get => type; }

    public virtual void Handle()
    {
        Debug.Log("cha");
    }

    public bool CheckDirection(PivoteDirection direction)
    {
        PivoteDirection typeTemp = pivoteDirectionList.Where(x => x == direction).FirstOrDefault();
        if (typeTemp == PivoteDirection.None) return false;
        return true;
    }
}
