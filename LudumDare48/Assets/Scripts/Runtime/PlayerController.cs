using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LD48
{
    ///<summary>The class responsible for moving the player</summary>
    public class PlayerController : MonoBehaviour
    {
        //a core part of the whole thing, being paused causes a stop to movement without clearing any buffers.
        //by default starts as unpaused.
        private bool isPaused = false;

        //a maximum of 5 trigger colliders can be handled at the same time.
        private List<int> knownTriggers = new List<int>(5);
        private List<int> triggerVerifyBuffer = new List<int>(5);

        //the attached collider.
        [SerializeField]
        private new CapsuleCollider2D collider;
        [SerializeField]
        private LayerMask groundMask;
        [SerializeField]
        private float verticalGroundCheckOffset = -1.2f, groundCheckTolerance = 0.15f, groundSnapDistance = 0.075f, verticalOriginToGroundOffset = -1;
        [SerializeField]
        private float speed, jumpSpeed = 1.5f;
        [SerializeField]
        private new SpriteRenderer renderer;

#region ANIMATION
        [SerializeField]
        private Animator animator;

        static readonly int IsMovingParam = Animator.StringToHash("IsMoving");

#endregion

        //cache the transform because unity is doodoo
        private new Transform transform;

        private Vector2 velocity;
        private Vector2 desiredVelocity;
        private bool isGrounded = true; //should always start on the ground, so why not.

        void Start()
        {
            transform = base.transform;
            DialogueHandler.OnDialogueEnd += Unpause;
        }

        private void OnDestroy() 
        {
            DialogueHandler.OnDialogueEnd -= Unpause;
        }

        void Update()
        {
            if(!isPaused)
            {
                HandleInput();
                HandleMovement();
                HandleTriggers();
            }
            //animation should always happen
            Animate();
        }

        //Fetches the Input from the InputManager and uses it to manipulate the players speed, etc.
        private void HandleInput()
        {
            float movementInput = Input.GetAxis("Horizontal");
            bool jumpInput = Input.GetButtonDown("Jump");

            desiredVelocity.x = movementInput * speed;
            if(jumpInput && isGrounded)
                desiredVelocity.y = jumpSpeed;

        }

        private void OnDrawGizmos() 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(base.transform.position + new Vector3(0, verticalGroundCheckOffset), groundCheckTolerance);
        }

        //In here the speed is actually used, and collision is handled.
        private void HandleMovement()
        {
            //0. cache deltaTime.
            float deltaTime = Time.deltaTime;

            //0.5 check for ground.
            Vector2 origin = transform.position;
            Vector2 groundCheckPos = origin;
            groundCheckPos.y += verticalOriginToGroundOffset;
            Physics2D.queriesHitTriggers = false;
            Collider2D groundCollider = Physics2D.OverlapCircle(groundCheckPos, groundCheckTolerance, groundMask);
            isGrounded = groundCollider;
            //0.75 snap to the surface.
            if(isGrounded && velocity.y <= 0)
            {
                float groundY = groundCollider.bounds.max.y - verticalOriginToGroundOffset;
                if(Mathf.Abs(groundY - origin.y) <= groundSnapDistance)
                {
                    origin.y = groundY; 
                    //after snapping, also make sure that the desired downwards velocity is really low to avoid collision bugs.
                    if(desiredVelocity.y <= 0)
                    {
                        desiredVelocity.y = 0;
                    }
                }
            }
            //1. Do gravity stuff.
            if(!isGrounded)
            {
                desiredVelocity.y -= 9.81f * deltaTime;
            }
            MoveWithCollision();

            //Making this bit a local method because i shouldnt think about using this outside of this context
            //also because it needs to be able to call itself without fucking things up lol.
            void MoveWithCollision()
            {
                //2. use the desired velocity to check where to go
                Vector2 translation = desiredVelocity * deltaTime;
                Vector2 projectedPosition = origin + translation;

                //3. cast the collider, check for collision on the way there.
                RaycastHit2D[] hits = new RaycastHit2D[5];
                float distance = translation.magnitude;
                Vector2 direction = translation / distance;

                //I HAVE to write this long line because for some stupid reason, unity wont allow CapsuleCollider2D.Cast without a rigidbody???
                //ITS DOING THE EXACT SAME THING, WHY DOESNT ONE WORK WITHOUT A RIGIDBODY????
                Physics2D.queriesHitTriggers = false; //just make this query ignore triggers real quick <.<
                RaycastHit2D hit = Physics2D.CapsuleCast(origin + collider.offset, collider.size, collider.direction, 0, direction, distance, groundMask);
                Physics2D.queriesHitTriggers = true;
                
                //4. update velocity based on travelled distance.
                if(hit)
                {
                    //tiny distance away stops the player from getting stuck, but causes some stutter.
                    Vector2 maxTravelDist = Mathf.Max(0, hit.distance - 0.05f) * direction; 
                    //if bumping into the ceiling, reset velocity to 0, the player should just fall down afterwards.
                    if(hit.normal.y < -0.9f && desiredVelocity.y > 0f)
                    {
                        desiredVelocity.y = 0;
                        MoveWithCollision();
                        return;
                    }
                    if((hit.normal.x > 0.9f && desiredVelocity.x < 0f) || (hit.normal.x < -0.9f && desiredVelocity.x > 0f))
                    {
                        desiredVelocity.x = 0;
                        MoveWithCollision();
                        return;
                    }
                    transform.position = origin + maxTravelDist;
                    //remove velocity based on hit.normal?
                    velocity = maxTravelDist / deltaTime;
                    
                }
                else
                {
                    transform.position = projectedPosition;
                    velocity = desiredVelocity;
                }
            }
        }


        private void Animate()
        {
            //velocity based look direction because uhhh why not
            renderer.flipX = (velocity.x > 0.04f)? false : 
                             (velocity.x < -0.04f)? true : renderer.flipX;
            animator.SetBool(IsMovingParam, Mathf.Abs(velocity.x) > 0.01f);
        }

        private void HandleTriggers()
        {
            //set up the filter.
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = true;
            filter.layerMask = groundMask;
            Collider2D[] results = new Collider2D[5];
            //get the overlap of this collider with others.
            Physics2D.queriesHitTriggers = true;
            int c = collider.OverlapCollider(filter, results);
            //reset the verify buffer
            triggerVerifyBuffer.Clear();

            for(int i = 0; i < c; i++)
            {
                var col = results[i];
                //handle trigger colliders.
                if(col.isTrigger)
                {
                    int triggerID = col.GetInstanceID();
                    triggerVerifyBuffer.Add(triggerID);
                    //trigger not yet registered?
                    if(!knownTriggers.Contains(triggerID))
                    {
                        //a benefit of writing our own "custom physics" is that we can exchange this for, or just add entirely new messages or events.
                        col.SendMessage("OnTriggerEnter2D", collider, SendMessageOptions.DontRequireReceiver);
                        //print("found trigger");
                        knownTriggers.Add(triggerID);
                    }
                }
            }

            //verify knownTriggers with the buffer.
            //I know linq ew, but its much simpler rn, not too concerned with optimization, this game is super small.
            knownTriggers = new List<int>(knownTriggers.Where(item => triggerVerifyBuffer.Contains(item)));
            //Could have TriggerExit here aswell, super easy to implement, but very unnecessary for this scope.
        }

        public void SetPaused(bool paused) 
        { 
            this.isPaused = paused;
            if(isGrounded && paused)
                velocity.x = 0;
        }
        private void Unpause() => SetPaused(false);
    }
}