using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToolManager : MonoBehaviour
{
    public Transform toolHolder;

    Tool currentTool;
    Rect guiRect = new Rect(Screen.width / 2, Screen.height / 2, 800f, 100f);
    Rect infoRect = new Rect(Screen.width - 200, Screen.height + 100, 100, 100);



    [HideInInspector]
    public bool showToolGuiText = false;
    bool isToolOwned = false;
    bool toolSlotTaken = false;

    float bitsIncrease;
    float electronicIncrease;

    PlayerScript playerScript;
    private List<Tool> tools;
    [HideInInspector]
    public bool toolnUseGUI = false;

    void Start()
    {
        playerScript = FindObjectOfType<PlayerScript>();
        tools = new List<Tool>();
    }


    public void CheckTool(Tool tool)
    {
        showToolGuiText = true;
        if (tools.Count != 0)
        {
            toolSlotTaken = true;
            if (tool.ID == currentTool.ID)
                isToolOwned = true;
            else
                isToolOwned = false;

        }
        else
        {
            toolSlotTaken = false;
        }


        CheckInput(tool);
    }
    void CheckInput(Tool tool)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (toolSlotTaken)
            {
                if (isToolOwned)
                {
                    Debug.Log("already own this tool");
                }
                else
                {
                    ReplaceTool(tool);
                }
            }
            else
            {
                AddTool(tool);
            }
        }
    }

    void ReplaceTool(Tool tool)
    {
        DropTool(currentTool);
        AddTool(tool);

    }


    void StoreStats(float bits,float electronics)
    {
        electronicIncrease = electronics;
        bitsIncrease = bits;
    }
    public void AddTool(Tool tool)
    {
        tools.Add(tool);
        currentTool = tool;
        currentTool.transform.position = toolHolder.position;
        currentTool.transform.rotation = toolHolder.rotation;
        currentTool.transform.parent = toolHolder;
        playerScript.SetCurrentTool(currentTool);
        currentTool.gameObject.SetActive(false);
      

    }
    void DropTool(Tool tool)
    {
        tools.Remove(tool);
        tool.gameObject.SetActive(true);
        tool.transform.parent = null;

    }
    void OnGUI()
    {
        if (showToolGuiText)
        {
            if (toolSlotTaken)
            {
                if (isToolOwned)
                {
                    GUI.Label(guiRect, "Tool already in use !");
                }
                else
                {
                    GUI.Label(guiRect, "Press <E> to replace !");
                }
            }
            else
            {
                GUI.Label(guiRect, "Press <E> to take !");
            }

        }
        if (tools.Count > 0 && currentTool != null)
        {
            switch (currentTool.Type)
            {
                case ToolTypes.Screwdriver:
                    GUI.Label(infoRect, "SCREW");
                    break;
                case ToolTypes.Hammer:
                    GUI.Label(infoRect, "HAMMER TIME !");
                    break;
                case ToolTypes.Wrench:
                    GUI.Label(infoRect, "WRENCH");
                    break;
            }
        }
    }


}
