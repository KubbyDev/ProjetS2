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
        targetRotation = Quaternion.LookRotation(point - transform.position);
    }
    
    // Movements -------------------------------------------------------------------------------------------------------

    public void Jump()
    {
        if(timeToMove <= 0)
            move.Jump();
    }
    
    /// <summary>
    /// Bouge jusqu'a position. UpdateRotation = true => regarde devant lui en marchant
    /// </summary>
    public void MoveTo(Vector3 position, bool updateRotation = true)
    {
        if(updateRotation)
            LookAt(position);

        targetPosition = position;
    }
}