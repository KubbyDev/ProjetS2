using UnityEngine;

public class Back : MonoBehaviour
{    
    public bool Player_Has_Back = false;

    [SerializeField] private GameObject theVoidPrefab;
    
    public void Player_Got_Back()
    {
        Player_Has_Back = true;
    }

    public void TP_Back()
    {
        if (Player_Has_Back)
        {
            Instantiate(theVoidPrefab, transform.position, Quaternion.identity);
                
            Spawns.AtRandomUnused(this.gameObject);
            Player_Has_Back = false;
        }
    }
}

