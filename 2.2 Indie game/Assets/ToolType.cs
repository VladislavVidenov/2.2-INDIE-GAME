using UnityEngine;
using System.Collections;

public enum ToolTypes
{
    Screwdriver, Hammer, Wrench
};
public class ToolType : MonoBehaviour {

    //select the coorect tooltype in inspector.
    public ToolTypes toolType;

    public float increaseScrap;
    public float increaseElectronics;


}
