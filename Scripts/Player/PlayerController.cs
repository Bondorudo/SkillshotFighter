using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityInputs { Q, E, R, F, V }
public enum PlayerType { Player_1, Player_2, Bot }

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public PlayerType playerType;

    [Header("Character Stats")]
    public int maxHP;
    public int currentHP;
    public float moveSpeed;
    public float jumpForce;
    public int maxJumpAmount;
    private int currentJumpAmount;
    public List<Abilities> abilities = new List<Abilities>();

    [Header("References")]
    public GameManager gm;
    public Transform rotationPivot;
    public Transform firePoint;
    private Rigidbody2D rb;

    [Header("Other Fields")]
    [HideInInspector]public Vector2 lookDir;
    Vector2 movement;

    public AbilityInputs abilityInput;

    public bool isAbilitySelected;
    public bool activateAbility;

    public float yFlip = 0.5f;
    public float rotationSpeed;


    [Header("Environment Checks")]
    public LayerMask whatIsGround;
    public Transform checkGround;
    public float groundCheckRadius;
    public bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
        currentJumpAmount = maxJumpAmount;
    }

    // Update is called once per frame
    void Update()
    {
        ShowAbilityIndicator();
        FlipPlayersToFaceEachOther();
        CheckAbilityInput();
        EnvironmentChecks();

        if (playerType != PlayerType.Bot)
        {
            Inputs();
            AbilitySelected();
        }

        if (currentHP <= 0)
            Die();
        

        if (isGrounded && rb.velocity.y <= 0.2f)
        {
            currentJumpAmount = maxJumpAmount;
        }
    }

    private void FixedUpdate()
    {
        if (playerType != PlayerType.Bot)
        {
            rb.position = new Vector2(rb.position.x + movement.x * moveSpeed * Time.deltaTime, rb.position.y);
        }

        lookDir = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y) - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        rotationPivot.rotation = Quaternion.Euler(0,0,angle);
    }

    private void CheckAbilityInput()
    {
        if (activateAbility)
        {
            switch (abilityInput)
            {
                case AbilityInputs.Q:
                    Ability(0);
                    break;
                case AbilityInputs.E:
                    Ability(1);
                    break;
                case AbilityInputs.R:
                    Ability(2);
                    break;
                case AbilityInputs.F:
                    Ability(3);
                    break;
                case AbilityInputs.V:
                    Ability(4);
                    break;
            }
            activateAbility = false;
            isAbilitySelected = false;
        }
    }

    public void Ability(int i)
    {
        GameObject abilityObject = Instantiate(abilities[i].abilityObject, firePoint.position, firePoint.rotation);
        SkillShot ss = abilityObject.GetComponent<SkillShot>();
        
        ss.i = i;
        ss.MovingDirection(lookDir.normalized);
        ss.playerType = playerType;
        ss.player = gameObject;

        if (abilities[i].projectileType == ProjectileType.SyndraBall)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ss.gameObject.transform.position = new Vector3(pos.x, pos.y, 0);

            float dist = Vector2.Distance(transform.position, ss.gameObject.transform.position);

            if (dist >= abilities[i].range)
            {
                // Get a line from skillshot to player and it's angle, normalize it and then multiply by range.
                Vector3 angle = ss.gameObject.transform.position - transform.position;
                ss.gameObject.transform.position = transform.position + (angle.normalized * abilities[i].range);
            }
        }
    }



    private void ShowAbilityIndicator()
    {
        if (isAbilitySelected)
            firePoint.gameObject.SetActive(true);
        else
            firePoint.gameObject.SetActive(false);
    }

    private void Inputs()
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.Space))
        {
            if (currentJumpAmount > 0)
                Jump();
        }

        if (Time.time > abilities[0].startTime)
        {
            if (Input.GetKeyDown("q"))
            {
                abilities[0].startTime = Time.time + abilities[0].cooldown;
                abilityInput = abilities[0].abilityInput;
                isAbilitySelected = true;
            }
        }
        
        if (Time.time > abilities[1].startTime)
        {
            if (Input.GetKeyDown("e"))
            {
                abilities[1].startTime = Time.time + abilities[1].cooldown;
                abilityInput = abilities[1].abilityInput;
                isAbilitySelected = true;
            }
        }


        if (Time.time > abilities[2].startTime)
        {
            if (Input.GetKeyDown("r"))
            {
                abilities[2].startTime = Time.time + abilities[2].cooldown;
                abilityInput = abilities[2].abilityInput;
                isAbilitySelected = true;
            }
        }

        if (Time.time > abilities[3].startTime)
        {
            if (Input.GetKeyDown("f"))
            {
                abilities[3].startTime = Time.time + abilities[3].cooldown;
                abilityInput = abilities[3].abilityInput;
                isAbilitySelected = true;
            }
        }

        if (Time.time > abilities[4].startTime)
        {
            if (Input.GetKeyDown("v"))
            {
                abilities[4].startTime = Time.time + abilities[4].cooldown;
                abilityInput = abilities[4].abilityInput;
                isAbilitySelected = true;
            }
        }
    }


    public void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;
        currentJumpAmount--;
    }

    private void AbilitySelected()
    {
        if (isAbilitySelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                activateAbility = true;
            }
        }
    }

    public void FlipPlayersToFaceEachOther()
    {
        if (gm.players.Count >= 2)
        {
            Vector2 vectorToTarget = gm.players[1].transform.position - gm.players[0].transform.position;
            float angle = Mathf.Atan2(yFlip, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector2.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotationSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
    }

    public void GainHealth(int health)
    {
        currentHP += health;
    }

    public void Die()
    {
        Debug.Log("DEAD");
        gm.EndGame(gameObject);
        Destroy(gameObject);
    }

    public void EnvironmentChecks()
    {
        isGrounded = Physics2D.OverlapCircle(checkGround.position, groundCheckRadius, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}


[System.Serializable]
public class Abilities
{
    public AbilityInputs abilityInput;

    [Header("Projectile Type Stats")]
    public ProjectileType projectileType;
    public Boomerang boomerang;
    public Arched arched;
    public FollowMouse followMouse;

    [Header("After Destination")]
    public AfterDestination afterDestination;
    public float lingerTime;
    public int timesToReflect;

    [Header("Base Stats")]
    public GameObject abilityObject;
    public float cooldown;
    public float speed;
    public float range;
    public int armorLevel; // How many times skillshot can be hit by another skillshot before breaking.
    public bool canBounce;
    [HideInInspector] public float startTime;

    [Header("What To Do On Hit")]
    public OnHit onHit;
    public int healthModifier;

}

[System.Serializable]
public class Boomerang
{
    public float timeTostayStill;
    public ReturnType returnType;
}

[System.Serializable]
public class Arched
{
    public float archAngle;
}

[System.Serializable]
public class FollowMouse
{
    // How fast the projectile turns to and goes to mouse position
    public float followSpeed;
}
