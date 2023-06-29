using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float moveSpeed;
        private Vector2 moveDir;
        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        [SerializeField] private AudioSource walkingSound;
        
        public static PlayerMovement Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this; // this is the first instance
            }
            else
            {
                Destroy(gameObject); // this must be a duplicate from a scene reload - DESTROY!
            }
            
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
        
        }

        void Update()
        {
            ProcessInputs();
        }

        private void FixedUpdate()
        {
            Move();   
        }
    
        void ProcessInputs()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            moveDir = new Vector2(moveX, moveY).normalized;
        }

        void Move()
        {
            rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
            // if player move, set animator variable "moving" to true
            // if player is moving left, invert the sprite by multiplying the x scale by -1
            if (moveDir.x != 0 || moveDir.y != 0)
            {
                animator.SetBool("moving", true);
                if (!walkingSound.isPlaying)
                {
                    walkingSound.Play();
                }
                if (moveDir.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
                else if (moveDir.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
            }
            else
            {
                animator.SetBool("moving", false);
                if (walkingSound.isPlaying)
                {
                    walkingSound.Stop();
                }
            }
        }
    }
}
