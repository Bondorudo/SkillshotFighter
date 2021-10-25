using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType { Fireball, Boomerang, Arched, FollowMouse, SyndraBall }
public enum AfterDestination { Destroy, Linger }
public enum ReturnType { StraightLine, ToPlayer }
public enum OnHit { Damage, Heal }


public class SkillShot : MonoBehaviour
{
    [Header("Set up skillshot")]
    public GameObject player;
    public List<Abilities> abilities;
    public int i;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Vector2 playerPosInCreation;
    private LayerMask whatIsEnvironment;

    [Header("Generic Fields")]
    [HideInInspector] public PlayerType playerType;

    // Private Fields
    private bool boomerangReturn;
    private bool returnToPlayer;
    private float speed;
    private float dist;
    int reflect;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        whatIsEnvironment = LayerMask.GetMask("Environment");
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        abilities = player.GetComponent<PlayerController>().abilities;
        playerPosInCreation = player.transform.position;
        speed = abilities[i].speed;
        reflect = abilities[i].timesToReflect;

        switch (abilities[i].projectileType)
        {
            case ProjectileType.Fireball:
                break;
            case ProjectileType.Boomerang:
                break;
            case ProjectileType.Arched:
                break;
            case ProjectileType.FollowMouse:
                break;
            case ProjectileType.SyndraBall:
                StartCoroutine(Linger());
                break;
        }
    }

    protected virtual void Update()
    {
        if (abilities[i].projectileType != ProjectileType.SyndraBall)
        {
            if (dist <= abilities[i].range)
            {
                rb.velocity = moveDir * speed;
            }
            else
            {
                rb.velocity = Vector2.zero;
                if (abilities[i].projectileType == ProjectileType.Boomerang)
                {
                    Debug.Log("Turn");
                    StartCoroutine(BoomerangTurnAround());
                }
                else
                {
                    DoAfterDestination();
                }
            }
        }

        if (boomerangReturn)
        {
            rb.velocity = moveDir * speed;

            if (returnToPlayer)
            {
                moveDir = (player.transform.position - transform.position).normalized;
                speed *= 1.005f;
            }
        }

        Reflect();
    }

    public void MovingDirection(Vector2 dir)
    {
        moveDir = dir;
    }


    #region IEnumerators
    IEnumerator Linger()
    {
        yield return new WaitForSeconds(abilities[i].lingerTime);
        Destroy(gameObject);
    }

    IEnumerator BoomerangTurnAround()
    {
        Vector2 tempMoveDir = moveDir;

        yield return new WaitForSeconds(abilities[i].boomerang.timeTostayStill);

        boomerangReturn = true;

        switch (abilities[i].boomerang.returnType)
        {
            // Boomeran returns in a straight line 
            case ReturnType.StraightLine:
                Debug.Log("Return in line");
                moveDir = -tempMoveDir;

                break;
            // Boomeran tracks player and returns to them
            case ReturnType.ToPlayer:
                Debug.Log("Return to player");
                returnToPlayer = true;
                break;
        }
    }
    #endregion

    private void Reflect()
    {
        // TODO:change hit from a single raycast to a square raycast around projectile
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDir, 0.1f, whatIsEnvironment);
        if (hit && abilities[i].canBounce)
        {
            Vector2 reflectPos = transform.position;
            dist = Vector2.Distance(reflectPos, transform.position);
            if (reflect > 0)
            {
                Debug.Log("Hit! " + hit.transform.name);
                moveDir = Vector2.Reflect(moveDir, hit.normal);
                reflect--;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
            dist = Vector2.Distance(playerPosInCreation, transform.position);
    }

    private void DoAfterDestination()
    {
        switch (abilities[i].afterDestination)
        {
            case AfterDestination.Destroy:
                Destroy(gameObject);
                break;
            case AfterDestination.Linger:
                // TODO: Projectile stays where it lands, can be changed how long it stays there for.
                StartCoroutine(Linger());
                break;
               
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerController>().playerType != playerType)
            {
                if (abilities[i].onHit == OnHit.Damage)
                    collision.gameObject.GetComponent<PlayerController>().TakeDamage(abilities[i].healthModifier);
                else if (abilities[i].onHit == OnHit.Heal)
                    collision.gameObject.GetComponent<PlayerController>().GainHealth(abilities[i].healthModifier);
                Destroy(gameObject);
            }
            else if (returnToPlayer)
            {
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.tag == "Environment")
        {
            if (abilities[i].projectileType == ProjectileType.Boomerang)
            {
                if (boomerangReturn)
                {
                    Destroy(gameObject);
                }

                Debug.Log("Return " + boomerangReturn);
                rb.velocity = Vector2.zero;
                StartCoroutine(BoomerangTurnAround());
            }
        }
        else
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, moveDir);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(ray);
    }
}
