using UnityEngine;
using System.Collections;

public enum ToolTypes
{
    Screwdriver, Hammer, Wrench
};
public class Tool : MonoBehaviour {

    //select the coorect tooltype in inspector.
    public ToolTypes Type;

    public int bitsBoost;

    [HideInInspector]
    public int ID;
    [HideInInspector]
    public bool draw = false;
    void Start()
    {
        ID = Type.GetHashCode();
    }
}
