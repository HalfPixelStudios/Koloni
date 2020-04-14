using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalContainer : MonoBehaviour
{
    [SerializeField] public GameObject floor;
    
    // Start is called before the first frame update
    public static GlobalContainer Global;
    public GameObject selector;

    private void Awake()
    {
        Global = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool InBounds(float val,int l,int r)
    {
        return val <= r && l <= val;


    }

    public static bool checkFloorClick(out RaycastHit hit)
    {
        if (Input.GetMouseButtonDown(0)) {
            

            return Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100) 
                   && hit.collider.name.Equals("Floor");

        }

        hit = new RaycastHit();
        return false;
    }
}
