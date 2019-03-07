using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour {
    public float animSpeed = 1.5f;           
    public bool isjumping = false;
    public string m_PlayerInput;         

    public float moveSpeed = 2.4f;
    public float rotateSpeed = 2.0f;
    public float jumpPower = 3.0f;

    public float m_MovementInputValueY;
    public float m_MovementInputValueX;
    public Vector3 nowpos;

    private Rigidbody rb;
    private Vector3 velocity;
    private string m_MovementInputAxisH;
    private string m_MovementInputAxisV;
    private Animator anim;                        



    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        m_MovementInputAxisH = "Horizontal" + m_PlayerInput;
        m_MovementInputAxisV = "Vertical" + m_PlayerInput;
    }

    void Update()
    {
        m_MovementInputValueY = Input.GetAxis(m_MovementInputAxisV);
        m_MovementInputValueX = Input.GetAxis(m_MovementInputAxisH);
        nowpos = transform.localPosition;

       // Debug.Log("input: " + m_MovementInputValueX + " " + m_MovementInputValueY);

    }
   
    void FixedUpdate()
    {
        Move(m_MovementInputValueX, m_MovementInputValueY);
        if (Input.GetKeyDown(KeyCode.Space) && m_PlayerInput == "Self")
        {
            if (!isjumping)
            {
                anim.SetBool("Jump", true);
                rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                isjumping = true;
            }
        }

        if(anim.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.Jump"))
        {
           
            if (!anim.IsInTransition(0)){
                float gravityControl = anim.GetFloat("GravityControl");
                if (gravityControl > 0)
                    rb.useGravity = false;
                anim.SetBool("Jump", false);
                
            }
        }
        else
        {
            isjumping = false;
        }
        
    }

    public void Move(float mivx, float mivy)
    {

        if (mivy > 0.1)
        {
            if (Input.GetButton("LeftShift" + m_PlayerInput))
            {
                anim.SetFloat("Speed", 1.0f);
                moveSpeed = 3.2f;
            }
            else
            {
                anim.SetFloat("Speed", 0.21f);
                moveSpeed = 2.4f;
            }
            
        }            
        else if (mivy < -0.1)
        {
            anim.SetFloat("Speed", -0.11f);
            moveSpeed = 2.0f;
        }
        else if(mivx > 0.1 || mivx < -0.1)
        {
            anim.SetFloat("Speed", 0.21f);
            moveSpeed = 2.0f;
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }
        anim.speed = animSpeed;
        rb.useGravity = true;


        velocity = new Vector3(mivx, 0, mivy);
        velocity = transform.TransformDirection(velocity);
        velocity *= moveSpeed;      
        transform.localPosition += velocity * Time.fixedDeltaTime;
   
    }

    public  void  MoveByTrans(Vector3 pos)
    {
        Vector3 lastpos = transform.localPosition;
        float mivx = pos.x - lastpos.x;
        float mivy = pos.z - lastpos.z;
        velocity = new Vector3(mivx, 0, mivy);
        velocity = transform.TransformDirection(velocity);
        velocity *= moveSpeed;
        anim.SetFloat("Speed", 0.21f);
        if(Mathf.Abs(mivx) > 0.1 && Mathf.Abs(mivy) > 0.1 ) anim.speed = animSpeed;
        transform.localPosition += velocity * Time.fixedDeltaTime;
    }

    public void jump()
    {
        anim.SetBool("Jump", true);
        rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        if (anim.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.Jump"))
        {

            if (!anim.IsInTransition(0))
            {
                float gravityControl = anim.GetFloat("GravityControl");
                if (gravityControl > 0)
                    rb.useGravity = false;
                anim.SetBool("Jump", false);

            }
        }
    }

    public void etcmove(Vector2 direction) 
    {
        Move(direction.x * 1.2f, direction.y * 1.2f);
       // Debug.Log("etc: " + direction.x + " " + direction.y);
    }
}
