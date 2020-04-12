using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalContainer : MonoBehaviour
{
    [SerializeField] public GameObject floor;
    
    // Start is called before the first frame update
    public static GlobalContainer Global;

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
}
