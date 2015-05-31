﻿
using UnityEngine;
using System.Collections;
/* PlayerScript - Handle Input from a player */

public class Player : MonoBehaviour
{
     //Components
     private PlayerMoveController playerMoveController;
     private AttackController attackController;
     public Health health;
     public bool gotAttacked;

     //Player progression
     public bool BowUnlocked;
     public bool UpgradedBow;
     public bool OtherWeaponsUnlocked;
     LoadAndSaveManager.GameStateData.PlayerData DataAboutPlayer;
     public enum SecondaryWeapons { Projectile, Mine };
     public SecondaryWeapons ChosenWeapon;

     //Double tap flags
     private float ButtonCooler;
     private int ButtonCount;
     private float lastTapTimeW;
     private float lastTapTimeS;
     private float lastTapTimeA;
     private float lastTapTimeD;
     private float tapSpeed;

     private int keyCount;
     private bool keyHeldDown;

     //Timers
     public bool isInvincible;
     public float timeSpentInvincible;
     public float attackedTimer;
     public float TimeBowCharging;
     public float BaseTime;
     private float stun_Timer;


     void Awake()
     {
          stun_Timer = 0;
          isInvincible = false;
          timeSpentInvincible = 1;
          playerMoveController = GetComponent<PlayerMoveController>();
          attackController = GetComponent<AttackController>();
          tapSpeed = .25f;
          BowUnlocked = true;
          GameManager.Notifications.AddListener(this, "LevelLoaded");
          GameManager.Notifications.AddListener(this, "PrepareToSave");
          ChosenWeapon = SecondaryWeapons.Projectile;
          //-----------------------------------------------------------change after varible intergrated into player progression system.
          OtherWeaponsUnlocked = true;
     }

     public void LevelLoaded()
     {
          DataAboutPlayer = GameManager.StateManager.GameState.Player;
          BowUnlocked = DataAboutPlayer.IsBowUnlocked;
          UpgradedBow = DataAboutPlayer.IsBowHoldDownUnlocked;
          playerMoveController.SetDashLockState(DataAboutPlayer.IsDashUnlocked);
          OtherWeaponsUnlocked = DataAboutPlayer.IsLandMineUnlocked;
          playerMoveController.setDashSpeed(DataAboutPlayer.DashSpeed);


     }

     public void PrepareToSave()
     {
          BowUnlocked = true;
          DataAboutPlayer.IsBowUnlocked = BowUnlocked;
          DataAboutPlayer.DashSpeed = playerMoveController.getDashSpeed();
          GameManager.StateManager.GameState.Player = DataAboutPlayer;
     }

     // Update is called once per frame
     void Update()
     {
          /* Player Input */
          // Retrieve axis information from keyboard
          float inputX = 0;
          float inputY = 0;

          //Check for the invincibility timer. If the player is invincible, add to the timer. When the timers over, reset the flags
          if (isInvincible)
          {
               timeSpentInvincible += Time.deltaTime;

               if (timeSpentInvincible <= 3f)
               {
                    float remainder = timeSpentInvincible % .3f;
                    GetComponent<Renderer>().enabled = remainder > .15f;
               }
               else
               {
                    isInvincible = false;
                    timeSpentInvincible = 0;
                    GetComponent<Renderer>().enabled = true;
               }
          }

          //The player acts according to input
          if (stun_Timer > 0)
          {
               playerMoveController.Move(0, 0);
               stun_Timer -= Time.deltaTime;
          }
          else
          {
               if (Input.GetButtonDown("Jump"))
               {
                    playerMoveController.Dash();
               }

               if (Input.GetButtonDown("Switch") && OtherWeaponsUnlocked == true)
               {
                    if (ChosenWeapon == SecondaryWeapons.Projectile)
                    {
                         ChosenWeapon = SecondaryWeapons.Mine;
                    }
                    else if (ChosenWeapon == SecondaryWeapons.Mine)
                    {
                         ChosenWeapon = SecondaryWeapons.Projectile;
                    }
               }

               //Check for attack input
               if (Input.GetButtonDown("Fire1") && attackController.CanAttack())
               {
                    attackController.Attack();
               }

               if (Input.GetButtonDown("Fire2"))
               {
                    if (ChosenWeapon == SecondaryWeapons.Projectile)
                    {
                         if (BaseTime == 0)
                         {
                              BaseTime = Time.time;
                         }
                    }
                    if (ChosenWeapon == SecondaryWeapons.Mine)
                    {
                         attackController.PlaceMine();
                    }

               }

               if (Input.GetButtonUp("Fire2") && attackController.CanAttack() && BowUnlocked == true)
               {
                    if (ChosenWeapon == SecondaryWeapons.Projectile)
                    {
                         TimeBowCharging = Time.time;
                         double timeDifference = TimeBowCharging - BaseTime;
                         if (timeDifference < 1.0f)
                         {
                              attackController.ShootProjectile();
                         }
                         else
                         {
                              attackController.ShootProjectile(3);
                         }
                         BaseTime = 0;
                    }
               }


               //Check for how many keys are being pressed and act accordingly
               if (Input.GetButton("Up"))
               {
                    keyCount++;
               }
               if (Input.GetButton("Down"))
               {
                    keyCount++;
               }
               if (Input.GetButton("Left"))
               {
                    keyCount++;
               }
               if (Input.GetButton("Right"))
               {
                    keyCount++;
               }
               if (keyCount >= 2)
               {
                    playerMoveController.isPressingMultiple = true;
               }
               else
               {
                    playerMoveController.isPressingMultiple = false;
               }

               //Only move if the 2 buttons or less are being pressed
               if (keyCount < 3 && keyCount > 0)
               {

                    //Handle double taps for dashing
                    if (Input.GetButtonDown("Up"))
                    {
                         //playerMoveController.newFacing = (int)MoveController.facingDirection.up;

                         if ((Time.time - lastTapTimeW) < tapSpeed)
                         {
                              playerMoveController.Dash();
                         }
                         lastTapTimeW = Time.time;
                    }
                    if (Input.GetButtonDown("Down"))
                    {
                         //playerMoveController.newFacing = (int)MoveController.facingDirection.down;

                         if ((Time.time - lastTapTimeS) < tapSpeed)
                         {
                              playerMoveController.Dash();
                         }
                         lastTapTimeS = Time.time;
                    }
                    if (Input.GetButtonDown("Left"))
                    {
                         //playerMoveController.newFacing = (int)MoveController.facingDirection.left;

                         if ((Time.time - lastTapTimeA) < tapSpeed)
                         {
                              playerMoveController.Dash();
                         }
                         lastTapTimeA = Time.time;
                    }
                    if (Input.GetButtonDown("Right"))
                    {
                         //playerMoveController.newFacing = (int)MoveController.facingDirection.right;

                         if ((Time.time - lastTapTimeD) < tapSpeed)
                         {
                              playerMoveController.Dash();
                         }
                         lastTapTimeD = Time.time;
                    }


                    //Face the player depending on the button being pressed
                    if (Input.GetButton("Up"))
                    {
                          inputY = 1;
                         playerMoveController.newFacing = (int)MoveController.facingDirection.up;
                    }
                    if (Input.GetButton("Down"))
                    {
                          inputY = -1;
                         playerMoveController.newFacing = (int)MoveController.facingDirection.down;

                    }
                    if (Input.GetButton("Left"))
                    {
                          inputX = -1;
                         playerMoveController.newFacing = (int)MoveController.facingDirection.left;
                    }
                    if (Input.GetButton("Right"))
                    {
                          inputX = 1;
                         playerMoveController.newFacing = (int)MoveController.facingDirection.right;
                    }

                    playerMoveController.isMoving = true;
                    playerMoveController.Move(inputX, inputY);
               }

               else
               {
                    playerMoveController.isMoving = false;
                    playerMoveController.Move(0, 0);
               }

               keyCount = 0;
          }
     }

     public void setStun(float st)
     {
          stun_Timer = st;
     }
}

