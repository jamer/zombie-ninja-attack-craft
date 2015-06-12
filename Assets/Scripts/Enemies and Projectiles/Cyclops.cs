//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;


public class Cyclops : Enemy
{
     public Projectile laser, laserObject;
     public bool canTeleport;
     public float projectileSpeed;
     private AnimationController animationController;

     private Health health;
     private SpriteRenderer sprRend;
     private BoxCollider2D collider;
     private bool isAgro, teleporting;


     private float throwF;
     private Transform playerPos;
     private Vector2 distance, speed, facing, vTemp, v_Transform, teleportRun;
     private double temp, teleportCD, laserCD, idleTime;
     private Vector3 someVec;

     //private Animator animator;


     public void Start()
     {
          // Components
          moveController = GetComponent<EnemyMoveController>();
          animationController = GetComponent<AnimationController>();
          sprRend = GetComponent<SpriteRenderer>();
          collider = GetComponent<BoxCollider2D>();
          health = GetComponent<Health>();
          player = FindObjectOfType<Player>();

          //laser = GetComponent<Projectile> ();
          //laserObject = GetComponent <Projectile> ();

          //rigidbody2D.mass = 10;

          distance = new Vector2(0, 0);
          speed = new Vector2(0, 0);
          isAgro = false;

          rnd = new System.Random(Guid.NewGuid().GetHashCode());
          t = 3 + rnd.Next(0, 3000) / 1000f;

          teleporting = false;
          teleportCD = 11;
          temp = 0;
          canTeleport = true;

          facing = new Vector2(0, 0);

     }

     public void Update()
     {
          checkInvincibility();
          if (checkStun())
          {
               stunTimer -= Time.deltaTime;
               moveController.Move(0, 0);
          }
          else
          {
               rnd = new System.Random();
               findPos();
               if (player != null)
               {
                    if (teleporting)
                    {
                         if (temp > 0)
                         {
                              collider.enabled = false;
                              sprRend.enabled = false;
                              moveController.Move(teleportRun * 2);
                              temp -= Time.deltaTime;
                         }
                         else
                         {
                              collider.enabled = true;
                              sprRend.enabled = true;
                              animationController.isTeleporting = false;
                              moveController.Move(0, 0);
                              teleporting = false;
                         }
                    }
                    else
                    {

                         playerPos = player.transform;
                         //basic aggression range formula
                         distance = playerPos.position - transform.position;
                         if (distance.magnitude <= AgroRange)
                         {
                              isAgro = true;
                         }
                         if (distance.magnitude > AgroRange)
                         {
                              isAgro = false;
                         }

                         if (isAgro)
                         {
                              findPos();
                              float xSp = player.transform.position.x - transform.position.x;
                              float ySp = player.transform.position.y - transform.position.y;
                              //Debug.Log ("xSp: " + xSp + " ySp: " + ySp);
                              moveController.Move(0, 0);
                              if (canTeleport)
                              {
                                   if (distance.magnitude < 0.5 && teleportCD >= 10) {
                                   
                                        animationController.isTeleporting = true;

                                   }
                              }
                              else if (distance.magnitude < 1.5)
                              {
                                   moveController.Move(-direction / 4);
                              }

                              else if (laserCD >= 3)
                              {
                                   moveController.Move(0, 0);

                                        animationController.isAttacking = true;
                                       
                                   

                              }

                             
                         }
                         else
                         {
                              if (idleTime > 0.4)
                              {
                                   someVec = idle(t, rnd);
                                   t = someVec.z;
                                   idleTime = 0;
                              }
                              moveController.Move(someVec.x, someVec.y);
                         }

                         idleTime += Time.deltaTime;
                         t += Time.deltaTime;
                         laserCD += Time.deltaTime;
                         teleportCD += Time.deltaTime;
                         //Debug.Log (t);
                         //GetComponent<Rigidbody2D> ().velocity = speed;
                         //Debug.Log (rigidbody2D.velocity.magnitude);
                    }
               }
          }
     }

     public bool getAgro()
     {
          return isAgro;
     }

     public int currentHp()
     {
          return health.currentHealth;
     }

     public void Shoot()
     {
          Vector3 offset;

          laser = Instantiate(laserObject, transform.position, transform.rotation) as Projectile;
          laser.GetComponent<Rigidbody2D>().velocity = (direction * projectileSpeed);
          laserCD = 0;
     }

     public void DoneShooting()
     {
          animationController.isAttacking = false;
     }

     public void Teleporting()
     {
          sprRend.enabled = false;
          collider.enabled = false;
          teleporting = true;
          animationController.isTeleporting = false;
          teleportRun = direction;
          temp = 0.6f;
          teleportCD = 0;
     }
}
