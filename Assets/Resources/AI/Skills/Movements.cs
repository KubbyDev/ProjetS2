using UnityEngine;

public partial class Skills
{
    public void Jump()
    {
        if(blockInputs <= 0)
            move.Jump();
    }
    
    /// <summary> Bouge jusqu'a une cible fixe. </summary>
    /// <summary> UpdateRotation = true => regarde devant lui en marchant </summary>
    /// <summary> jumping = true => saute quand il est assez proche, si la cible est en hauteur </summary>
    /// <summary> stopDistance distance a laquelle l'IA considere qu'elle est arrive a destination </summary>
    public void MoveTo(Vector3 position, bool updateRotation = true, bool jumping = false, float stopDistance = 2f)
    {
        if(updateRotation)
            LookAt(position);
        
        targetPosition = position;
        
        if(Vector3.Distance(position, transform.position) < stopDistance)
            targetPosition = Vector3.zero; //Annule tout mouvement

        if (jumping &&
            GetVerticalDistance(transform.position, position) > 3 &&
            GetHorizontalDistance(transform.position, position) < 10 &&
            MaxHeight() < position.y)
        {
            Jump();   
        }
    }
    
    /// <summary> Se deplace vers la cible (potentiellement mobile). </summary>
    /// <summary> useJump permet de demander a l'IA de sauter quand elle arrive a proximite de la cible (si elle est en hauteur) </summary>
    /// <summary> predictPath permet de demander ou non de predire les deplacements de la cible </summary>
    /// <summary> lookAtPredictedPosition permet d'obliger l'IA a regarder devant elle quand elle marche </summary>
    /// <param name="target"></param>
    /// <param name="useJump"></param>
    /// <param name="predictPath"></param>
    /// <param name="lookAtPredictedPosition"></param>
    public void MoveTo(GameObject target, bool useJump = false, bool predictPath = true, bool lookAtPredictedPosition = true)
    {
        if (!predictPath)
        {
            MoveTo(transform.position, lookAtPredictedPosition, useJump);
        }
        else
        {
            //Recuperation de la vitesse de la cible
            Vector3 velocity = Vector3.zero;
            Rigidbody rb = target.GetComponent<Rigidbody>();
            PlayerInfo info = target.GetComponent<PlayerInfo>();
            if (rb != null) velocity = rb.velocity;
            else if (info != null) velocity = info.velocity;
            else Debug.Log("Couldn't find the speed of " + target);
            
            //Premiere prediction (position + v_target * d_ia_target / v_ia)
            Vector3 predictedPosition = PredictContactPoint(target, velocity, move.movementSpeed);

            //Si la cible vient vers l'IA on ignore la prediction
            if (Vector3.Dot(ProjectOnGround(target.transform.position - transform.position).normalized, ProjectOnGround(velocity).normalized) < -0.5)
            {
                //on bouge juste sur le cote pour la chopper quand elle passe
                Vector3 predictedMovement = Vector3.Cross(Vector3.up, ProjectOnGround(velocity));
                predictedPosition = transform.position + Vector3.Dot(predictedMovement, ProjectOnGround(velocity)) * 10f * predictedMovement;
            }
            
            if (useJump &&
                GetVerticalDistance(transform.position, target.transform.position) > 5 &&
                GetHorizontalDistance(transform.position, target.transform.position) 
                    < velocity.magnitude * 7.0f * GetVerticalDistance(transform.position, target.transform.position) &&
                MaxHeight() < target.transform.position.y)
            {
                targetPosition = Vector3.zero; //Annule tout mouvement
                Jump();    
            }
            
            MoveTo(predictedPosition, lookAtPredictedPosition);
        }
    }
    
    /// <summary>
    /// Se dirige vers position en evitant de s'approcher des adversaires
    /// </summary>
    /// <param name="position"></param>
    public void MoveToAvoidingEnnemies(Vector3 position)
    {
        GameObject nearestPlayer = GetNearestOpponent();
        
        if (nearestPlayer == null)
        {
            MoveTo(position);
            return;
        }
        
        Vector3 targetDirection = (position - transform.position).normalized;
        Vector3 toNearestPlayer = nearestPlayer.transform.position - transform.position;
        float distToNearestPlayer = toNearestPlayer.magnitude;
        
        //Si le joueur le plus proche est trop loin on l'ignore
        if(distToNearestPlayer > 50)
        {
            MoveTo(position);
            return;
        }

        //On devie le vecteur en fonction de la proximite du joueur le plus proche
        targetDirection += -toNearestPlayer.normalized * (50 - toNearestPlayer.magnitude)/50;
        
        MoveTo(transform.position + targetDirection*5);
    }
    
    /// <summary> Se dirige entre le but et le joueur le plus proche du but (en regardant le but adverse) </summary>
    public void MoveToSupportPosition() => MoveTo(SupportPosition(), false);

    /// <summary> Renvoie la position entre le but allie et le joueur ennemi le plus proche du but </summary>
    public Vector3 SupportPosition() => (GetNearestOpponentFromAllyGoal().transform.position + AllyGoal().transform.position) / 2;

    /// <summary> Se dirige sous les cages pour un arret eventuel </summary>
    public void MoveToDefensivePosition() => MoveTo(AllyGoal().transform.position, false, false, 10f);
    
    /// <summary> Avance aux 2/3 du terrain </summary>
    public void MoveToOffensivePosition() => MoveTo(OffensivePosition(), false);

    /// <summary> Renvoie la position aux 2/3 du terrain </summary>
    public Vector3 OffensivePosition() => (EnemyGoal().transform.position * 2 + AllyGoal().transform.position) / 3; 

    /// <summary> Se dirige dans les buts pour defendre </summary>
    public void MoveInGoal() => MoveTo(AllyGoal().transform.position, false, true, 5f);

    /// <summary> Se dirige vers les buts adverse en evitant de s'approcher des adversaires </summary>
    public void MoveToGoalAvoidingEnnemies() => MoveToAvoidingEnnemies(EnemyGoal().transform.position);
}
