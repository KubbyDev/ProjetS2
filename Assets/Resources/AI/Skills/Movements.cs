using UnityEngine;

public partial class Skills
{
    /// <summary>
    /// Se deplace vers la cible en predisant ou non ses deplacements
    /// </summary>
    /// <param name="target"></param>
    /// <param name="predictPath"></param>
    /// <param name="lookAtPredictedPosition"></param>
    public void MoveTo(GameObject target, bool predictPath = true, bool lookAtPredictedPosition = true)
    {
        if(!predictPath)
            MoveTo(target.transform.position);
        else
        {
            Vector3 velocity = Vector3.zero;
            Rigidbody rb = target.GetComponent<Rigidbody>();
            PlayerInfo info = target.GetComponent<PlayerInfo>();
            
            if (rb != null) velocity = rb.velocity;
            else if (info != null) velocity = info.velocity;
            else Debug.Log("Couldn't find the speed of " + target);

            //Le vecteur a ajouter pour faire la predition est v_target * d / v_balle (c'est pas parfait mais c'est suffisant)
            Vector3 predictedPosition = target.transform.position
                                        + velocity
                                        * Vector3.Distance(target.transform.position, transform.position)
                                        / move.movementSpeed;

            //Si la balle vient vers l'IA on ignore la prediction et on va vers elle
            if (Vector3.Dot((target.transform.position - transform.position).normalized, 
                    (predictedPosition - transform.position).normalized) < 0)
            {
                predictedPosition = Ball.ball.transform.position;
            }
            
            MoveTo(predictedPosition, lookAtPredictedPosition);
        }
    }
    
    /// <summary>
    /// Se dirige entre le but et le joueur le plus proche du but (en regardant le but adverse)
    /// </summary>
    public void MoveToSupportPosition()
    {
        MoveTo((GetNearestOpponentFromGoal().transform.position + GetAllyGoal().transform.position)/2, false);
    }
}
