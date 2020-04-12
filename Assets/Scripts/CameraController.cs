using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalContainer;

public class CameraController : MonoBehaviour
{
    public float zoomFactor;
    public float rotateFactor;
    public float translateFactor;
    void Start()
    {
        
    }
    
    
    void Update()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        print(transform.eulerAngles);
        if (Input.GetMouseButton(1))
        {
            //camera can get stuck
            
       
            if ((inBounds(transform.eulerAngles.x, 270,360 )||inBounds(transform.eulerAngles.x, 0,90 ))
                &&(inBounds(transform.eulerAngles.y,270,360))||inBounds(transform.eulerAngles.y,0,90))
            {
                transform.Rotate(my*rotateFactor,mx*rotateFactor,0);
                
            }
            
            
        }else if (Input.GetMouseButton(0))
        {
            transform.Translate(mx*translateFactor,my*translateFactor,0);
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += zoomFactor * scroll * transform.forward;
        
        

    }
}
