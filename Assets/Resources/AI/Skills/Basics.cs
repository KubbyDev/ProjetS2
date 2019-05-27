using UnityEngine;

public partial class Skills
{
    // IDK -------------------------------------------------------------------------------------------------------------
    //Retroune la distanace par rapport au but allie
    public float AllyGoalDist()
    {
        return Vector3.Distance(AllyGoal().transform.position, infos.transform.position);
    }

    //Retourne la distance par rapport au but ennemi
    public float EnnemyGoalDist()
    {
        return Vector3.Distance(EnemyGoal().transform.position, infos.transform.position);
    }
    
    //Retourne la taille du terrain/ distance entre les 2 buts
    public float GoalsDist()
    {
        return Vector3.Distance(AllyGoal().transform.position, EnemyGoal().transform.position);
    }
    
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

    /// <summary>
    /// Imagine qu'on va tirer sur target un projectile ayant une vitesse objectVelocity
    /// Renvoie le point de contact entre le projectile et la cible en supposant que le projectile suit une trajectoire (presque) optimale (droite)
    /// </summary>
    /// <param name="target"></param>
    /// <param name="targetVelocity"></param>
    /// <param name="objectVelocity"></param>
    /// <param name="targetUsesGravity"></param>
    /// <returns></returns>
    private Vector3 PredictContactPoint(GameObject target, Vector3 targetVelocity, float objectVelocity, bool targetUsesGravity = true)
    {
        float travelTime = Vector3.Distance(target.transform.position, transform.position) / objectVelocity;

        //Le vecteur a ajouter pour faire la predition est v_target * d_target_objet / v_objet (c'est pas parfait mais c'est suffisant)
        Vector3 predicted = target.transform.position + targetVelocity * travelTime;

        //On ajoute ensuite une estimation rapide de la gravite
        if (targetUsesGravity)
            predicted += timeToMove * Physics.gravity;

        return predicted;
    }
}