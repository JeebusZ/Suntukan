using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPun
{
    [Header("PLAYER MOVEMENT")]
    public float speed = 1f;
    private float horizontal;
    private float vertical;
    private Rigidbody rg;

    [Header("PLAYER JUMP")]
    public float jumpHeight = 2f;
    public bool isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;
    private Collider[] groundHit;
    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;

    [Header("PLAYER SHADOW")]
    private RaycastHit _hit;
    private float shadowCastLength = 100f;
    public GameObject shadowOBJ;
    public Vector3 shadowOffset;

    [Header("PLAYER SPRITE")]
    [SerializeField] private SpriteRenderer sprite;
    public Animator playerAnim;
    private Vector3 playerScreenPoint;
    private Vector2 mouse;

    [Header("PLAYER ATTACK")]
    public bool canAttack;

    [Header("PLAYER STATS")]
    public Hero hero;
    private float health;
    private float stamina;
    private float damage;

    [Header("PHOTON")]
    private PhotonView view;

    [Header("Debug")]
    [SerializeField] private bool isRaw;

    private void Awake()
    {
        if(rg == null)
        {
            rg = GetComponent<Rigidbody>();
        }
    }
    private void Start()
    {
        canAttack = true;
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            SetupPlayer();
        }
    }
    private void Update()
    {
        if(view.IsMine)
        {
            if (playerAnim != null)
            {
                if (horizontal != 0 || vertical != 0)
                {
                    playerAnim.SetBool("IsMoving", true);
                }
                else if (horizontal == 0 || vertical == 0)
                    playerAnim.SetBool("IsMoving", false);
                Jump();
                view.RPC("RotateSprite", RpcTarget.AllBuffered);
                Attack();
                Shadow();
            }
        }
    }
    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            Vector3 currentPos = rg.position;
            if (!isRaw)
            {
                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");
            }
            else
            {
                horizontal = Input.GetAxisRaw("Horizontal");
                vertical = Input.GetAxisRaw("Vertical");
            }

            Vector3 InputVector = new Vector3(-horizontal, 0, -vertical);
            InputVector = Vector3.ClampMagnitude(InputVector, 1);
            Vector3 movement = InputVector * speed;
            Vector3 newPos = currentPos + movement * Time.fixedDeltaTime;
            rg.MovePosition(newPos);

        }
    }

    #region Jump
    private void Jump()
    {
        groundHit = Physics.OverlapSphere(feetPos.position, checkRadius, whatIsGround);
        if (groundHit.Length > 0)
        {
            isGrounded = true;
        }
        else
            isGrounded = false;
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            //rg.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rg.velocity = Vector2.up * jumpHeight;
        }
        if (Input.GetKey(KeyCode.Space) && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                rg.velocity = Vector2.up * jumpHeight;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }
    private void Shadow()
    {
        Debug.DrawLine(transform.position, Vector3.down * shadowCastLength, Color.red, 0.5f);
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, shadowCastLength))
        {
            if (_hit.collider.tag == "Ground")
            {
                shadowOBJ.transform.position = _hit.point + shadowOffset;
            }
        }
    }
    #endregion

    [PunRPC]
    private void RotateSprite()
    {
        //mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        mouse = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        playerScreenPoint = Camera.main.WorldToScreenPoint(this.transform.position);

        if(mouse.x < playerScreenPoint.x)
        {
            //Debug.Log("Left");
            sprite.flipX = false;
        }
        else
        {
            //Debug.Log("Right");
            sprite.flipX = true;
        }

        if(mouse.y < playerScreenPoint.y)
        {
            playerAnim.SetBool("IsFront", true);
        }
        else
        {
            playerAnim.SetBool("IsFront", false);
        }
    }

    private void Attack()
    {
        if (canAttack)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Do attack here
                playerAnim.SetTrigger("Attack");
                canAttack = false;
                speed = 1.5f;
            }
        }
    }

    public void SetupPlayer()
    {
        if(hero != null)
        {
            health = hero.Health;
            stamina = hero.Stamina;
            damage = hero.Damage;
            Debug.LogWarning("Player Setup!");
        }
    }

    public void ResetPlayerViaAnimation()
    {
        canAttack = true;
        speed = 1;
        Debug.Log("Reset");
    }
}
