using UnityEngine;
using System.Collections;

public class RespawnEnemy : MonoBehaviour {

    void Start()
    {
        Instantiate(Resources.Load("Robot_Melee"), transform.position, Quaternion.identity);
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.H))
        {
            Instantiate(Resources.Load("Robot_Melee"), transform.position, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Instantiate(Resources.Load("Robot_RangedRush"), transform.position, Quaternion.identity);
        }

    }
}
