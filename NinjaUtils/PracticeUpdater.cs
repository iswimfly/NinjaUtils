using Reptile;
using Rewired;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

namespace PracticeUtils
{
    internal class PracticeUpdater : MonoBehaviour
    {
        public static PracticeUpdater Instance;

        private PracticeFunction practiceFunction;
        private PracticeCalls practiceCalls;
        private PracticeGUI practiceGUI;

        private int mGrafs = 0;
        private int lGrafs = 0;
        private int xlGrafs = 0;
        public PracticeUpdater()
        {
            Instance = this;
            practiceFunction = PracticeFunction.Instance;
            practiceCalls = PracticeCalls.Instance;
            practiceGUI = PracticeGUI.Instance;
        }
        public void Update()
        {
            if (practiceCalls.timescaleEnabled && Time.timeScale != practiceCalls.timescale) { Time.timeScale = practiceCalls.timescale; }
            else if (Time.timeScale != 1f) { Time.timeScale = 1f; }

            if (Core.Instance != null)
            {
                practiceCalls.corePuased = Core.Instance.IsCorePaused;

                if (practiceCalls.loadedBaseModule == null) { practiceCalls.loadedBaseModule = FindObjectOfType<BaseModule>(); }

                if (practiceCalls.limitFPS)
                {
                    if (UnityEngine.Application.targetFrameRate != practiceCalls.fpsLimit)
                    {
                        UnityEngine.Application.targetFrameRate = practiceCalls.fpsLimit;
                    }
                }
            }
            else
            {
                practiceCalls.corePuased = true;
            }

            if (WorldHandler.instance != null)
            {
                if (practiceCalls.player != WorldHandler.instance.GetCurrentPlayer()) { practiceCalls.player = WorldHandler.instance.GetCurrentPlayer(); }
            }

            if (practiceCalls.player != null)
            {
                if ((Characters)typeof(Player).GetField("character", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(practiceCalls.player) != practiceCalls.currentCharIndex)
                {
                    practiceCalls.currentCharIndex = (Characters)typeof(Player).GetField("character", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(practiceCalls.player);
                    practiceCalls.currentChar = (int)practiceCalls.currentCharIndex;
                }
                if (Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(practiceCalls.currentCharIndex).moveStyle != practiceCalls.currentStyleIndex)
                {
                    if (Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(practiceCalls.currentCharIndex).moveStyle == MoveStyle.ON_FOOT)
                    {
                        Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(practiceCalls.currentCharIndex).moveStyle = MoveStyle.SKATEBOARD;
                    }

                    practiceCalls.currentStyleIndex = Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(practiceCalls.currentCharIndex).moveStyle;

                    practiceCalls.currentStyle = (int)practiceCalls.currentStyleIndex;
                    practiceFunction.NextStyle(practiceCalls.player, false, true);
                }
                practiceCalls.wallrunLineAbility = (WallrunLineAbility)typeof(Player).GetField("wallrunAbility", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(practiceCalls.player);

                if (practiceCalls.wallrunLineAbility != null)
                {
                    practiceCalls.storageSpeed = (float)typeof(WallrunLineAbility).GetField("lastSpeed", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(practiceCalls.wallrunLineAbility);
                }
                else
                {
                    practiceCalls.storageSpeed = 0;
                }

                practiceCalls.playerSpeed = practiceCalls.player.GetTotalSpeed();
                if (practiceCalls.playerSpeedMax < practiceCalls.playerSpeed) { practiceCalls.playerSpeedMax = practiceCalls.playerSpeed; }
            }
            else
            {
                if (practiceCalls.playerSpeed != 0) { practiceCalls.playerSpeed = 0; }
                if (practiceCalls.storageSpeed != 0) { practiceCalls.storageSpeed = 0f; }
            }

            if (practiceCalls.invul)
            {
                if (practiceCalls.player != null)
                {
                    practiceCalls.player.ResetHP();
                    if (practiceCalls.player.AmountOfCuffs() > 0)
                    {
                        practiceCalls.player.RemoveAllCuffs();
                    }
                }
            }
            practiceCalls.wantedManager = WantedManager.instance;
            if (practiceCalls.wantedManager != null) { practiceCalls.isWanted = practiceCalls.wantedManager.Wanted; } else { practiceCalls.isWanted = false; }

            if (practiceCalls.currentStage != Reptile.Utility.GetCurrentStage() && (Reptile.Utility.GetIsCurrentSceneStage() && WorldHandler.instance.GetCurrentPlayer() != null && practiceCalls.loadedBaseModule != null))
            {
                if (!practiceCalls.loadedBaseModule.IsLoading)
                {
                    practiceCalls.currentStage = Reptile.Utility.GetCurrentStage();

                    practiceCalls.currentRespawner = 0;
                    practiceCalls.respawners.Clear();

                    practiceCalls.currentDreamRespawner = 0;
                    practiceCalls.dreamRespawners.Clear();

                    foreach (var item in WorldHandler.instance.SceneObjectsRegister.playerSpawners.FindAll((PlayerSpawner candidate) => candidate.isRespawner))
                    {
                        practiceCalls.respawners.Add(item.transform.position);
                    };

                    if (WorldHandler.instance.SceneObjectsRegister.RetrieveDreamEncounter() != null)
                    {
                        foreach (var item in WorldHandler.instance.SceneObjectsRegister.RetrieveDreamEncounter().checkpoints)
                        {
                            practiceCalls.dreamRespawners.Add(item.spawnLocation.position);
                        }
                    }
                }
            }

            if (practiceCalls.isMenuing && Core.Instance != null)
            {
                if (practiceCalls.corePuased)
                {
                    if (practiceCalls.GetGameplayCamera(practiceCalls.player) != null)
                    {
                        CameraMode cameraMode = (CameraMode)typeof(GameplayCamera).GetField("cameraMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(practiceCalls.GetGameplayCamera(practiceCalls.player));
                        cameraMode.inputEnabled = true;
                        practiceCalls.isMenuing = false;
                    }
                }
                else if (!Cursor.visible && practiceCalls.GetGameInput() != null && Reptile.Utility.GetIsCurrentSceneStage())
                {
                    practiceCalls.GetGameInput().SetUICursorMode();
                }
            }

            if (Core.Instance != null)
            {
                if (practiceCalls.loadedBaseModule != null && practiceCalls.corePuased)
                {
                    if (practiceCalls.loadedBaseModule.IsInGamePaused)
                    { practiceCalls.isPaused = true; }
                    else { practiceCalls.isPaused = false; }
                }
                else
                {
                    practiceCalls.isPaused = false;
                }
            }
        }

        public void FixedUpdate() 
        {
            if (practiceCalls.fly || practiceCalls.noclip)
            {
                if (practiceCalls.player != null)
                {
                    practiceCalls.flyOff = false;

                    if (practiceCalls.noclip)
                    {
                        practiceCalls.player.GetComponent<Collider>().enabled = false;
                        practiceCalls.player.interactionCollider.enabled = false;
                    }
                    else 
                    {
                        practiceCalls.player.GetComponent<Collider>().enabled = true;
                        practiceCalls.player.interactionCollider.enabled = true;
                    }

                    if (Camera.main != null) { Camera.main.farClipPlane = 20000f; }

                    FieldInfo userInputEnabled = typeof(Player).GetField("userInputEnabled", BindingFlags.Instance | BindingFlags.NonPublic);
                    userInputEnabled.SetValue(practiceCalls.player, false);

                    Transform cameraMode = (Transform)typeof(WorldHandler).GetField("currentCameraTransform", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(WorldHandler.instance);

                    Vector3 velocity = Vector3.zero;

                    float deadzone = 0.01f;

                    float finalFlySpeedForward = 1f;
                    float finalFlySpeedRight = 1f;

                    if (practiceCalls.fly)
                    {
                        finalFlySpeedForward = practiceCalls.flySpeed;
                        finalFlySpeedRight = practiceCalls.flySpeed;
                    } 
                    else
                    {
                        finalFlySpeedForward = practiceCalls.noclipSpeed;
                        finalFlySpeedRight = practiceCalls.noclipSpeed;
                    }

                    if (UnityEngine.Input.GetAxis("Vertical") > deadzone) { finalFlySpeedForward = finalFlySpeedForward * UnityEngine.Input.GetAxis("Vertical"); }
                    else if (UnityEngine.Input.GetAxis("Vertical") < -deadzone) { finalFlySpeedForward = finalFlySpeedForward * (UnityEngine.Input.GetAxis("Vertical") * -1); }

                    if (UnityEngine.Input.GetAxis("Horizontal") > deadzone) { finalFlySpeedRight = finalFlySpeedRight * UnityEngine.Input.GetAxis("Horizontal"); }
                    else if (UnityEngine.Input.GetAxis("Horizontal") < -deadzone) { finalFlySpeedRight = finalFlySpeedRight * (UnityEngine.Input.GetAxis("Horizontal") * -1); }

                    Vector3 forward = finalFlySpeedForward * cameraMode.forward;
                    Vector3 right = finalFlySpeedRight * cameraMode.right;
                    forward.y = 0f;
                    right.y = 0f;

                    practiceCalls.player.CompletelyStop();
                    if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetAxis("Vertical") > deadzone)
                    {
                        practiceCalls.player.motor.rotation = cameraMode.rotation;
                        velocity += forward;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetAxis("Vertical") < -deadzone)
                    {
                        practiceCalls.player.motor.rotation = cameraMode.rotation;
                        velocity += forward * -1;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetAxis("Horizontal") < -deadzone)
                    {
                        practiceCalls.player.motor.rotation = cameraMode.rotation;
                        velocity += right * -1;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetAxis("Horizontal") > deadzone)
                    {
                        practiceCalls.player.motor.rotation = cameraMode.rotation;
                        velocity += right;
                    }

                    if (practiceCalls.fly)
                    {
                        velocity = velocity.normalized * practiceCalls.flySpeed;
                    } 
                    else
                    {
                        velocity = velocity.normalized * practiceCalls.noclipSpeed;
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.Space) || UnityEngine.Input.GetKey(KeyCode.JoystickButton0))
                    {
                        if (practiceCalls.player.IsGrounded())
                        {
                            practiceCalls.player.motor.ForcedUnground();
                            practiceCalls.player.Jump();
                        }
                        velocity.y = 20;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.JoystickButton1))
                    {
                        velocity.y = -20;
                    }
                    else
                    {
                        velocity.y = 0.00f;
                    }

                    practiceCalls.player.SetVelocity(velocity);
                }
                else
                {
                    practiceCalls.fly = false;
                    practiceCalls.noclip = false;
                }
            }
            else
            {
                if (!practiceCalls.flyOff && practiceCalls.player != null)
                {
                    if (Camera.main != null) { Camera.main.farClipPlane = 1000f; }
                    FieldInfo userInputEnabled = typeof(Player).GetField("userInputEnabled", BindingFlags.Instance | BindingFlags.NonPublic);
                    userInputEnabled.SetValue(practiceCalls.player, true);

                    practiceCalls.player.interactionCollider.enabled = true;
                    practiceCalls.player.GetComponent<Collider>().enabled = true;

                    practiceCalls.flyOff = true;
                }
            }

            if (practiceCalls.autoFinishGrafs)
            {
                if (GameObject.Find("GraffitiGame(Clone)") != null)
                {
                    GraffitiGame grafGame = GameObject.Find("GraffitiGame(Clone)").GetComponent<GraffitiGame>();
                    FieldInfo gSpot = typeof(GraffitiGame).GetField("gSpot", BindingFlags.Instance | BindingFlags.NonPublic);
                    FieldInfo state = typeof(GraffitiGame).GetField("state", BindingFlags.Instance | BindingFlags.NonPublic);
                    FieldInfo targetsHit = typeof(GraffitiGame).GetField("targetsHitSequence", BindingFlags.Instance | BindingFlags.NonPublic);
                    
                    GraffitiSpot graffitiSpot = (GraffitiSpot)gSpot.GetValue(grafGame);
                    if ((GraffitiGame.GraffitiGameState)state.GetValue(grafGame) == GraffitiGame.GraffitiGameState.MAIN_STATE)
                    {
                        List<int> targetsHitSequence = new List<int>();
                        var setState = grafGame.GetType().GetMethod("SetState", BindingFlags.NonPublic | BindingFlags.Instance);
                        object[] parameters = new object[] { GraffitiGame.GraffitiGameState.COMPLETE_TARGETS };
                        switch (graffitiSpot.size)
                        {
                            case GraffitiSize.S:

                                break;

                            case GraffitiSize.M:
                                switch (mGrafs)
                                {
                                    case 0:
                                        targetsHitSequence.Add(1);
                                        targetsHitSequence.Add(4);
                                        targetsHitSequence.Add(2);
                                        targetsHitSequence.Add(3);
                                        targetsHitSequence.Add(0);
                                        mGrafs = 1;
                                        break;
                                    case 1:
                                        targetsHitSequence.Add(4);
                                        targetsHitSequence.Add(2);
                                        targetsHitSequence.Add(3);
                                        targetsHitSequence.Add(1);
                                        targetsHitSequence.Add(0);
                                        mGrafs = 0;
                                        break;
                                }
                                
                                targetsHit.SetValue(grafGame, targetsHitSequence);
                                setState.Invoke(grafGame, parameters);
                                break;

                            case GraffitiSize.L:
                                switch (lGrafs)
                                {
                                    case 0:
                                        targetsHitSequence.Add(2);
                                        targetsHitSequence.Add(4);
                                        targetsHitSequence.Add(5);
                                        targetsHitSequence.Add(3);
                                        targetsHitSequence.Add(1);
                                        targetsHitSequence.Add(0);
                                        lGrafs = 1;
                                        break;
                                    case 1:
                                        targetsHitSequence.Add(5);
                                        targetsHitSequence.Add(3);
                                        targetsHitSequence.Add(4);
                                        targetsHitSequence.Add(1);
                                        targetsHitSequence.Add(2);
                                        targetsHitSequence.Add(0);
                                        lGrafs = 0;
                                        break;
                                }
                                
                                targetsHit.SetValue(grafGame, targetsHitSequence);
                                setState.Invoke(grafGame, parameters);
                                break;

                            case GraffitiSize.XL:
                                switch (xlGrafs)
                                {
                                    case 0:
                                        targetsHitSequence.Add(6);
                                        targetsHitSequence.Add(3);
                                        targetsHitSequence.Add(5);
                                        targetsHitSequence.Add(4);
                                        targetsHitSequence.Add(2);
                                        targetsHitSequence.Add(1);
                                        targetsHitSequence.Add(0);
                                        xlGrafs = 1;
                                        break;
                                    case 1:
                                        targetsHitSequence.Add(2);
                                        targetsHitSequence.Add(3);
                                        targetsHitSequence.Add(5);
                                        targetsHitSequence.Add(1);
                                        targetsHitSequence.Add(4);
                                        targetsHitSequence.Add(6);
                                        targetsHitSequence.Add(0);
                                        xlGrafs = 0;
                                        break;
                                }
                                
                                targetsHit.SetValue(grafGame, targetsHitSequence);
                                setState.Invoke(grafGame, parameters);
                                break;


                        }
                    }
                    
                }
            }
        }

    }
}
