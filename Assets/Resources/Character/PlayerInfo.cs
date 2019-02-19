using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int goalsScored;                   //Le nombre de buts que le joueur a marque
    public Team team;                         //La team du joueur
    public Quaternion rotation;               //L'orientation de la camera
    public Transform cameraAnchor;            //L'ancre de la camera
    public Vector3 velocity;                  //La vitesse du joueur
    public bool isGrounded;                   //Si le joueur est au sol
    public bool hasBall;                      //Si le joueur a la balle
    public bool isPlayer;                     //False: C'est une IA

    public enum Team
    {
        Blue = 1,
        Orange = -1
    }
}
