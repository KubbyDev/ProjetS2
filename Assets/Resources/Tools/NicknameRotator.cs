using UnityEngine;

/*
 * Ce script sert a tourner les pseudos au dessus des tetes des joueurs pour qu'ils soient toujours lisibles
 */

public class NicknameRotator : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
