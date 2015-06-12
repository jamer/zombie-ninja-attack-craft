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

namespace AssemblyCSharp
{
     public class Flamie : Enemy
     {
          //private Player player;
          //public float AgroRange;
          public Projectile fireBlock, fireBlockObject;
          public Explosion explBlock, explBlockObject;

          //private EnemyMoveController moveController;
          private Rigidbody2D rigidbody;
          private Health health;

          private bool isAgro;

          private Transform playerPos;
          private Vector2 distance, speed, facing;
          private double temp, fireBlock_CD, idleTime;
          private Vector3 someVec;
          private double fireDist, vel;
          //private Animator animator;


          public void Start()
          {
               //animator = GetComponent<Animator>();
               moveController = GetComponent<EnemyMoveController>();
               health = GetComponent<Health>();
               player = FindObjectOfType<Player>();
               rigidbody = GetComponent<Rigidbody2D>();

               distance = new Vector2(0, 0);
               speed = new Vector2(0, 0);
               isAgro = false;

               rnd = new System.Random(Guid.NewGuid().GetHashCode());
               t = 3 + rnd.Next(0, 3000) / 1000f;

               //temp is the number for exponential speed when running away
               temp = 1.0000001;
               fireBlock_CD = 0;

               facing = new Vector2(0, 0);
               fireDist = fireBlockObject.GetComponent<Collider2D>().bounds.size.magnitude;
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
                    vel = rigidbody.velocity.magnitude;
                    rnd = new System.Random();
                    currentX = transform.position.x;
                    currentY = transform.position.y;

                    //to offset fireblocks to be a bit behind the flamie, so arrows and sword swings hit the flamie instead of the blocks
                    //Vector3 fireVect = new Vector3(direction.normalized.x/(-8), direction.normalized.y/(-8), 0);

                    //place fire block that deals damage to enemy (projectile that stays in one spot?)
                    //time = distance/speed, create new block after passing the old one.
                    if (vel > 0.1 && fireBlock_CD > .28 / vel)
                    {
                         fireBlock = Instantiate(fireBlockObject, new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f), transform.rotation) as Projectile;
                         fireBlock_CD = 0;
                    }
                    else if (vel < 0.1 && fireBlock_CD > 1)
                    {
                         fireBlock = Instantiate(fireBlockObject, new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f), transform.rotation) as Projectile;
                         fireBlock_CD = 0;

                    }
                    if (player != null)
                    {
                         //basic aggression range formula
                         playerPos = player.transform;
                         float xSp = player.transform.position.x - transform.position.x;
                         float ySp = player.transform.position.y - transform.position.y;
                         direction = new Vector2(xSp, ySp);

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

                              moveController.Move(direction.normalized, 8);

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
                         t -= Time.deltaTime;
                         fireBlock_CD += Time.deltaTime;
                         //cd1 += Time.deltaTime;
                         //cd2 += Time.deltaTime;
                    }
               }
               if (health.currentHp() == 0)
               {
                    onDeath();
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

          public void onDeath()
          {
               //play pre-explosion animation
               Explosion lnd = Instantiate(explBlockObject, transform.position, transform.rotation) as Explosion;
               Debug.Log("WOW! I JUST EXPLODED!");
               //death animation
          }


     }
}


