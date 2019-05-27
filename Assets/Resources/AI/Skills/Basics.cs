using UnityEngine;

public partial class Skills
{
    // Orientation -----------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Tourne sur l'axe horinzontal (angle en degres)
    /// </summary>
    /// <param name="yaw"></param>
    public void SetYaw(float yaw)
    {
        SetOrientation(targetRotation.eulerAngles.x, yaw);
    }
    
    /// <summary>
    /// Tourne sur l'axe vertical (angle en degres)
    /// </summary>
    /// <param name="pitch"></param>
    public void SetPitch(float pitch)
    {
        SetOrientation(pitch, targetRotation.eulerAngles.y);
    }

    /// <summary>
    /// Tourne l'IA (pitch = axe vertical, yaw = axe horinzontal)
    /// </summary>
    /// <param name="pitch"></param>
    /// <param name="yaw"></param>
    public void SetOrientation(float pitch, float yaw)
    {
        targetRotation = Quaternion.Euler(new Vector3(pitch, yaw, 0));
    }
    
    /// <summary>
    /// Regarde le point specifie
    /// </summary>
    /// <param name="point"></param>
    public void LookAt(Vector3 point)
    {
        if(point - transform.position != Vector3.zero)
            targetRotation = Quaternion.LookRotation(point - transform.position);
    }
    
    // Tools -----------------------------------------------------------------------------------------------------------

    public float GetHorizontalDistance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(new Vector3(a.x, 0, a.z), new Vector3(b.x, 0, b.z));
    }
    
    public float GetVerticalDistance(Vector3 a, Vector3 b)
    {
        return b.y - a.y;
    }

    private Vector3 ProjectOnGround(Vector3 a)
    {
        return new Vector3(a.x, 0, a.z);
    }

    /// <summary>
    /// Revoie la hauteur max atteinte avec la vitesse et la position actuelle
    /// </summary>
    private float MaxHeight()
    {
        return transform.position.y + 0.5f * infos.velocity.y * infos.velocity.y / -Physics.gravity.y;
    } 
}