using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    private Skills skills;            //Le script qui effectue les mouvement que ce script ordonne

    void Start()
    {
        skills = GetComponent<Skills>();
    }

    void Update()
    {

        // A gnee gneee taper ballon
        skills.currentState = Skills.State.GoToTheBall;

    }

}
