using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

//Cette classe concentre toutes les infos importantes sur le joueur

public class PlayerInfo : MonoBehaviour
{
    public static GameObject localPlayer;  //Une reference au GameObject de ce client (initialise dans PlayerSetup)

    public const float baseCatchRange = 10f; //Valeur de base du catch range de la balle
    public const float baseCatchWidth = 6f;  //Valeur de base du catch width de la balle
    
    //Ces variables sont stockees ici, si vous les modifiez,
    //ca aura une influence sur le jeu
    public int goalsScored;                //Le nombre de buts que le joueur a marque
    public Team team = Team.None;          //La team du joueur
    public Hero hero = Hero.Stricker;      //La classe jouee par ce joueur
    public bool isPlayer;                  //False: C'est une IA
    public int ping;                       //Le ping de ce joueur
    public string nickname;                //Le pseudo du joueur
    public float maxCatchRange;            //La distance max a laquelle la balle peut etre attrapee
    public float catchWidth;               //L'imprecision autorisee pour attraper la balle
    public float BACooldown = 0f;          //Le cooldown de la basic attack
    public float firstCooldown = 0f;       //Le cooldown du A
    public float secondCooldown = 0f;      //Le cooldown du E
    
    //Ces variables sont simplement copiees ici
    //Les modifier n'aura aucun effet
    public Vector3 cameraPosition;         //La position de l'ancre de la camera
    public Quaternion cameraRotation;         //La rotation de la camera
    public Vector3 velocity;               //La vitesse du joueur
    public bool isGrounded;                //Si le joueur est au sol
    public bool hasBall;                   //Si le joueur a la balle
    public Vector3 lastMovementInput;      //Le dernier input ZQSD entre
    
    private float timeToPingUpdate;
    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        
        cameraPosition = transform.Find("CameraAnchor").position;
        cameraRotation = Camera.main.transform.rotation;
        maxCatchRange = baseCatchRange;
        catchWidth = baseCatchWidth;

        transform.Find("CirclesParticles").GetComponent<ParticleSystem>().Play();
        
        //Si c'est le joueur local, on utilise le hero par defaut
        if (isPlayer && pv != null && pv.IsMine)
        {
            SetHero(Settings.settings.defaultHero);
            UpdateInfos();
        }
    }

    void Update()
    {
        if (BACooldown > 0)
            BACooldown -= Time.deltaTime;
        
        if (firstCooldown > 0)
            firstCooldown -= Time.deltaTime;
        
        if (secondCooldown > 0)
            secondCooldown -= Time.deltaTime;

        //Si c'est le joueur local
        if (isPlayer && pv.IsMine)
        {
            //On update le ping toutes les 2 secondes
            if (timeToPingUpdate > 0)
                timeToPingUpdate -= Time.deltaTime;
            else
            {
                timeToPingUpdate = 2;
                pv.RPC("UpdatePing", RpcTarget.All, PhotonNetwork.GetPing());
            }
        }
    }

    [PunRPC]
    private void UpdatePing(int newPing)
    {
        ping = newPing;
    }

    public void SetTeam(Team t)
    {
        team = t;
        GetComponent<MeshRenderer>().material = t.GetMaterial();
        transform.Find("CirclesParticles").GetComponent<ParticleSystem>().startColor = t.GetMaterial().color;
        transform.Find("Light").GetComponent<Light>().color = t.GetMaterial().color;
    }
    
    public void SetHero(Hero h)
    {
        hero = h;
        firstCooldown = 0f;
        secondCooldown = 0f;
        GetComponent<MeshRenderer>().materials = hero.GetModel().materials;
        GetComponent<MeshFilter>().mesh = hero.GetModel().mesh;
        
        //Change les images pour les cooldowns des spells (si c'est le joueur local)
        if(isPlayer && pv != null && pv.IsMine)
            CooldownDisplay.cooldownDisplayer.UpdateSprites(hero.GetSpellsSprites());
    }

    //Synchronise les infos de ce joueur chez tous les clients
    public void UpdateInfos()
    {
        pv.RPC("UpdateInfo_RPC", RpcTarget.Others, (int) team, (int) hero);
    }

    //Synchronise les infos de ce joueur chez un client specifique
    public void UpdateInfos(Player player)
    {
        pv.RPC("UpdateInfo_RPC", player, (int) team, (int) hero);
    }
    
    [PunRPC]
    public void UpdateInfo_RPC(int team, int hero)
    {       
        SetHero((Hero) hero);
        SetTeam((Team) team);
    }

    public void ResetSpells()
    {
        BACooldown = 0;
        firstCooldown = 0;
        secondCooldown = 0;

        GetComponent<Back>().Player_Has_Back = false;
        GetComponent<Hook>().Player_Has_Hook = false;
        GetComponent<PowerShoot>().Player_Has_PowerShoot = false;
        
        GetComponent<Striker>().StopSpells();
        GetComponent<Warden>().StopSpells();
        GetComponent<Ninja>().StopSpells();
    }
}
