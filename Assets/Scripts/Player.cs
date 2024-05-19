using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Numerics;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    
    public float speed;
    public float gravity;
    public bool isDebug;
    public LayerMask whatIsGround;
    public GameObject spawnPoint;
    public float waitOnDeath;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    private Animator animator;

    private BoxCollider2D mCollider;

    [HideInInspector]
    public bool facingRight=true;
    [HideInInspector]
    public bool facingUp=true;
    bool isDying=false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("Moving", false);
    }

    // Update is called once per frame
    void Update()
    {
        
        
        RespondToMovementInput();
        RespondToGravityInput();
        checkAnimation();
        if ((facingRight == false && moveInput > 0) || (facingRight == true && moveInput < 0))
        {
            FlipHorizontally();
        }
    }
    void fixedUpdate(){

         if (!isDying)
        {
            isGrounded = CheckIsGrounded();
            moveInput = Input.GetAxis("Horizontal");

            rb.velocity = new Vector2(moveInput * speed, isGrounded ? 0.0f : gravity);

            // Activamos la animación si es necesario.
            

            // Activamos la animación de caminar.
            checkAnimation();

            // Volteamos horizontalmente.
            if ((facingRight == false && moveInput > 0) || (facingRight == true && moveInput < 0))
            {
                FlipHorizontally();
            }
        }
    }

    void RespondToMovementInput(){

        if(!isDying){
             moveInput=Input.GetAxis("Horizontal");

            isGrounded=CheckIsGrounded();

        
             rb.velocity= new Vector2(moveInput *speed,isGrounded ? 0.0f : gravity);
        }
       
    }

    bool CheckIsGrounded(){
       RaycastHit2D raycastHit= Physics2D.BoxCast(mCollider.bounds.center, mCollider.bounds.size, 0f, facingUp ? Vector2.down: Vector2.up, 0.1f, whatIsGround);

       PaintDebugBox(raycastHit);

       return raycastHit.collider != null;
        
    }

     void FlipHorizontally()
    {
        facingRight = !facingRight;
        Vector2 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    void FlipVertically()
    {
        facingUp = !facingUp;
        Vector2 Scaler = transform.localScale;
        Scaler.y *= -1;
        transform.localScale = Scaler;
    }

    void PaintDebugBox(RaycastHit2D raycastHit)
    {
        if (isDebug)
        {
            Color rayColor;

            if (raycastHit.collider != null)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            Vector2 abajo = facingUp ? Vector2.down : Vector2.up;

            Debug.DrawRay(mCollider.bounds.center + new Vector3(mCollider.bounds.extents.x, 0), abajo * (mCollider.bounds.extents.y + 0.1f), rayColor);
            Debug.DrawRay(mCollider.bounds.center - new Vector3(mCollider.bounds.extents.x, 0), abajo * (mCollider.bounds.extents.y + 0.1f), rayColor);

            if (facingUp)
            {
                Debug.DrawRay(mCollider.bounds.center - new Vector3(mCollider.bounds.extents.x, mCollider.bounds.extents.y + 0.1f), Vector3.right * (mCollider.bounds.size.y), rayColor);
            }
            else
            {
                Debug.DrawRay(mCollider.bounds.center + new Vector3(mCollider.bounds.extents.x, mCollider.bounds.extents.y + 0.1f), -Vector3.right * (mCollider.bounds.size.y), rayColor);
            }

            Debug.Log(raycastHit.collider);
        }
    }
    void RespondToGravityInput(){
        if(Input.GetKeyDown(KeyCode.W) && isGrounded==true){
            gravity = -gravity;
            FlipVertically();
        }
    }
    void checkAnimation(){
        if( moveInput!=0 && isGrounded==true){
            animator.SetBool("Moving", true);
        }else{
            animator.SetBool("Moving", false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Danger")){
            StartCoroutine(ManageDeath());
        }
    }

    IEnumerator ManageDeath(){
        isDying = true;
        animator.SetBool("isDying", true);
        


        // Desactivamos el rigidbody, para que no le afecten las físicas.
        rb.Sleep();

        // Esperamos unos segundos.
        yield return new WaitForSeconds(waitOnDeath);
        
        animator.SetBool("isDying", false);

        // Si estabamos boca abajo antes de morir
        if (gravity > 0.0f) 
        {
            // Reseteamos la gravedad y la dirección del personaje
            FlipVertically();
            gravity *= -1;
        }

        // Movemos al jugador a la posición del checkpoint activo
        transform.position = spawnPoint.transform.position;
        transform.localEulerAngles = new Vector3(0,0,0);

        // Pedimos al controlador del juego que mueva la cámara a la habitación de respawn
        
        
        rb.WakeUp();
        isDying = false;
    }

}
