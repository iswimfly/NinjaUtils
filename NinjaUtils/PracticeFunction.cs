using Reptile;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;

namespace PracticeUtils
{
    internal class PracticeFunction : MonoBehaviour
    {
        public static PracticeFunction Instance;

        private PracticeFunction practiceFunction;
        private PracticeCalls practiceCalls;
        private PracticeGUI practiceGUI;

        public PracticeFunction()
        {
            Instance = this;
            practiceFunction = PracticeFunction.Instance;
            practiceCalls = PracticeCalls.Instance;
            practiceGUI = PracticeGUI.Instance;
        }

        public void ToggleCursor(GameInput gameInput, GameplayCamera gameplayCamera)
        {
            if (gameInput != null && !practiceCalls.corePuased && Reptile.Utility.GetIsCurrentSceneStage() && Reptile.Utility.GetCurrentStage() != Stage.NONE)
            {
                if (!Cursor.visible && !Core.Instance.IsCorePaused)
                {
                    if (gameplayCamera != null)
                    {
                        CameraMode cameraMode = (CameraMode)typeof(GameplayCamera).GetField("cameraMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gameplayCamera);
                        cameraMode.inputEnabled = false;
                    }
                    practiceCalls.isMenuing = true;
                    gameInput.SetUICursorMode();
                }
                else
                {
                    if (gameplayCamera != null)
                    {
                        CameraMode cameraMode = (CameraMode)typeof(GameplayCamera).GetField("cameraMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gameplayCamera);
                        cameraMode.inputEnabled = true;
                    }
                    practiceCalls.isMenuing = false;
                    gameInput.SetGameCursorMode();
                }
            }
        }

        public void FillBoostMax(Player player)
        {
            if (player != null)
            {
                player.AddBoostCharge((float)typeof(Player).GetField("maxBoostCharge", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player));
            }
        }

        public void GoToNextSpawn(Player player)
        {
            if (player != null)
            {
                player.transform.position = practiceCalls.respawners.ToArray()[practiceCalls.currentRespawner];
                if (practiceCalls.currentRespawner + 1 < practiceCalls.respawners.Count()) { practiceCalls.currentRespawner++; } else { practiceCalls.currentRespawner = 0; }
            }
        }

        public void GoToNextDreamSpawn(Player player)
        {
            if (player != null && WorldHandler.instance != null)
            {
                if (WorldHandler.instance.SceneObjectsRegister.RetrieveDreamEncounter() != null)
                {
                    player.transform.position = practiceCalls.dreamRespawners.ToArray()[practiceCalls.currentDreamRespawner];
                    if (practiceCalls.currentDreamRespawner + 1 < practiceCalls.dreamRespawners.Count()) { practiceCalls.currentDreamRespawner++; } else { practiceCalls.currentDreamRespawner = 0; }
                }
            }
        }

        public void SaveLoad(Player player, bool save)
        {
            
            if (player != null)
            {
                if (save)
                {
                    practiceCalls.savedPos = player.tf.position;
                    practiceCalls.savedAng = player.tf.rotation;

                    practiceCalls.savePosX = practiceCalls.savedPos.x.ToString();
                    practiceCalls.savePosY = practiceCalls.savedPos.y.ToString();
                    practiceCalls.savePosZ = practiceCalls.savedPos.z.ToString();

                    practiceCalls.savedStorage = practiceCalls.storageSpeed;
                    practiceCalls.savedStorageS = practiceCalls.savedStorage.ToString();

                    practiceCalls.savedStyle = (MoveStyle)typeof(Player).GetField("moveStyle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player);

                    if (practiceCalls.shouldSaveVel)
                    {
                        practiceCalls.savedVel = player.GetPracticalWorldVelocity();
                        practiceCalls.saveVelX = practiceCalls.savedVel.x.ToString();
                        practiceCalls.saveVelY = practiceCalls.savedVel.y.ToString();
                        practiceCalls.saveVelZ = practiceCalls.savedVel.z.ToString();

                        practiceCalls.savedBoost = player.boostCharge;
                        practiceCalls.savedBoostS = practiceCalls.savedBoost.ToString();
                    }

                }
                else
                {
                    if (float.TryParse(practiceCalls.savePosX, out _) && float.TryParse(practiceCalls.savePosY, out _) && float.TryParse(practiceCalls.savePosZ, out _))
                    {
                        WorldHandler.instance.PlaceCurrentPlayerAt(new Vector3(float.Parse(practiceCalls.savePosX), float.Parse(practiceCalls.savePosY), float.Parse(practiceCalls.savePosZ)), practiceCalls.savedAng, true);
                    }
                    else
                    {
                        WorldHandler.instance.PlaceCurrentPlayerAt(practiceCalls.savedPos, practiceCalls.savedAng, true);
                    }
                    SetStorage(player, practiceCalls.savedStorage);
                    player.SetVelocity(practiceCalls.savedVel);
                    player.boostCharge = practiceCalls.savedBoost;
                    if (practiceCalls.savedStyle == MoveStyle.ON_FOOT)
                    {
                        player.SwitchToEquippedMovestyle(false);
                    }
                    else
                    {
                        player.SetCurrentMoveStyleEquipped(practiceCalls.savedStyle, false, false);
                        player.SwitchToEquippedMovestyle(true, false, showEffect: false);
                    }
                }

                practiceCalls.currentStyleIndex = (MoveStyle)typeof(Player).GetField("moveStyleEquipped", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player);
                Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(practiceCalls.currentCharIndex).moveStyle = practiceCalls.currentStyleIndex;
                Core.Instance.SaveManager.SaveCurrentSaveSlot();
            }
        }

        public void SelectNextStage()
        {
            practiceCalls.selectedStageV++;
            if (practiceCalls.selectedStageV > 7) { practiceCalls.selectedStageV = 0; }
            switch (practiceCalls.selectedStageV)
            {
                case 0:
                    practiceCalls.selectedStage = Stage.hideout;
                    break;
                case 1:
                    practiceCalls.selectedStage = Stage.downhill;
                    break;
                case 2:
                    practiceCalls.selectedStage = Stage.square;
                    break;
                case 3:
                    practiceCalls.selectedStage = Stage.tower;
                    break;
                case 4:
                    practiceCalls.selectedStage = Stage.Mall;
                    break;
                case 5:
                    practiceCalls.selectedStage = Stage.pyramid;
                    break;
                case 6:
                    practiceCalls.selectedStage = Stage.osaka;
                    break;
                case 7:
                    practiceCalls.selectedStage = Stage.Prelude;
                    break;
                default:
                    break;
            }
        }

        public void GoToStage(BaseModule baseModule)
        {
            if (baseModule != null)
            {
                if (!baseModule.IsLoading && !practiceCalls.corePuased && baseModule.StageManager != null && Reptile.Utility.GetCurrentStage() != Stage.NONE && Reptile.Utility.GetIsCurrentSceneStage())
                {
                    if (!baseModule.StageManager.IsExtendingLoadingScreen)
                    {
                        practiceCalls.isMenuing = false;
                        baseModule.StageManager.ExitCurrentStage(practiceCalls.selectedStage, Stage.NONE);
                    }
                }
            }
        }

        public void NextChar(Player player, bool nextChar = true)
        {
            if (player != null)
            {
                if (nextChar) { practiceCalls.currentChar++; } else { practiceCalls.currentChar--; }
                if (practiceCalls.currentChar > (int)Characters.MAX - 1) { practiceCalls.currentChar = 0; }
                else if (practiceCalls.currentChar < 0) { practiceCalls.currentChar = (int)Characters.MAX - 1; }

                bool shouldChange = false;
                if ((MoveStyle)typeof(Player).GetField("moveStyle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player) != MoveStyle.ON_FOOT) { shouldChange = true; }

                player.SetCharacter((Characters)practiceCalls.currentChar);
                practiceCalls.currentCharIndex = (Characters)practiceCalls.currentChar;

                var initHitboxes = player.GetType().GetMethod("InitHitboxes", BindingFlags.NonPublic | BindingFlags.Instance);
                initHitboxes?.Invoke(player, new object[] { });

                var initCuffs = player.GetType().GetMethod("initCuffs", BindingFlags.NonPublic | BindingFlags.Instance);
                initCuffs?.Invoke(player, new object[] { });

                player.SetCurrentMoveStyleEquipped((MoveStyle)practiceCalls.currentStyle, true, true);
                if (shouldChange) { player.SwitchToEquippedMovestyle(true); }
            }
        }

        public void NextStyle(Player player, bool nextStyle = true, bool updateStyle = false)
        {
            if (player != null)
            {
                if (!updateStyle)
                {
                    if (nextStyle) { practiceCalls.currentStyle++; } else { practiceCalls.currentStyle--; }
                    if (practiceCalls.currentStyle > (int)MoveStyle.MAX - 1) { practiceCalls.currentStyle = 1; }
                    else if (practiceCalls.currentStyle < 1) { practiceCalls.currentStyle = (int)MoveStyle.MAX - 1; }
                }

                bool shouldChange = false;
                if ((MoveStyle)typeof(Player).GetField("moveStyle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player) != MoveStyle.ON_FOOT) { shouldChange = true; }

                player.SetCurrentMoveStyleEquipped((MoveStyle)practiceCalls.currentStyle, true, true);
                if (shouldChange) { player.SwitchToEquippedMovestyle(true); }
                practiceCalls.currentStyleIndex = (MoveStyle)practiceCalls.currentStyle;

                Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(practiceCalls.currentCharIndex).moveStyle = (MoveStyle)practiceCalls.currentStyle;
                Core.Instance.SaveManager.SaveCurrentSaveSlot();
            }
        }

        public void NextOutfit(Player player, bool nextOutfit = true)
        {
            if (player != null)
            {
                if (nextOutfit) { practiceCalls.currentOutfit++; } else { practiceCalls.currentOutfit--; }
                if (practiceCalls.currentOutfit > 3) { practiceCalls.currentOutfit = 0; }
                else if (practiceCalls.currentOutfit < 0) { practiceCalls.currentOutfit = 3; }

                player.SetOutfit(practiceCalls.currentOutfit);
            }
        }

        public void LimitFPS()
        {
            if (Core.Instance != null)
            {
                practiceCalls.limitFPS = !practiceCalls.limitFPS;
                if (practiceCalls.limitFPS)
                {
                    UnityEngine.Application.targetFrameRate = practiceCalls.fpsLimit;
                }
                else
                {
                    UnityEngine.Application.targetFrameRate = -1;
                }
            }
        }

        public void EndWanted()
        {
            if (practiceCalls.wantedManager != null)
            {
                practiceCalls.wantedManager.StopPlayerWantedStatus(true);
            }
        }

        public void SetStorage(Player player, float storage)
        {
            if (practiceCalls.wallrunLineAbility != null && player != null)
            {
                FieldInfo lastSpeed = typeof(WallrunLineAbility).GetField("lastSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
                lastSpeed.SetValue(practiceCalls.wallrunLineAbility, storage);
            }
        }

        public void SetBoost(Player player, float boost)
        {
            if (player != null)
            {
                player.boostCharge = boost;
            }
        }

        public void ResetGraffiti(Player player)
        {
            int i = 0;
            string chunksName = "";
            switch (practiceCalls.currentStage)
            {
                case Stage.hideout:
                    i = 0;
                    break;
                case Stage.downhill:
                    chunksName = "stageChunks";
                    i = GameObject.Find(chunksName).transform.childCount;
                    break;
                case Stage.square:
                    chunksName = "Square_Chunks";
                    i = GameObject.Find(chunksName).transform.childCount;
                    break;
                case Stage.tower:
                    chunksName = "StageChunks";
                    i = GameObject.Find(chunksName).transform.childCount;
                    break;
                case Stage.Mall:
                    chunksName = "StageChunks";
                    i = GameObject.Find(chunksName).transform.childCount;
                    break;
                case Stage.pyramid:
                    chunksName = "Chunks";
                    i = GameObject.Find(chunksName).transform.childCount;
                    break;
                case Stage.osaka:
                    chunksName = "StageChunks";
                    i = GameObject.Find(chunksName).transform.childCount;
                    break;
                case Stage.Prelude:
                    chunksName = "stageChunks";
                    i = GameObject.Find(chunksName).transform.childCount;
                    break;

            }
            List<GameObject> chunks = new List<GameObject>();
            if (i > 0)
            {
                for (int j = 0; j < i; j++)
                {
                    if (GameObject.Find(chunksName).transform.GetChild(j).gameObject.activeSelf)
                    {
                        chunks.Add(GameObject.Find(chunksName).transform.GetChild(j).gameObject);
                    }
                    GameObject.Find(chunksName).transform.GetChild(j).gameObject.SetActive(true);
                }
            }
            GraffitiSpot[] grafs = FindObjectsOfType<GraffitiSpot>();
            foreach (GraffitiSpot graf in grafs)
            {
                graf.ResetFirstTime();
                graf.Invoke("ClearPaint", 0);
            }
            FieldInfo rep = typeof(Player).GetField("rep", BindingFlags.Instance | BindingFlags.NonPublic);
            rep.SetValue(practiceCalls.player, 0);

            FieldInfo progress = typeof(StageManager).GetField("currentStageProgress", BindingFlags.Instance | BindingFlags.NonPublic);
            
            StageProgress stageProgress = (StageProgress)progress.GetValue(practiceCalls.loadedBaseModule.StageManager);
            stageProgress.reputation = 0;

            FieldInfo grafsDone = typeof(Player).GetField("graffitiTitlesDone", BindingFlags.Instance | BindingFlags.NonPublic);
            List<string> grafsList = (List<string>)grafsDone.GetValue(player);
            grafsList.Clear();
            if (i > 0)
            {
                for (int j = 0; j < i; j++)
                {
                    if (!chunks.Contains(GameObject.Find(chunksName).transform.GetChild(j).gameObject))
                    {
                        GameObject.Find(chunksName).transform.GetChild(j).gameObject.SetActive(false);
                    }
                }
            }
        }

        public void Bytez()
        {
                FieldInfo carHandler = typeof(Car).GetField("handler", BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (CarsMoveHandler carsMoveHandler in FindObjectsOfType<CarsMoveHandler>())
                {
                    Destroy(carsMoveHandler);
                }
        }


        public void VisualizeZip() 
        {
            foreach (WallrunLine wallrunLine in Resources.FindObjectsOfTypeAll(typeof(WallrunLine)))
            {
                GameObject collisionCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                collisionCube.GetComponent<Collider>().enabled = false;
                collisionCube.transform.position = wallrunLine.transform.TransformPoint(new Vector3(0.49f, 0.5f, 1));
                collisionCube.transform.rotation = wallrunLine.transform.rotation;
                collisionCube.transform.localScale = new Vector3((wallrunLine.transform.localScale.x/40), 0.2f, wallrunLine.transform.localScale.z);
            }
        }
    }
}