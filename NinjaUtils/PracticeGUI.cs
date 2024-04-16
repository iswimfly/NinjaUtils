using Reptile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PracticeUtils
{
    internal class PracticeGUI : MonoBehaviour
    {
        public static PracticeGUI Instance;

        private PracticeFunction practiceFunction;
        private PracticeCalls practiceCalls;
        private TriggerTools triggerTools;

        public PracticeGUI()
        {
            Instance = this;
            practiceFunction = PracticeFunction.Instance;
            practiceCalls = PracticeCalls.Instance;
            triggerTools = TriggerTools.Instance;
        }

        private bool open = true;

        private Rect winRect = new Rect(20, 20, 275, 920);

        void OnGUI()
        {
            if (open)
            {
                winRect = GUI.Window(0, winRect, PracticeUtilsGUI, Utils.pluginName + " (" + Utils.pluginVersion + ")");
            }
        }

        private int sidePadding = 10;
        private int elementSizeW = 100;
        private int elementSizeH = 20;
        private int lineSpacing = 2;
        private float buttonSpacing = 1.5f;
        private int linePos;

        void PracticeUtilsGUI(int windowID)
        {
            GUIStyle colorWhite = new GUIStyle();
            colorWhite.normal.textColor = Color.white;
            colorWhite.alignment = TextAnchor.MiddleCenter;

            GUIStyle colorBlack = new GUIStyle();
            colorBlack.normal.textColor = Color.black;
            colorBlack.alignment = TextAnchor.MiddleCenter;

            GUIStyle colorRed = new GUIStyle();
            colorRed.normal.textColor = Color.red;
            colorRed.alignment = TextAnchor.MiddleCenter;

            linePos = 20;

            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Toggle Mouse Input (P)", colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, $"Core ({(practiceCalls.corePuased ? "<color=red>Paused</color>" : "<color=green>Running</color>")})", colorWhite);

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Speed: " + practiceCalls.playerSpeed.ToString("0.00"), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH / 2) + 2;
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Highest Speed: " + practiceCalls.playerSpeedMax.ToString("0.00"), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH / 2) + 2;
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Storage Speed: " + practiceCalls.storageSpeed.ToString("0.00"), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), "Fill Boost (R)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.FillBoostMax(practiceCalls.GetPlayer());
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Toggle invulnerable ({(practiceCalls.invul ? "<color=green>On</color>" : "<color=red>Off</color>")}) (i)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceCalls.invul = !practiceCalls.invul;
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"End Wanted ({(practiceCalls.isWanted ? "<color=red>Wanted</color>" : "<color=green>Safe</color>")}) (K)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.EndWanted();
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Position (X, Y, Z)", colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);

            practiceCalls.savePosX = GUI.TextArea(new Rect(sidePadding, linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), practiceCalls.savePosX);
            practiceCalls.savePosY = GUI.TextArea(new Rect((winRect.width / 2) - (((winRect.width / 3) - (sidePadding * buttonSpacing)) / 2), linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), practiceCalls.savePosY);
            practiceCalls.savePosZ = GUI.TextArea(new Rect(winRect.width - ((winRect.width / 3) - (sidePadding * buttonSpacing)) - sidePadding, linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), practiceCalls.savePosZ);

            if (!float.TryParse(practiceCalls.savePosX, out _)) { practiceCalls.savePosX = practiceCalls.savedPos.x.ToString(); }
            if (!float.TryParse(practiceCalls.savePosY, out _)) { practiceCalls.savePosY = practiceCalls.savedPos.y.ToString(); }
            if (!float.TryParse(practiceCalls.savePosZ, out _)) { practiceCalls.savePosZ = practiceCalls.savedPos.z.ToString(); }

            practiceCalls.savedPos.x = float.Parse(practiceCalls.savePosX);
            practiceCalls.savedPos.y = float.Parse(practiceCalls.savePosY);
            practiceCalls.savedPos.z = float.Parse(practiceCalls.savePosZ);

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Velocity (X, Y, Z)", colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);

            practiceCalls.saveVelX = GUI.TextArea(new Rect(sidePadding, linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), practiceCalls.saveVelX);
            practiceCalls.saveVelY = GUI.TextArea(new Rect((winRect.width / 2) - (((winRect.width / 3) - (sidePadding * buttonSpacing)) / 2), linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), practiceCalls.saveVelY);
            practiceCalls.saveVelZ = GUI.TextArea(new Rect(winRect.width - ((winRect.width / 3) - (sidePadding * buttonSpacing)) - sidePadding, linePos, (winRect.width / 3) - (sidePadding * buttonSpacing), elementSizeH), practiceCalls.saveVelZ);

            if (!float.TryParse(practiceCalls.saveVelX, out _)) { practiceCalls.saveVelX = practiceCalls.savedVel.x.ToString(); }
            if (!float.TryParse(practiceCalls.saveVelY, out _)) { practiceCalls.saveVelY = practiceCalls.savedVel.y.ToString(); }
            if (!float.TryParse(practiceCalls.saveVelZ, out _)) { practiceCalls.saveVelZ = practiceCalls.savedVel.z.ToString(); }

            practiceCalls.savedVel.x = float.Parse(practiceCalls.saveVelX);
            practiceCalls.savedVel.y = float.Parse(practiceCalls.saveVelY);
            practiceCalls.savedVel.z = float.Parse(practiceCalls.saveVelZ);

            linePos = linePos + (elementSizeH + lineSpacing);

            DrawText(sidePadding, linePos, (winRect.width/2) - (sidePadding * 2), elementSizeH, "Saved Storage", colorWhite, colorBlack);
            DrawText(sidePadding, linePos, (winRect.width * 1.5f) - (sidePadding * 2), elementSizeH, "Saved Boost", colorWhite, colorBlack);

            linePos = linePos + (elementSizeH + lineSpacing);
            practiceCalls.savedStorageS = GUI.TextArea(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), practiceCalls.savedStorageS);
            if (!float.TryParse(practiceCalls.savedStorageS, out _)) { practiceCalls.savedStorageS = practiceCalls.savedStorage.ToString(); }
            practiceCalls.savedStorage = float.Parse(practiceCalls.savedStorageS);

            practiceCalls.savedBoostS = GUI.TextArea(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), practiceCalls.savedBoostS);
            if (!float.TryParse(practiceCalls.savedBoostS, out _)) { practiceCalls.savedBoostS = practiceCalls.savedBoost.ToString(); }
            practiceCalls.savedBoost = float.Parse(practiceCalls.savedBoostS);

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Set Storage (O)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.SetStorage(practiceCalls.GetPlayer(), practiceCalls.savedStorage);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Set Boost (B)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.SetBoost(practiceCalls.GetPlayer(), practiceCalls.savedBoost);
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Toggle Saving Velocity + Boost ({(practiceCalls.shouldSaveVel ? "<color=green>On</color>" : "<color=red>Off</color>")})") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceCalls.shouldSaveVel = !practiceCalls.shouldSaveVel;
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Save Position (H)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.SaveLoad(practiceCalls.GetPlayer(), true);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Load Position (J)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.SaveLoad(practiceCalls.GetPlayer(), false);
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Spawns (N)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.GoToNextSpawn(practiceCalls.GetPlayer());
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Dream Spawns (M)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.GoToNextDreamSpawn(practiceCalls.GetPlayer());
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Current Stage: " + practiceCalls.currentStage.ToString(), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH / 2) + 2;
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Selected Stage: " + practiceCalls.selectedStage.ToString(), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), $"Go To Stage {(practiceCalls.isPaused ? "<color=red>(Off)</color>" : "")} (1)") && practiceCalls.isMenuing)
            {
                practiceFunction.GoToStage(practiceCalls.loadedBaseModule);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Select Stage (2)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.SelectNextStage();
            }

            linePos = linePos + ((elementSizeH) + lineSpacing);
            if (practiceCalls.fly) { DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "State: flying", colorWhite, colorBlack); }
            else if (practiceCalls.noclip) { DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "State: noclip", colorWhite, colorBlack); }
            else { DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "State: Normal", colorWhite, colorBlack); }

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Toggle noclip (\\)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceCalls.fly = false;
                practiceCalls.noclip = !practiceCalls.noclip;
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Enable fly (/)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceCalls.noclip = false;
                practiceCalls.fly = !practiceCalls.fly;
            }

            linePos = linePos + (elementSizeH + lineSpacing);
            practiceCalls.noclipSpeedS = GUI.TextArea(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), practiceCalls.noclipSpeedS);
            practiceCalls.flySpeedS = GUI.TextArea(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), practiceCalls.flySpeedS);

            if (!float.TryParse(practiceCalls.noclipSpeedS, out _)) { practiceCalls.noclipSpeedS = practiceCalls.noclipSpeed.ToString(); }
            if (!float.TryParse(practiceCalls.flySpeedS, out _)) { practiceCalls.flySpeedS = practiceCalls.flySpeed.ToString(); }

            practiceCalls.noclipSpeed = float.Parse(practiceCalls.noclipSpeedS);
            practiceCalls.flySpeed = float.Parse(practiceCalls.flySpeedS);

            linePos = linePos + (elementSizeH + lineSpacing);
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Character: " + practiceCalls.currentCharIndex.ToString(), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH / 2) + 2;
            DrawText(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH, "Style: " + practiceCalls.currentStyleIndex.ToString(), colorWhite, colorBlack);

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Prev Char ([)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.NextChar(practiceCalls.GetPlayer(), false);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Next Char (])") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.NextChar(practiceCalls.GetPlayer(), true);
            }

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Prev Style (-)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.NextStyle(practiceCalls.GetPlayer(), false);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Next Style (+)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.NextStyle(practiceCalls.GetPlayer(), true);
            }

            linePos = linePos + (elementSizeH);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding / buttonSpacing) - (winRect.width / 2), elementSizeH), "Prev Outfit (,)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.NextOutfit(practiceCalls.GetPlayer(), false);
            }

            if (GUI.Button(new Rect((winRect.width / 2) + (sidePadding / buttonSpacing), linePos, winRect.width - (sidePadding * buttonSpacing) - (winRect.width / 2), elementSizeH), "Next Outfit (.)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.NextOutfit(practiceCalls.GetPlayer(), true);
            }

            linePos = linePos + (elementSizeH * 2);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Limit FPS ({(practiceCalls.limitFPS ? "<color=green>On</color>" : "<color=red>Off</color>")}) (L)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.LimitFPS();
            }

            linePos = linePos + (elementSizeH);
            practiceCalls.fpsLimitS = GUI.TextArea(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), practiceCalls.fpsLimitS);

            if (!int.TryParse(practiceCalls.fpsLimitS, out _)) { practiceCalls.fpsLimitS = practiceCalls.fpsLimit.ToString(); }
            practiceCalls.fpsLimit = int.Parse(practiceCalls.fpsLimitS);



            linePos = linePos + (elementSizeH * 2);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Toggle Timescale ({(practiceCalls.timescaleEnabled ? "<color=green>On</color>" : "<color=red>Off</color>")}) (T)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceCalls.timescaleEnabled = !practiceCalls.timescaleEnabled;
            }

            linePos = linePos + (elementSizeH);
            practiceCalls.timescaleS = GUI.TextArea(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), practiceCalls.timescaleS);

            if (!float.TryParse(practiceCalls.timescaleS, out _)) { practiceCalls.timescaleS = practiceCalls.timescale.ToString(); }
            practiceCalls.timescale = float.Parse(practiceCalls.timescaleS);

            linePos = linePos + (elementSizeH*2 + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Toggle Triggers ({(triggerTools.DisplayTriggerZones ? "<color=green>On</color>" : "<color=red>Off</color>")}) (X)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                triggerTools.DisplayTriggerZones = !triggerTools.DisplayTriggerZones;
            }

            linePos = linePos + (elementSizeH * 2 + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Reset Area Graffiti/REP (Z)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.ResetGraffiti(practiceCalls.player);
            }

            linePos = linePos + (elementSizeH * 2 + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Freeze All Cars (C)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceFunction.Bytez();
            }

            linePos = linePos + (elementSizeH * 2 + lineSpacing);
            if (GUI.Button(new Rect(sidePadding, linePos, winRect.width - (sidePadding * 2), elementSizeH), $"Toggle Auto Graf ({(practiceCalls.autoFinishGrafs ? "<color=green>On</color>" : "<color=red>Off</color>")}) (G)") && (practiceCalls.isMenuing || practiceCalls.isPaused))
            {
                practiceCalls.autoFinishGrafs = !practiceCalls.autoFinishGrafs;
            }
        }

        void DrawText(float x, float y, float w, float h, string text, GUIStyle textColor, GUIStyle shadowColor = null)
        {
            if (shadowColor != null)
            {
                GUI.Label(new Rect(x + 1, y + 1, w, h), text, shadowColor);
                GUI.Label(new Rect(x + 2, y + 2, w, h), text, shadowColor);
            }

            GUI.Label(new Rect(x, y, w, h), text, textColor);
        }

        public void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.P)) { practiceFunction.ToggleCursor(practiceCalls.GetGameInput(), practiceCalls.GetGameplayCamera(practiceCalls.GetPlayer())); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.R)) { practiceFunction.FillBoostMax(practiceCalls.GetPlayer()); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.N)) { practiceFunction.GoToNextSpawn(practiceCalls.GetPlayer()); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.M)) { practiceFunction.GoToNextDreamSpawn(practiceCalls.GetPlayer()); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.H)) { practiceFunction.SaveLoad(practiceCalls.GetPlayer(), true); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.J)) { practiceFunction.SaveLoad(practiceCalls.GetPlayer(), false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) { practiceFunction.GoToStage(practiceCalls.loadedBaseModule); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) { practiceFunction.SelectNextStage(); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Backslash)) { practiceCalls.fly = false; practiceCalls.noclip = !practiceCalls.noclip; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Slash)) { practiceCalls.noclip = false; practiceCalls.fly = !practiceCalls.fly; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.I)) { practiceCalls.invul = !practiceCalls.invul; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.RightBracket)) { practiceFunction.NextChar(practiceCalls.GetPlayer(), true); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftBracket)) { practiceFunction.NextChar(practiceCalls.GetPlayer(), false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Minus)) { practiceFunction.NextStyle(practiceCalls.GetPlayer(), true); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Equals)) { practiceFunction.NextStyle(practiceCalls.GetPlayer(), false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Period)) { practiceFunction.NextOutfit(practiceCalls.GetPlayer(), true); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Comma)) { practiceFunction.NextOutfit(practiceCalls.GetPlayer(), false); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.L)) { practiceFunction.LimitFPS(); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.K)) { practiceFunction.EndWanted(); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.O)) { practiceFunction.SetStorage(practiceCalls.GetPlayer(), practiceCalls.savedStorage); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.B)) { practiceFunction.SetBoost(practiceCalls.GetPlayer(), practiceCalls.savedBoost); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.T)) { practiceCalls.timescaleEnabled = !practiceCalls.timescaleEnabled; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Quote)) { open = !open; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.X)) { triggerTools.DisplayTriggerZones = !triggerTools.DisplayTriggerZones; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Z)) { practiceFunction.ResetGraffiti(practiceCalls.GetPlayer()); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.C)) { practiceFunction.Bytez(); }
            if (UnityEngine.Input.GetKeyDown(KeyCode.G)) { practiceCalls.autoFinishGrafs = !practiceCalls.autoFinishGrafs; ; }
            //if (UnityEngine.Input.GetKeyDown(KeyCode.V)) { ninjaFunction.VisualizeZip(); }
            //if (UnityEngine.Input.GetKeyDown(KeyCode.B)) { ninjaFunction.HighlightWalls(); }
        }
    }
}
