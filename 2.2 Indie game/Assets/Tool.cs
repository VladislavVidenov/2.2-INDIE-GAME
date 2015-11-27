using UnityEngine;
using System.Collections;

public enum ToolTypes
{
    Screwdriver, Hammer, Wrench
};
public class Tool : MonoBehaviour {

    //select the coorect tooltype in inspector.
    public ToolTypes Type;

    public float increaseScrap;
    public float increaseElectronics;
    [HideInInspector]
    public float ID;
    [HideInInspector]
    public bool draw = false;
    void Start()
    {
        ID = Type.GetHashCode();
    }
}
