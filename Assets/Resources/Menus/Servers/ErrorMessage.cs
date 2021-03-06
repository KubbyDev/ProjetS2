﻿using UnityEngine;
using UnityEngine.UI;

public class ErrorMessage : MonoBehaviour
{
    [SerializeField] [Range(0.5f,5)] private float displayTime = 1;      //Le temps durant lequel l'erreur reste affichee
    [SerializeField] [Range(0, 1)]   private float fadeTime = 0.8f;      //La proportion du temps total prise par le fondu

    private Text errorMessage;           //Le component qui affiche le texte a l'ecran
    private float lastUpdate;            //Enregistre le moment ou le message d'erreur a ete affiche

    void Awake()
    {
        errorMessage = GetComponentInChildren<Text>(true);
    }
    
    void Update()
    {
        //L'avancement de l'animation (de 0 a 1)
        float animTime = (Time.time - lastUpdate) / displayTime;

        //Fondu de fin: On modifie le channel alpha (transparence) en fonction du temps
        if(animTime > 1-fadeTime)
            ModifyAlpha(1 - (animTime - 1 + fadeTime) / fadeTime);

        //Disparition du message
        if (Time.time - lastUpdate > displayTime)
            this.gameObject.SetActive(false);
    }

    //Affiche une erreur
    public void Display(string message)
    { 
        this.gameObject.SetActive(true);

        //Reset de la transparence
        ModifyAlpha(1);

        //Mise a jour du texte et du temps de depart
        errorMessage.text = message;
        lastUpdate = Time.time;
    }

    //Modifie la transparence de l'image et du texte du message d'erreur
    private void ModifyAlpha(float newAlpha)
    {
        Tools.ModifyAlpha(newAlpha, errorMessage);
        Tools.ModifyAlpha(newAlpha, GetComponent<Image>());
    }
}
