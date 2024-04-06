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
        private GameInput gameInput;

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
                if (gameInput == null) { gameInput = practiceCalls.GetGameInput(); }
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

                    float targetSpeed = practiceCalls.fly ? practiceCalls.flySpeed : practiceCalls.noclipSpeed;
                    float finalFlySpeedForward = targetSpeed;
                    float finalFlySpeedRight = targetSpeed;

                    practiceCalls.player.CompletelyStop();
                    if (gameInput != null)
                    {

                        Vector3 hAxis = gameInput.GetAxis(5, 0) * cameraMode.right * targetSpeed;
                        Vector3 vAxis = gameInput.GetAxis(6, 0) * Vector3.Normalize(new Vector3(cameraMode.forward.x, 0f, cameraMode.forward.z)) * targetSpeed;
                        Vector3 axis = hAxis + vAxis;
                        axis.y = 0f;

                        if (axis.magnitude > targetSpeed)
                            axis = axis.normalized * targetSpeed;

                        velocity = axis;

                        if (velocity != Vector3.zero) { practiceCalls.player.motor.rotation = Quaternion.Euler(new Vector3(0f, cameraMode.eulerAngles.y, cameraMode.eulerAngles.z));; }
                    }

                    if (UnityEngine.Input.GetKey(KeyCode.Space) || (gameInput != null && gameInput.GetButtonHeld(7, 0)))
                    {
                        if (practiceCalls.player.IsGrounded())
                        {
                            practiceCalls.player.motor.ForcedUnground();
                            practiceCalls.player.Jump();
                        }
                        velocity.y = 20;
                    }
                    else if (UnityEngine.Input.GetKey(KeyCode.LeftControl) || (gameInput != null && gameInput.GetButtonHeld(65, 0)))
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
