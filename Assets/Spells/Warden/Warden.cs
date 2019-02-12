using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Striker : MonoBehaviour
{
    private GameObject ball;
    private BallCanBeCaught canbecaught;


    [SerializeField] private float Freeze1_Duration = 1f;
    [SerializeField] private float Freeze2_Duration = 4f;
    [SerializeField] private float Freeze_Cooldown = 20f;

    private bool Freeze_Off_Cooldown;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Freeze_Spell()
    {
        StartCoroutine(Freeze_Coroutine());
    }

    IEnumerator Freeze_Coroutine()
    {
        if(Freeze_Off_Cooldown)
        {
            canbecaught.FreezeBall();
            yield return new WaitForSeconds(Freeze1_Duration);
            canbecaught.DeFreezeBall();
            // Freeze Part 2 Diable Gravity unless ball caught or time up
        }
    }
}
