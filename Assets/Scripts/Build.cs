using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static GlobalContainer;

public class Build : MonoBehaviour
{
    private List<List<int>> grid;
    private Vector2 selected;

    private int size = 5;
    // Start is called before the first frame update
    void Awake()
    {
        for (int y = 0; y < 1000 ;y++)
        {
            var row = new List<int>();
            for (int x = 0; x < 1000;x++)
            {
                row.Add(0);
                
            }
            grid.Add(row);
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (checkFloorClick(out hit))
        {
            setSelectedSquare((int)(hit.point.x / size), (int)(hit.point.z / size));
        }
        


    }

    void setSelectedSquare(int x,int z)
    {
        selected = new Vector2(x,z);
        Global.selector.transform.position=new Vector3(selected.x,selected.y);
        Global.selector.SetActive(true);
        
    }
}
