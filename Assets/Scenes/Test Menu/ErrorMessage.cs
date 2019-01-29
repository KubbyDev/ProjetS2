using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessage : MonoBehaviour
{
    [SerializeField] private float displayTime = 1;      //Le temps durant lequel l'erreur reste affichee
    
    [SerializeField] private Text errorMessage;
    private float lastUpdate;

    void Start()
    {
    }

    void Update()
    {
        if(Time.time - lastUpdate > displayTime)
            this.gameObject.SetActive(false);
    }

    public void Display(string message)
    {
        errorMessage.text = message;
        lastUpdate = Time.time;
        this.gameObject.SetActive(true);
    }
}
