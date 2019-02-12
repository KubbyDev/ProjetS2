using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCanBeCaught : MonoBehaviour
{
    public bool canbecaught = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FreezeBall()
    {
        canbecaught = false;
    }

    public void DeFreezeBall()
    {
        canbecaught = true;
    }
}
