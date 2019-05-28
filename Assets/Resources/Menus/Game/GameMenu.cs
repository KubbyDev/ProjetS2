using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public static GameMenu script;
    
    private Text timeDisplayer;          //Le component qui affiche le temps restant
    private Text blueScoreDisplayer;     //Le component qui affiche le score de l'equipe bleu
    private Text orangeScoreDisplayer;   //Le component qui affiche le score de l'equipe orange
    private Text centralDisplay;         //Sert a afficher le message de but, de win et le temps pendant l'engagement
    private float centralScaleAnimTime;  //Sert a faire la petite animation d'arrivee du texte sur le display central
    private float centralFadeAnimTime;   //Sert a faire la petite animation de sortie du texte sur le display central
    
    void Awake()
    {
        script = this;
        centralDisplay = transform.Find("Central").GetComponent<Text>();   
        timeDisplayer = transform.Find("Background").Find("Time").GetComponent<Text>();
    }

    public void OnStartGame()
    {
        //Team du joueur local
        Team localPlayerTeam = PlayerInfo.localPlayer.GetComponent<PlayerInfo>().team;

        //Recuperation des brackgrounds
        Transform background = transform.Find("Background");
        Transform leftBackground = background.Find("LeftBackground");
        Transform rightBackground = background.Find("RightBackground");
        
        //Affichage du scoreboard
        background.gameObject.SetActive(true);

        //Couleurs des carres des scores
        leftBackground.GetComponent<Image>().color = localPlayerTeam.GetMaterial().color;
        rightBackground.GetComponent<Image>().color = localPlayerTeam.OtherTeam().GetMaterial().color;
        Tools.ModifyAlpha(0.55f, leftBackground.GetComponent<Image>());
        Tools.ModifyAlpha(0.55f, rightBackground.GetComponent<Image>());
        
        //Recuperation de tous les composants de texte qui seront modifies souvent
        blueScoreDisplayer = (localPlayerTeam == Team.Blue ? leftBackground : rightBackground).Find("Score").GetComponent<Text>();
        orangeScoreDisplayer = (localPlayerTeam == Team.Orange ? leftBackground : rightBackground).Find("Score").GetComponent<Text>();
    }

    public void UpdateTimeDisplay(float timeLeft)
    {
        timeDisplayer.text = Tools.FormatTime(timeLeft);
    }

    public void DisplayKickoffCountdown()
    {
        StartCoroutine(Kickoff_Coroutine());
    }

    IEnumerator Kickoff_Coroutine()
    {
        DisplayOnCentral("3");
        yield return new WaitForSeconds(1);
        DisplayOnCentral("2");
        yield return new WaitForSeconds(1);
        DisplayOnCentral("1");
        yield return new WaitForSeconds(1);
        DisplayOnCentral("Go");
    }

    public void OnScore(GameObject playerWhoScored, int blueScore, int orangeScore)
    {
        DisplayOnCentral((playerWhoScored == null ? "" : playerWhoScored.GetComponent<PlayerInfo>().nickname) + " scored !", 3, 60);
        blueScoreDisplayer.text = blueScore.ToString();
        orangeScoreDisplayer.text = orangeScore.ToString();
    }
    
    public void OnWin(Team winningTeam)
    {
        string VictoryOrDefeat()
        {
            if (winningTeam == Team.None)
                return "Draw";

            if (PlayerInfo.localPlayer.GetComponent<PlayerInfo>().team == winningTeam)
                return "Victory";
            else
                return "Defeat";
        }
            
        DisplayOnCentral(VictoryOrDefeat(), 3, 100);
    }

    //Affiche un texte au milieu (et gere les animations)
    private void DisplayOnCentral(string text, float time = 1, int fontSize = 120)
    {
        centralDisplay.fontSize = fontSize;
        centralDisplay.text = text;
        
        centralDisplay.transform.localScale = Vector3.zero;
        Tools.ModifyAlpha(1, centralDisplay);
        
        centralScaleAnimTime = 0;
        centralFadeAnimTime = time;
    } 
    
    void Update()
    {
        //Animation d'arrivee
        if (centralScaleAnimTime < 1f)
        {
            centralScaleAnimTime += Time.deltaTime;
            centralDisplay.transform.localScale = Clamp(1f/0.05f * centralScaleAnimTime) * Vector3.one;
        }                                             //Inverse de la duree de l'animation
        
        //Animation de sortie
        if (centralFadeAnimTime > -1f) 
        {
            centralFadeAnimTime -= Time.deltaTime;
            Tools.ModifyAlpha(Clamp(1f/0.3f * centralFadeAnimTime), centralDisplay);
        }                         //Inverse de la duree de l'animation
    }

    //Bloque value entre min et max
    private float Clamp(float value, float min = 0, float max = 1)
    {
        return Math.Max(min, Math.Min(value, max));
    }
}
