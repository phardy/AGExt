﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;
using UnityEngine;






namespace ActionGroupsExtended
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class AGXEditor : PartModule
    {


        public static Dictionary<int, bool> IsGroupToggle; //is group a toggle group?
        public static bool[,] ShowGroupInFlight; //Show group in flight?
        
        public static string[] ShowGroupInFlightNames;
        public static int ShowGroupInFlightCurrent = 1;
        
        private List<Part> SelectedWithSym; //selected parts from last frame for Default actions monitoring
        private List<AGXDefaultCheck> SelectedWithSymActions; //monitor baseaction.actiongroups of selected parts
        public static bool NeedToLoadActions = true;
        public static bool LoadFinished = false;
        //Selected Parts Window Variables
        public static Rect SelPartsWin;
        private static Vector2 ScrollPosSelParts;
        public static Vector2 ScrollPosSelPartsActs;
        public static Vector2 ScrollGroups;
        public static  Vector2 CurGroupsWin;
        private static List<AGXPart> AGEditorSelectedParts;
        private static bool AGEEditorSelectedPartsSame = false;
        //private static GUIStyle AGXWinStyle = null;
        private static int SelectedPartsCount = 0;
        private static bool ShowSelectedWin = true;
        private static Part PreviousSelectedPart = null;
        private static bool SelPartsIncSym = true;
        private static string BtnTxt;
        private static bool AGXDoLock = false;
        private static Rect TestWin;
        private static  Part AGXRoot;
        public static List<AGXAction> DetachedPartActions;
        public static Timer DetachedPartReset;

        //private static bool NeedToSave = false;
        private static int GroupsPage = 1;
        private static string CurGroupDesc;
        private static bool AutoHideGroupsWin = false;
        private static bool TempShowGroupsWin = false;
        private static Rect KeyCodeWin;
        private static Rect CurActsWin;
        private static bool ShowKeyCodeWin = false;
        private static bool ShowJoySticks = false;
        private static bool ShowKeySetWin = false;
        private static int CurrentKeySet = 1;
        private static string CurrentKeySetName;
        private static Rect KeySetWin;
        private static ConfigNode AGExtNode;
        static string[] KeySetNames = new string[5];
        private static bool TrapMouse = false;
        //private static int LastPartCount = 0;
        public static List<AGXAction> CurrentVesselActions;
       // private static bool MouseOverExitBtns = false;

        private IButton AGXBtn;



        public static Rect GroupsWin;
        public static bool Trigger;
        //private static bool Trigger2;
        //private static int Value = 0;
        public static string HexStr = "Load";

        private static List<BaseAction> PartActionsList;



        private static bool ShipListOk = false;
        static Texture2D BtnTexRed = new Texture2D(1, 1);
        static Texture2D BtnTexGrn = new Texture2D(1, 1);
        public static Dictionary<int, string> AGXguiNames;
        public static Dictionary<int, KeyCode> AGXguiKeys;
        public static int AGXCurActGroup = 1;
        static List<string> KeyCodeNames = new List<string>();
        static List<string> JoyStickCodes = new List<string>();
        private static bool ActionsListDirty = true; //is our actions requiring update?
        private static bool LoadGroupsOnceCheck = false;
        private static bool ShowCurActsWin = true;
        public static Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>();
        private bool AGXShow = false;
        private static GUISkin AGXSkin;
        private static GUIStyle AGXWinStyle = null;
        //private static GUIStyle TWR1WinStyle = null; //window style
        private static GUIStyle AGXLblStyle = null; //window style
        private static GUIStyle AGXBtnStyle = null; //window style
        private static GUIStyle AGXFldStyle = null; //window style
        Texture2D ButtonTexture = new Texture2D(64, 64);
        Texture2D ButtonTextureRed = new Texture2D(64, 64);
        Texture2D ButtonTextureGreen = new Texture2D(64, 64);

       
        
       
        public void Start()
        {
            //foreach (Part p in 

            //var EdPnl = EditorPanels.Instance.actions;
            //EditorActionGroups.Instance.groupActionsList.AddValueChangedDelegate(OnGroupActionsListChange);
            KSPActs[1] = KSPActionGroup.Custom01;
            KSPActs[2] = KSPActionGroup.Custom02;
            KSPActs[3] = KSPActionGroup.Custom03;
            KSPActs[4] = KSPActionGroup.Custom04;
            KSPActs[5] = KSPActionGroup.Custom05;
            KSPActs[6] = KSPActionGroup.Custom06;
            KSPActs[7] = KSPActionGroup.Custom07;
            KSPActs[8] = KSPActionGroup.Custom08;
            KSPActs[9] = KSPActionGroup.Custom09;
            KSPActs[10] = KSPActionGroup.Custom10;
            
            TestWin = new Rect(600, 300, 100, 200);
            RenderingManager.AddToPostDrawQueue(0, AGXOnDraw);
            AGEditorSelectedParts = new List<AGXPart>();
            PartActionsList = new List<BaseAction>();
            ScrollPosSelParts = Vector2.zero;
            ScrollPosSelPartsActs = Vector2.zero;
            ScrollGroups = Vector2.zero;
            CurGroupsWin = Vector2.zero;
            
            //AGXVsl = new AGXVessel();
            
            AGXWinStyle = new GUIStyle(HighLogic.Skin.window);
            
            BtnTexRed.SetPixel(0, 0, new Color(1, 0, 0, .5f));
            BtnTexRed.Apply();
            BtnTexGrn.SetPixel(0, 0, new Color(0, 1, 0, .5f));
            BtnTexGrn.Apply();
            
            AGXguiNames = new Dictionary<int,string>();
            
            
            AGXguiKeys = new Dictionary<int, KeyCode>();
           

           
            for (int i = 1; i <= 250; i = i + 1)
            {
                AGXguiNames[i] = "";
                AGXguiKeys[i] = KeyCode.None;
            }

            
            KeyCodeNames = new List<String>();
            KeyCodeNames.AddRange(Enum.GetNames(typeof(KeyCode)));
            KeyCodeNames.Remove("None");
            JoyStickCodes.AddRange(KeyCodeNames.Where(JoySticks));
            KeyCodeNames.RemoveAll(JoySticks);
           AGExtNode = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
           if (AGExtNode.GetValue("EditShow") == "0")
           {
               AGXShow = false;
           }
           else
           {
               AGXShow = true;
           }
           CurrentKeySet = Convert.ToInt32(AGExtNode.GetValue("ActiveKeySet"));
           LoadCurrentKeySet();
           CurrentKeySetName = AGExtNode.GetValue("KeySetName" + CurrentKeySet);
           CurrentVesselActions = new List<AGXAction>();
           AGXRoot = null;
           GroupsWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("EdGroupsX")), Convert.ToInt32(AGExtNode.GetValue("EdGroupsY")), 250, 530);
           SelPartsWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("EdSelPartsX")), Convert.ToInt32(AGExtNode.GetValue("EdSelPartsY")), 365, 270);
           KeyCodeWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("EdKeyCodeX")), Convert.ToInt32(AGExtNode.GetValue("EdKeyCodeY")), 410, 730);
           KeySetWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("EdKeySetX")), Convert.ToInt32(AGExtNode.GetValue("EdKeySetY")), 185, 335);
           CurActsWin = new Rect(Convert.ToInt32(AGExtNode.GetValue("EdCurActsX")), Convert.ToInt32(AGExtNode.GetValue("EdCurActsY")), 345, 140);


           LoadCurrentKeyBindings();


           if (ToolbarManager.ToolbarAvailable) //check if toolbar available, load if it is
           {


               AGXBtn = ToolbarManager.Instance.add("AGX", "AGXBtn");
               AGXBtn.TexturePath = "Diazo/AGExt/icon_button";
               AGXBtn.ToolTip = "Action Groups Extended";
               AGXBtn.OnClick += (e) =>
               {
                   //List<UnityEngine.Transform> UIPanelList = new List<UnityEngine.Transform>(); //setup list to find Editor Actions UI transform into a list. Could not figure out how to find just a transform
                   //UIPanelList.AddRange(FindObjectsOfType<UnityEngine.Transform>().Where(n => n.name == "PanelActionGroups")); //actual find command
                   if (EditorLogic.fetch.editorScreen == EditorLogic.EditorScreen.Actions)
                   {
                       if (AGXShow)
                       {
                           //UIPanelList.First().Translate(new Vector3(500f, 0, 0), UIPanelList.First().parent.transform); //hide UI panel
                           AGXShow = false;
                           AGExtNode.SetValue("EditShow", "0");
                           EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.actions);

                       }
                       else
                       {
                           // UIPanelList.First().Translate(new Vector3(-500f, 0, 0), UIPanelList.First().parent.transform); //show UI panel
                           AGXShow = true;
                           AGExtNode.SetValue("EditShow", "1");
                           EditorPanels.Instance.panelManager.Dismiss();
                       }
                       AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
                   }
                   else
                   {
                       EditorLogic.fetch.SelectPanelActions();
                   }
               };
           }
           else
           {
               AGXShow = true; //toolbar not installed, show AGX regardless
           }
           
           DetachedPartActions = new List<AGXAction>();
          
            DetachedPartReset = new Timer();
           DetachedPartReset.Interval = 500;
           
           DetachedPartReset.Stop();
           DetachedPartReset.AutoReset = true;
           
           DetachedPartReset.Elapsed += new ElapsedEventHandler(ResetDetachedParts);

           SelectedWithSym = new List<Part>();
           SelectedWithSymActions = new List<AGXDefaultCheck>();

           EditorPanels.Instance.actions.AddValueChangedDelegate(OnUIChanged); //detect when EditorPanel moves. this ONLY detects editor panel, going from parts to crew will NOT trigger this
           EditorLogic.fetch.crewPanelBtn.AddValueChangedDelegate(OnOtherButtonClick); //detect when Part button clicked at top of screen
           EditorLogic.fetch.partPanelBtn.AddValueChangedDelegate(OnOtherButtonClick); //detect when Crew button clicked at top of screen
           
           IsGroupToggle = new Dictionary<int, bool>();
           ShowGroupInFlight = new bool[6, 251];
           ShowGroupInFlightNames = new string[6];
           
           ShowGroupInFlightNames[1] = "Group 1";
           ShowGroupInFlightNames[2] = "Group 2";
           ShowGroupInFlightNames[3] = "Group 3";
           ShowGroupInFlightNames[4] = "Group 4";
           ShowGroupInFlightNames[5] = "Group 5";
           
           
           
            for (int i = 1; i <= 250; i++)
           {
               IsGroupToggle[i] = false;
               for (int i2 = 1; i2 <= 5; i2++)
               {
                   ShowGroupInFlight[i2, i] = true;
               }
           }
            AGXSkin = (GUISkin)MonoBehaviour.Instantiate(HighLogic.Skin);
            AGXWinStyle = new GUIStyle(AGXSkin.window);
            AGXLblStyle = new GUIStyle(AGXSkin.label);
            AGXFldStyle = new GUIStyle(AGXSkin.textField);
            AGXFldStyle.fontStyle = FontStyle.Normal;
            //AGXFldStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1);
            AGXFldStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            AGXLblStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            AGXLblStyle.wordWrap = false;

            AGXBtnStyle = new GUIStyle(AGXSkin.button);
            AGXBtnStyle.fontStyle = FontStyle.Normal;
            AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
            AGXBtnStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            //AGXScrollStyle.normal.background = null;
            //print("AGX " + AGXBtnStyle.normal.background);
            byte[] importTxt = File.ReadAllBytes(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/ButtonTexture.png");
            byte[] importTxtRed = File.ReadAllBytes(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/ButtonTextureRed.png");
            byte[] importTxtGreen = File.ReadAllBytes(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/ButtonTextureGreen.png");
            //byte[] testXport = AGXBtnStyle.normal.background.EncodeToPNG();
            //File.WriteAllBytes(Application.dataPath + "/SavedScreen.png", testXport);
            ButtonTexture.LoadImage(importTxt);
            ButtonTexture.Apply();
            ButtonTextureRed.LoadImage(importTxtRed);
            ButtonTextureRed.Apply();
            ButtonTextureGreen.LoadImage(importTxtGreen);
            ButtonTextureGreen.Apply();
            AGXBtnStyle.normal.background = ButtonTexture;
           LoadFinished = true;
           
           }

        public void LoadGroupVisibility()
        {
            try
            {
                foreach (PartModule pm in EditorLogic.startPod.Modules.OfType<ModuleAGExtData>())
                {
                    string LoadString = (string)pm.Fields.GetValue("AGXGroupStates");
                    if (LoadString.Length == 1501)
                    {
                        ShowGroupInFlightCurrent = Convert.ToInt32(LoadString.Substring(0, 1));
                        LoadString = LoadString.Substring(1);

                        for (int i = 1; i <= 250; i++)
                        {
                            if (LoadString[0] == '1')
                            {
                                IsGroupToggle[i] = true;
                            }
                            else
                            {
                                IsGroupToggle[i] = false;
                            }
                            LoadString = LoadString.Substring(1);
                            //ReturnStr = ReturnStr + Convert.ToInt16(IsGroupToggle[i]).ToString(); //add toggle state for group
                            for (int i2 = 1; i2 <= 5; i2++)
                            {
                                if (LoadString[0] == '1')
                                {
                                    ShowGroupInFlight[i2, i] = true;
                                }
                                else
                                {
                                    ShowGroupInFlight[i2, i] = false;
                                }
                                LoadString = LoadString.Substring(1);
                                //ReturnStr = ReturnStr + Convert.ToInt16(ShowGroupInFlight[i2, i]).ToString(); //add flight state visibility for each group
                            }
                        }
                    }
                }
            }
            catch
            {
                print("AGX Load Actions Visibility Fail!");
            }
        }
        
        public void OnOtherButtonClick(IUIObject obj) //reset EditorPanel if needed
        {
            //only run this if the Action Panel was hidden by other code
            if(AGXShow)
            {
                EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.actions);
            }
        }

        public void OnUIChanged(IUIObject obj)
        {
         
            if (EditorLogic.fetch.editorScreen == EditorLogic.EditorScreen.Actions) //we in action groups mode?
            {
                if (AGXShow)
                {
                    EditorPanels.Instance.panelManager.Dismiss(); //show AGX? hide panel
                }
                else
                {
                    EditorPanels.Instance.panelManager.BringIn(EditorPanels.Instance.actions); //hide AGX? show panel
                }
            }
        }


        public void SetDefaultAction(BaseAction ba, int group)
        {
            Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>();
            KSPActs[1] = KSPActionGroup.Custom01; //setup list to delete action from 
            KSPActs[2] = KSPActionGroup.Custom02;
            KSPActs[3] = KSPActionGroup.Custom03;
            KSPActs[4] = KSPActionGroup.Custom04;
            KSPActs[5] = KSPActionGroup.Custom05;
            KSPActs[6] = KSPActionGroup.Custom06;
            KSPActs[7] = KSPActionGroup.Custom07;
            KSPActs[8] = KSPActionGroup.Custom08;
            KSPActs[9] = KSPActionGroup.Custom09;
            KSPActs[10] = KSPActionGroup.Custom10;

            ba.actionGroup = ba.actionGroup | KSPActs[group];
        }

        public static void ResetDetachedParts(object source, ElapsedEventArgs e)
        {
            
            DetachedPartReset.Stop();
            DetachedPartActions.Clear();

        }

        public static List<AGXAction> AttachAGXPart(Part p, List<BaseAction> baList, List<AGXAction> agxList)
        {
            List<AGXAction> RetActs = new List<AGXAction>();
            List<Part> symParts = new List<Part>();
            symParts.Add(p);
            symParts.AddRange(p.symmetryCounterparts);
            foreach (AGXAction agAct in DetachedPartActions)
            {
                if (symParts.Contains(agAct.prt))
                {
                    AGXAction ToAdd = new AGXAction() { prt = p, ba = baList.First(b => b.name == agAct.ba.name), group = agAct.group };
                        RetActs.Add(ToAdd);
                }
            }
            return RetActs;
        }

        public void OnDisable()
        {

           
            SaveCurrentKeyBindings();
            SaveWindowPositions();
            if (ToolbarManager.ToolbarAvailable) //if toolbar loaded, destroy button on leaving scene
            {
                AGXBtn.Destroy();
            }
        }

        public static void SaveWindowPositions()
        {
           
                
            AGExtNode.SetValue("EdGroupsX", GroupsWin.x.ToString());
            AGExtNode.SetValue("EdGroupsY", GroupsWin.y.ToString());
            AGExtNode.SetValue("EdSelPartsX", SelPartsWin.x.ToString());
            AGExtNode.SetValue("EdSelPartsY", SelPartsWin.y.ToString());
            AGExtNode.SetValue("EdKeyCodeX", KeyCodeWin.x.ToString());
            AGExtNode.SetValue("EdKeyCodeY", KeyCodeWin.y.ToString());
            AGExtNode.SetValue("EdKeySetX", KeySetWin.x.ToString());
            AGExtNode.SetValue("EdKeySetY", KeySetWin.y.ToString());
            AGExtNode.SetValue("EdCurActsX", CurActsWin.x.ToString());
            AGExtNode.SetValue("EdCurActsY", CurActsWin.y.ToString());
            AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
           
            
        }
        
       

        private static bool JoySticks(String s)
        {
            return s.StartsWith("Joystick");
        }

        public void AGXOnDraw()
        {

            Vector3 RealMousePos = new Vector3();
            RealMousePos = Input.mousePosition;
            RealMousePos.y = Screen.height - Input.mousePosition.y;

            if (!ToolbarManager.ToolbarAvailable)
            {
                AGXShow = true; //no toolbar so show AGX with KSP actions editor still up
            }
            


            if (AGXShow)
            {
                if (ShowKeySetWin)
                {
                    KeySetWin = GUI.Window(673467792, KeySetWin, KeySetWindow, "Keysets", AGXWinStyle);
                    TrapMouse = KeySetWin.Contains(RealMousePos);
                    if (!AutoHideGroupsWin)
                    {
                        GroupsWin = GUI.Window(673467795, GroupsWin, GroupsWindow, "", AGXWinStyle);
                    }
                    ShowCurActsWin = false;

                }

                if (ShowSelectedWin)
                {

                    SelPartsWin = GUI.Window(673467794, SelPartsWin, SelParts, "AGExt Selected parts: " + AGEditorSelectedParts.Count(), AGXWinStyle);
                    ShowCurActsWin = true;
                    TrapMouse = SelPartsWin.Contains(RealMousePos);
                    if (AutoHideGroupsWin && !TempShowGroupsWin)
                    {
                    }
                    else
                    {
                        GroupsWin = GUI.Window(673467795, GroupsWin, GroupsWindow, "", AGXWinStyle);
                        TrapMouse |= GroupsWin.Contains(RealMousePos);
                    }

                    if (ShowKeyCodeWin)
                    {
                        KeyCodeWin = GUI.Window(673467793, KeyCodeWin, KeyCodeWindow, "Keycodes", AGXWinStyle);
                        TrapMouse |= KeyCodeWin.Contains(RealMousePos);
                    }

                }
                if (ShowCurActsWin && ShowSelectedWin)
                {
                    CurActsWin = GUI.Window(673467790, CurActsWin, CurrentActionsWindow, "Actions (This group): " + CurrentVesselActions.FindAll(p => p.group == AGXCurActGroup).Count.ToString(), AGXWinStyle);
                    TrapMouse |= CurActsWin.Contains(RealMousePos);
                }
            }
        }

        public void CurrentActionsWindow(int WindowID)
        {
            HighLogic.Skin.scrollView.normal.background = null;
            List<AGXAction> ThisGroupActions = new List<AGXAction>();
            ThisGroupActions.AddRange(CurrentVesselActions.Where(p => p.group == AGXCurActGroup));
            GUI.Box(new Rect(5, 25, 310, 110), "");
            CurGroupsWin = GUI.BeginScrollView(new Rect(10, 30, 330, 100), CurGroupsWin, new Rect(0, 0, 310, Math.Max(100,0+(20*(ThisGroupActions.Count)))));
            int RowCnt = new int();
            RowCnt = 1;

            if (ThisGroupActions.Count > 0)
            {
                while (RowCnt <= ThisGroupActions.Count)
                {
                    TextAnchor TxtAnch4 = new TextAnchor();
                    TxtAnch4 = GUI.skin.button.alignment;
                   // GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                    AGXBtnStyle.alignment = TextAnchor.MiddleLeft;
                    if (GUI.Button(new Rect(0, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).group.ToString() + ": " + AGXguiNames[ThisGroupActions.ElementAt(RowCnt - 1).group],AGXBtnStyle))
                    {
                        int ToDel = 0;
                        foreach (AGXAction AGXToRemove in CurrentVesselActions)
                        {
                            
                            if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                            {
                                
                                CurrentVesselActions.RemoveAt(ToDel);
                                goto BreakOutA;
                            }
                            ToDel = ToDel + 1;
                        }
                    BreakOutA:
                        SaveCurrentVesselActions();
                    if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                    {
                        RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                    }


                    }

                    if (GUI.Button(new Rect(100, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).prt.partInfo.title, AGXBtnStyle))
                        {
                            int ToDel = 0;
                            foreach (AGXAction AGXToRemove in CurrentVesselActions)
                            {

                                if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                                {
                                    
                                    CurrentVesselActions.RemoveAt(ToDel);
                                    goto BreakOutB;
                                }
                                ToDel = ToDel + 1;
                            }
                        BreakOutB:
                            SaveCurrentVesselActions();
                        if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                        {
                            RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                        }
                        }


                        try
                        {
                            if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), ThisGroupActions.ElementAt(RowCnt - 1).ba.guiName, AGXBtnStyle))
                            {
                                int ToDel = 0;
                                foreach (AGXAction AGXToRemove in CurrentVesselActions)
                                {

                                    if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                                    {
                                       
                                        CurrentVesselActions.RemoveAt(ToDel);
                                        goto BreakOutC;
                                    }
                                    ToDel = ToDel + 1;
                                }
                            BreakOutC:
                                SaveCurrentVesselActions();
                            if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                            {
                                RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                            }
                            }
                        }
                        catch
                        {
                            if (GUI.Button(new Rect(200, 0 + (20 * (RowCnt - 1)), 100, 20), "error", AGXBtnStyle))
                            {
                                int ToDel = 0;
                                foreach (AGXAction AGXToRemove in CurrentVesselActions)
                                {

                                    if (AGXToRemove.group == AGXCurActGroup && AGXToRemove.ba == ThisGroupActions.ElementAt(RowCnt - 1).ba)
                                    {
                                      
                                        CurrentVesselActions.RemoveAt(ToDel);
                                        goto BreakOutD;
                                    }
                                    ToDel = ToDel + 1;
                                }
                            BreakOutD:
                                SaveCurrentVesselActions();
                            if (ThisGroupActions.ElementAt(RowCnt - 1).group < 11)
                            {
                                RemoveDefaultAction(ThisGroupActions.ElementAt(RowCnt - 1).ba, ThisGroupActions.ElementAt(RowCnt - 1).group);
                            }
                            }
                        }

                        AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
                        //GUI.skin.button.alignment = TxtAnch4;
                        RowCnt = RowCnt + 1;
                    }
           
                
                }
            
            else
            {
                TextAnchor TxtAnch5 = new TextAnchor();

                TxtAnch5 = GUI.skin.label.alignment;

                //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                AGXLblStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(10, 30, 274, 30), "No actions",AGXLblStyle);
                //GUI.skin.label.alignment = TxtAnch5;
                AGXLblStyle.alignment = TextAnchor.MiddleLeft;
            }
            GUI.EndScrollView();

            GUI.DragWindow();
        }
   
        public void KeySetWindow(int WindowID)
        {
           
           

           //GUI.DrawTexture(new Rect(6, (CurrentKeySet * 25) +1, 68, 18), BtnTexGrn,if();

            AGXBtnStyle.normal.background = CurrentKeySet == 1 ? ButtonTextureGreen : ButtonTexture; 
            if(GUI.Button(new Rect(5,25,70,20),"Select 1:",AGXBtnStyle))
           {
               SaveCurrentKeyBindings();
               
                CurrentKeySet = 1;
              
                LoadCurrentKeyBindings();
           }
            KeySetNames[0] = GUI.TextField(new Rect(80, 25, 100, 20), KeySetNames[0],AGXFldStyle);
            AGXBtnStyle.normal.background = CurrentKeySet == 2 ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(5, 50, 70, 20), "Select 2:", AGXBtnStyle))
           {

               SaveCurrentKeyBindings(); 
               CurrentKeySet = 2;
              
               LoadCurrentKeyBindings();
           }
           KeySetNames[1] = GUI.TextField(new Rect(80, 50, 100, 20), KeySetNames[1], AGXFldStyle);
           AGXBtnStyle.normal.background = CurrentKeySet == 3 ? ButtonTextureGreen : ButtonTexture;
           if (GUI.Button(new Rect(5, 75, 70, 20), "Select 3:", AGXBtnStyle))
           {

               SaveCurrentKeyBindings(); 
               CurrentKeySet = 3;
               //SaveCurrentKeySet();
               LoadCurrentKeyBindings();
           }
           KeySetNames[2] = GUI.TextField(new Rect(80, 75, 100, 20), KeySetNames[2], AGXFldStyle);
           AGXBtnStyle.normal.background = CurrentKeySet == 4 ? ButtonTextureGreen : ButtonTexture;
           if (GUI.Button(new Rect(5, 100, 70, 20), "Select 4:", AGXBtnStyle))
           {
               SaveCurrentKeyBindings(); 
               CurrentKeySet = 4;
               //SaveCurrentKeySet();
               LoadCurrentKeyBindings();
           }
           KeySetNames[3] = GUI.TextField(new Rect(80, 100, 100, 20), KeySetNames[3], AGXFldStyle);
           AGXBtnStyle.normal.background = CurrentKeySet == 5 ? ButtonTextureGreen : ButtonTexture; 
           if (GUI.Button(new Rect(5, 125, 70, 20), "Select 5:", AGXBtnStyle))
           {
               SaveCurrentKeyBindings(); 
               CurrentKeySet = 5;
              // SaveCurrentKeySet();
               LoadCurrentKeyBindings();
           }
           KeySetNames[4] = GUI.TextField(new Rect(80, 125, 100, 20), KeySetNames[4], AGXFldStyle);
           AGXBtnStyle.normal.background = ButtonTexture;

           Color TxtClr3 = GUI.contentColor;
           GUI.contentColor = new Color(0.5f, 1f, 0f, 1f);
           //GUI.skin.label.fontStyle = FontStyle.Bold;
           AGXLblStyle.fontStyle = FontStyle.Bold;
           TextAnchor TxtAnc = GUI.skin.label.alignment;
           //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
           AGXLblStyle.alignment = TextAnchor.MiddleCenter;
           GUI.Label(new Rect(5, 145, 175, 25), "Actiongroup Groups",AGXLblStyle);
           //GUI.skin.label.fontStyle = FontStyle.Normal;
           //GUI.skin.label.alignment = TxtAnc;
           AGXLblStyle.alignment = TextAnchor.MiddleLeft;
           AGXLblStyle.fontStyle = FontStyle.Normal;
           GUI.contentColor = TxtClr3;

           //GUI.DrawTexture(new Rect(6, (ShowGroupInFlightCurrent * 25) + 141, 68, 18), BtnTexGrn);
           AGXBtnStyle.normal.background = ShowGroupInFlightCurrent == 1 ? ButtonTextureGreen : ButtonTexture;
           if (GUI.Button(new Rect(5, 165, 70, 20), "Group 1:", AGXBtnStyle))
           {
               ShowGroupInFlightCurrent = 1;
           }
           ShowGroupInFlightNames[1] = GUI.TextField(new Rect(80, 165, 100, 20), ShowGroupInFlightNames[1]);
           AGXBtnStyle.normal.background = ShowGroupInFlightCurrent == 2 ? ButtonTextureGreen : ButtonTexture;
           if (GUI.Button(new Rect(5, 190, 70, 20), "Group 2:", AGXBtnStyle))
           {
               ShowGroupInFlightCurrent = 2;
           }
           ShowGroupInFlightNames[2] = GUI.TextField(new Rect(80, 190, 100, 20), ShowGroupInFlightNames[2]);
           AGXBtnStyle.normal.background = ShowGroupInFlightCurrent == 3 ? ButtonTextureGreen : ButtonTexture;
           if (GUI.Button(new Rect(5, 215, 70, 20), "Group 3:", AGXBtnStyle))
           {
               ShowGroupInFlightCurrent = 3;
           }
           ShowGroupInFlightNames[3] = GUI.TextField(new Rect(80, 215, 100, 20), ShowGroupInFlightNames[3]);
           AGXBtnStyle.normal.background = ShowGroupInFlightCurrent == 4 ? ButtonTextureGreen : ButtonTexture;
           if (GUI.Button(new Rect(5, 240, 70, 20), "Group 4:", AGXBtnStyle))
           {
               ShowGroupInFlightCurrent = 4;
           }
           ShowGroupInFlightNames[4] = GUI.TextField(new Rect(80, 240, 100, 20), ShowGroupInFlightNames[4]);
           AGXBtnStyle.normal.background = ShowGroupInFlightCurrent == 5 ? ButtonTextureGreen : ButtonTexture;
           if (GUI.Button(new Rect(5, 265, 70, 20), "Group 5:", AGXBtnStyle))
           {
               ShowGroupInFlightCurrent = 5;
           }
           ShowGroupInFlightNames[5] = GUI.TextField(new Rect(80, 265, 100, 20), ShowGroupInFlightNames[5]);
           AGXBtnStyle.normal.background = ButtonTexture;
            if (GUI.Button(new Rect(5, 300,175,30),"Close Window",AGXBtnStyle))
            {
                
                AGExtNode.SetValue("KeySetName1", KeySetNames[0]);
                AGExtNode.SetValue("KeySetName2", KeySetNames[1]);
                AGExtNode.SetValue("KeySetName3", KeySetNames[2]);
                AGExtNode.SetValue("KeySetName4", KeySetNames[3]);
                AGExtNode.SetValue("KeySetName5", KeySetNames[4]);
                CurrentKeySetName = KeySetNames[CurrentKeySet - 1];
                AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
                foreach (Part p in EditorLogic.fetch.getSortedShipList())
                {
                    foreach (ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                    {
                        pm.AGXGroupStates = SaveGroupVisibility(EditorLogic.startPod, pm.AGXGroupStates);
                        pm.AGXGroupStateNames = SaveGroupVisibilityNames(EditorLogic.startPod, pm.AGXGroupStates);
                    }
                }
                ShowKeySetWin = false;
            }

            GUI.DragWindow();
        }

        public static string SaveCurrentKeySet(Part p, String CurKey)
        {

            if (LoadFinished)
                return CurrentKeySet.ToString();
            else
            {
                return CurKey;
            }
                    
          
                }
     

        public void LoadCurrentKeySet()
        {
           
            bool ShipListOk3 = new bool();
            ShipListOk3 = false;
            try
            {

              
                if (EditorLogic.SortedShipList.Count >= 1)
                {
                    foreach (Part p in EditorLogic.SortedShipList)
                    {
                    }
                    ShipListOk3 = true;
                }
            }
            catch
            {
                
                ShipListOk3 = false;
                CurrentKeySet = 1;
            }
           
            if (ShipListOk3)
            {
                foreach (PartModule pm in EditorLogic.startPod.Modules.OfType<ModuleAGExtData>())
                {
                    CurrentKeySet = Convert.ToInt32(pm.Fields.GetValue("AGXKeySet"));

                }
                
            }
           
            if (CurrentKeySet >= 1 && CurrentKeySet <= 5)
            {
            }
            else
            {
                CurrentKeySet = 1;
            }
            
            if (ShipListOk3)
            {
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    foreach (ModuleAGExtData agpm in p.Modules.OfType<ModuleAGExtData>())
                    {
                        agpm.partCurrentKeySet = CurrentKeySet;
                    }
                }
            }
           
            CurrentKeySetName = AGExtNode.GetValue("KeySetName" + CurrentKeySet);
            
        }

        


        public void LoadCurrentKeyBindings()
        {
          
           
          
            String LoadString = AGExtNode.GetValue("KeySet" + CurrentKeySet.ToString());
          
            for (int i = 1; i <= 250; i++)
            {
                AGXguiKeys[i] = KeyCode.None;
            }
         
            if (LoadString.Length > 0)
            {
                while (LoadString[0] == '\u2023')
                {
                    LoadString = LoadString.Substring(1);

                    int KeyLength = new int();
                    int KeyIndex = new int();
                    KeyCode LoadKey = new KeyCode();
                    KeyLength = LoadString.IndexOf('\u2023');
                   
                    if (KeyLength == -1)
                    {
                       
                        KeyIndex = Convert.ToInt32(LoadString.Substring(0, 3));

                        LoadString = LoadString.Substring(3);

                        LoadKey = (KeyCode)Enum.Parse(typeof(KeyCode), LoadString);
                        AGXguiKeys[KeyIndex] = LoadKey;


                    }
                    else
                    {
                      
                        KeyIndex = Convert.ToInt32(LoadString.Substring(0, 3));

                        LoadString = LoadString.Substring(3);

                        LoadKey = (KeyCode)Enum.Parse(typeof(KeyCode), LoadString.Substring(0, KeyLength - 3));
                        LoadString = LoadString.Substring(KeyLength - 3);

                        AGXguiKeys[KeyIndex] = LoadKey;


                    }

                }
            }
         
        }
        
        public static void SaveCurrentKeyBindings()
        {

            

                AGExtNode.SetValue("KeySetName" + CurrentKeySet, CurrentKeySetName);
                string SaveString = "";
                int KeyID = new int();
                KeyID = 1;
                while (KeyID <= 250)
                {
                    if (AGXguiKeys[KeyID] != KeyCode.None)
                    {
                        SaveString = SaveString + '\u2023' + KeyID.ToString("000") + AGXguiKeys[KeyID].ToString();
                    }
                    KeyID = KeyID + 1;
                }

                AGExtNode.SetValue("KeySet" + CurrentKeySet.ToString(), SaveString);
                AGExtNode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AGExt/AGExt.cfg");
                if (CurrentKeySet == 1)
                {
                    SaveDefaultCustomKeys();
                }
              
            
        }
        public static void SaveDefaultCustomKeys()
        {
            GameSettings.CustomActionGroup1.primary = AGXguiKeys[1]; //copy keys to KSP itself
            GameSettings.CustomActionGroup2.primary = AGXguiKeys[2];
            GameSettings.CustomActionGroup3.primary = AGXguiKeys[3];
            GameSettings.CustomActionGroup4.primary = AGXguiKeys[4];
            GameSettings.CustomActionGroup5.primary = AGXguiKeys[5];
            GameSettings.CustomActionGroup6.primary = AGXguiKeys[6];
            GameSettings.CustomActionGroup7.primary = AGXguiKeys[7];
            GameSettings.CustomActionGroup8.primary = AGXguiKeys[8];
            GameSettings.CustomActionGroup9.primary = AGXguiKeys[9];
            GameSettings.CustomActionGroup10.primary = AGXguiKeys[10];
            GameSettings.SaveSettings(); //save keys to disk
            GameSettings.CustomActionGroup1.primary = KeyCode.None; //unbind keys so they don't conflict
            GameSettings.CustomActionGroup2.primary = KeyCode.None;
            GameSettings.CustomActionGroup3.primary = KeyCode.None;
            GameSettings.CustomActionGroup4.primary = KeyCode.None;
            GameSettings.CustomActionGroup5.primary = KeyCode.None;
            GameSettings.CustomActionGroup6.primary = KeyCode.None;
            GameSettings.CustomActionGroup7.primary = KeyCode.None;
            GameSettings.CustomActionGroup8.primary = KeyCode.None;
            GameSettings.CustomActionGroup9.primary = KeyCode.None;
            GameSettings.CustomActionGroup10.primary = KeyCode.None;
        }
        public void KeyCodeWindow(int WindowID)
        {
            if (GUI.Button(new Rect(5, 3, 100, 25), "None", AGXBtnStyle))
            {
                AGXguiKeys[AGXCurActGroup] = KeyCode.None;
                ShowKeyCodeWin = false;

            }
            
            //if (ShowJoySticks)
            //{
            //    GUI.DrawTexture(new Rect(281, 3, 123, 18), BtnTexGrn);
            //}
            AGXBtnStyle.normal.background = ShowJoySticks ? ButtonTextureGreen : ButtonTexture;
            if (GUI.Button(new Rect(280, 2, 125, 20), "Show JoySticks", AGXBtnStyle))
            {
                ShowJoySticks= !ShowJoySticks;
            }
            AGXBtnStyle.normal.background = ButtonTexture;
            if (!ShowJoySticks)
            {
                int KeyListCount = new int();
                KeyListCount = 0;
                while (KeyListCount <= 34)
                {
                    if (GUI.Button(new Rect(5, 25 + (KeyListCount * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                        ShowKeyCodeWin = false;

                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 69)
                {
                    if (GUI.Button(new Rect(105, 25 + ((KeyListCount - 35) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                        ShowKeyCodeWin = false;

                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 104)
                {
                    if (GUI.Button(new Rect(205, 25 + ((KeyListCount - 70) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                        ShowKeyCodeWin = false;

                    }
                    KeyListCount = KeyListCount + 1;
                }
                while (KeyListCount <= 139)
                {
                    if (GUI.Button(new Rect(305, 25 + ((KeyListCount - 105) * 20), 100, 20), KeyCodeNames.ElementAt(KeyListCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(KeyListCount));
                        ShowKeyCodeWin = false;

                    }
                    KeyListCount = KeyListCount + 1;
                }
            }
            else
            {
                int JoyStickCount = new int();
                JoyStickCount = 0;
                while (JoyStickCount <= 34)
                {
                    if (GUI.Button(new Rect(5, 25 + (JoyStickCount * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(JoyStickCount));
                        ShowKeyCodeWin = false;

                    }
                    JoyStickCount = JoyStickCount + 1;
                }
                while (JoyStickCount <= 69)
                {
                    if (GUI.Button(new Rect(130, 25 + ((JoyStickCount - 35) * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(JoyStickCount));
                        ShowKeyCodeWin = false;

                    }
                    JoyStickCount = JoyStickCount + 1;
                }
                while (JoyStickCount <= 99)
                {
                    if (GUI.Button(new Rect(255, 25 + ((JoyStickCount - 70) * 20), 125, 20), JoyStickCodes.ElementAt(JoyStickCount), AGXBtnStyle))
                    {
                        AGXguiKeys[AGXCurActGroup] = (KeyCode)Enum.Parse(typeof(KeyCode), KeyCodeNames.ElementAt(JoyStickCount));
                        ShowKeyCodeWin = false;

                    }
                    JoyStickCount = JoyStickCount + 1;
                }
            }
            GUI.DragWindow();
        }

        public void RemoveDefaultAction(BaseAction ba, int group)
        {
            Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>();
            KSPActs[1] = KSPActionGroup.Custom01; //setup list to delete action from 
            KSPActs[2] = KSPActionGroup.Custom02;
            KSPActs[3] = KSPActionGroup.Custom03;
            KSPActs[4] = KSPActionGroup.Custom04;
            KSPActs[5] = KSPActionGroup.Custom05;
            KSPActs[6] = KSPActionGroup.Custom06;
            KSPActs[7] = KSPActionGroup.Custom07;
            KSPActs[8] = KSPActionGroup.Custom08;
            KSPActs[9] = KSPActionGroup.Custom09;
            KSPActs[10] = KSPActionGroup.Custom10;

            ba.actionGroup = ba.actionGroup & ~KSPActs[group];

        }

        public void SelParts(int WindowID)
        {

            HighLogic.Skin.scrollView.normal.background = null;

            SelectedPartsCount = AGEditorSelectedParts.Count;
            int SelPartsLeft = new int();
            SelPartsLeft = -10;


            //GUI.DrawTexture(new Rect(25, 30, 80, PartsScrollHeight), TexBlk, ScaleMode.StretchToFill, false);
            //AGXPart FirstPart = new AGXPart();
            // FirstPart = AGEditorSelectedParts.First().AGPart.name;
            GUI.Box(new Rect(SelPartsLeft + 20, 25, 200, 110), "");
            if (AGEditorSelectedParts != null && AGEditorSelectedParts.Count > 0)
            {

                int ButtonCount = new int();
                ButtonCount = 1;

                ScrollPosSelParts = GUI.BeginScrollView(new Rect(SelPartsLeft + 20, 30, 220, 110), ScrollPosSelParts, new Rect(0, 0, 200, (20 * Math.Max(5, SelectedPartsCount)) + 10));

                //GUI.Box(new Rect(SelPartsLeft, 25, 200, (20 * Math.Max(5, SelectedPartsCount)) + 10), "");
                while (ButtonCount <= SelectedPartsCount)
                {

                    if (GUI.Button(new Rect(5, 0 + ((ButtonCount - 1) * 20), 190, 20), AGEditorSelectedParts.ElementAt(ButtonCount - 1).AGPart.partInfo.title, AGXBtnStyle))
                    {

                        AGEditorSelectedParts.RemoveAt(ButtonCount - 1);
                        if (AGEditorSelectedParts.Count == 0)
                        {
                            AGEEditorSelectedPartsSame = false;
                        }
                        return;
                    }

                    ButtonCount = ButtonCount + 1;
                }

                GUI.EndScrollView();



            }

            if (SelPartsIncSym)
            {
                //GUI.DrawTexture(new Rect(SelPartsLeft + 246, 26, 108, 23), BtnTexGrn, ScaleMode.StretchToFill, false);
                AGXBtnStyle.normal.background = ButtonTextureGreen;
                BtnTxt = "Symmetry? Yes";
            }
            else
            {
                //GUI.DrawTexture(new Rect(SelPartsLeft + 246, 26, 108, 23), BtnTexRed, ScaleMode.StretchToFill, false);
                AGXBtnStyle.normal.background = ButtonTextureRed;
                BtnTxt = "Symmetry? No";
            }



            if (GUI.Button(new Rect(SelPartsLeft + 245, 25, 110, 25), BtnTxt, AGXBtnStyle))
            {
                SelPartsIncSym = !SelPartsIncSym;

            }
            AGXBtnStyle.normal.background = ButtonTexture;
            if (GUI.Button(new Rect(SelPartsLeft + 245, 55, 110, 25), "Clear List", AGXBtnStyle))
            {
                AGEditorSelectedParts.Clear();
                PartActionsList.Clear();
                AGEEditorSelectedPartsSame = false;

            }

            GUI.Box(new Rect(SelPartsLeft + 20, 150, 200, 110), "");

            if (AGEEditorSelectedPartsSame)
            {

                if (PartActionsList.Count > 0)
                {
                    int ActionsCount = new int();
                    int ActionsCountTotal = new int();
                    ActionsCount = 1;
                    ActionsCountTotal = PartActionsList.Count;
                    ScrollPosSelPartsActs = GUI.BeginScrollView(new Rect(SelPartsLeft + 20, 155, 220, 110), ScrollPosSelPartsActs, new Rect(0, 0, 200, (20 * Math.Max(5, ActionsCountTotal)) + 10));

                    while (ActionsCount <= ActionsCountTotal)
                    {

                        if (GUI.Button(new Rect(5, 0 + ((ActionsCount - 1) * 20), 190, 20), PartActionsList.ElementAt(ActionsCount - 1).guiName, AGXBtnStyle))
                        {

                            int PrtCnt = new int();
                            PrtCnt = 0;
                            foreach (AGXPart agPrt in AGEditorSelectedParts)
                            {
                                List<BaseAction> ThisPartActionsList = new List<BaseAction>();
                                ThisPartActionsList.AddRange(agPrt.AGPart.Actions);
                                foreach (PartModule pm3 in agPrt.AGPart.Modules)
                                {
                                    ThisPartActionsList.AddRange(pm3.Actions);
                                }
                                AGXAction ToAdd = new AGXAction();
                                if (ThisPartActionsList.ElementAt(ActionsCount - 1).guiName == PartActionsList.ElementAt(ActionsCount - 1).guiName)
                                {
                                   ToAdd = new AGXAction() { prt = agPrt.AGPart, ba = ThisPartActionsList.ElementAt(ActionsCount - 1), group = AGXCurActGroup, activated = false };
                                }
                                else
                                {
                                    ToAdd = new AGXAction() { prt = agPrt.AGPart, ba = PartActionsList.ElementAt(ActionsCount - 1), group = AGXCurActGroup, activated = false };
                                }
                                List<AGXAction> Checking = new List<AGXAction>();
                                Checking.AddRange(CurrentVesselActions);
                                Checking.RemoveAll(p => p.group != ToAdd.group);

                                Checking.RemoveAll(p => p.prt != ToAdd.prt);
                                
                                Checking.RemoveAll(p => p.ba != ToAdd.ba);
                              


                                if (Checking.Count == 0)
                                {
                                    
                                    CurrentVesselActions.Add(ToAdd);
                                    SaveCurrentVesselActions();
                                }
                                PrtCnt = PrtCnt + 1;
                                if (ToAdd.group < 11)
                                {
                                    SetDefaultAction(ToAdd.ba, ToAdd.group);
                                }
                            }
                            
                            
                        }

                        ActionsCount = ActionsCount + 1;
                    }
                    GUI.EndScrollView();
                }

                // 
            }
            else
            {
                TextAnchor TxtAnch = new TextAnchor();

                TxtAnch = GUI.skin.label.alignment;

                //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                AGXLblStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(SelPartsLeft + 20, 180, 190, 40), "Select parts of\nthe same type",AGXBtnStyle);


                AGXLblStyle.alignment = TextAnchor.MiddleLeft;
                //GUI.skin.label.alignment = TxtAnch;

            }
           

            TextAnchor TxtAnch2 = new TextAnchor();
            TxtAnch2 = GUI.skin.button.alignment;
            //GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            AGXBtnStyle.alignment = TextAnchor.MiddleLeft;
            if (GUI.Button(new Rect(SelPartsLeft + 245, 85, 120, 30), AGXCurActGroup + ": " + AGXguiNames[AGXCurActGroup], AGXBtnStyle))
            {
                TempShowGroupsWin = true;
            }
            //GUI.skin.button.alignment = TxtAnch2;
            AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
            if (IsGroupToggle[AGXCurActGroup])
            {

                Color TxtClr = GUI.contentColor;
                GUI.contentColor = Color.green;
                if (GUI.Button(new Rect(SelPartsLeft + 245, 160, 110, 22), "Toggle Grp: Yes", AGXBtnStyle))
                {

                    IsGroupToggle[AGXCurActGroup] = false;
                }
                GUI.contentColor = TxtClr;
            }
            else
            {
                if (GUI.Button(new Rect(SelPartsLeft + 245, 160, 110, 22), "Toggle Grp: No", AGXBtnStyle))
                {

                    IsGroupToggle[AGXCurActGroup] = true;
                }
            }
            GUI.Label(new Rect(SelPartsLeft + 231, 183, 110, 22), "Show:",AGXLblStyle);
            Color TxtClr2 = GUI.contentColor;

            if (ShowGroupInFlight[1, AGXCurActGroup])
            {
                GUI.contentColor = Color.green;
            }
            else
            {
                GUI.contentColor = Color.red;
            }
            if (GUI.Button(new Rect(SelPartsLeft + 271, 183, 20, 22), "1", AGXBtnStyle))
            {
                ShowGroupInFlight[1, AGXCurActGroup] = !ShowGroupInFlight[1, AGXCurActGroup];
            }

            if (ShowGroupInFlight[2, AGXCurActGroup])
            {
                GUI.contentColor = Color.green;
            }
            else
            {
                GUI.contentColor = Color.red;
            }
            if (GUI.Button(new Rect(SelPartsLeft + 291, 183, 20, 22), "2", AGXBtnStyle))
            {
                ShowGroupInFlight[2, AGXCurActGroup] = !ShowGroupInFlight[2, AGXCurActGroup];
                //CalculateActionsToShow();
            }

            if (ShowGroupInFlight[3, AGXCurActGroup])
            {
                GUI.contentColor = Color.green;
            }
            else
            {
                GUI.contentColor = Color.red;
            }
            if (GUI.Button(new Rect(SelPartsLeft + 311, 183, 20, 22), "3", AGXBtnStyle))
            {
                ShowGroupInFlight[3, AGXCurActGroup] = !ShowGroupInFlight[3, AGXCurActGroup];
                //CalculateActionsToShow();
            }

            if (ShowGroupInFlight[4, AGXCurActGroup])
            {
                GUI.contentColor = Color.green;
            }
            else
            {
                GUI.contentColor = Color.red;
            }
            if (GUI.Button(new Rect(SelPartsLeft + 331, 183, 20, 22), "4", AGXBtnStyle))
            {
                ShowGroupInFlight[4, AGXCurActGroup] = !ShowGroupInFlight[4, AGXCurActGroup];
               // CalculateActionsToShow();
            }

            if (ShowGroupInFlight[5, AGXCurActGroup])
            {
                GUI.contentColor = Color.green;
            }
            else
            {
                GUI.contentColor = Color.red;
            }
            if (GUI.Button(new Rect(SelPartsLeft + 351, 183, 20, 22), "5", AGXBtnStyle)) 
            {
                ShowGroupInFlight[5, AGXCurActGroup] = !ShowGroupInFlight[5, AGXCurActGroup];
               // CalculateActionsToShow();
            }
            GUI.contentColor = TxtClr2;
            GUI.Label(new Rect(SelPartsLeft + 245, 115, 110, 20), "Description:",AGXLblStyle);
            CurGroupDesc = AGXguiNames[AGXCurActGroup];
            CurGroupDesc = GUI.TextField(new Rect(SelPartsLeft + 245, 135, 120, 22), CurGroupDesc,AGXFldStyle);
            AGXguiNames[AGXCurActGroup] = CurGroupDesc;
            GUI.Label(new Rect(SelPartsLeft + 245, 203, 110, 25), "Keybinding:",AGXLblStyle);
            if (GUI.Button(new Rect(SelPartsLeft + 245, 222, 120, 20), AGXguiKeys[AGXCurActGroup].ToString(),AGXBtnStyle))
            {
                ShowKeyCodeWin = true;
            }
            if (GUI.Button(new Rect(SelPartsLeft + 245, 244, 120, 20),CurrentKeySetName,AGXBtnStyle))
            {
                SaveCurrentKeyBindings();
               KeySetNames[0] = AGExtNode.GetValue("KeySetName1");
                KeySetNames[1] = AGExtNode.GetValue("KeySetName2");
                KeySetNames[2] = AGExtNode.GetValue("KeySetName3");
                KeySetNames[3] = AGExtNode.GetValue("KeySetName4");
                KeySetNames[4] = AGExtNode.GetValue("KeySetName5");
                KeySetNames[CurrentKeySet - 1] = CurrentKeySetName; 
                ShowKeySetWin = true;
            }


            GUI.DragWindow();
        }

        public void SaveCurrentVesselActions()
        {
            string errLine = "1";
            try
            {
                errLine = "2";
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    errLine = "3";
                    foreach (ModuleAGExtData agpm in p.Modules.OfType<ModuleAGExtData>())
                    {
                        errLine = "4";
                        agpm.partAGActions.Clear();
                        errLine = "5";
                        agpm.partAGActions.AddRange(CurrentVesselActions.Where(agp => agp.prt == p));
                        errLine = "6";
                    }
                }
            }
            catch (Exception e)
            {
                print("AGX Editor Fail (SaveCurrentVesselActions) " + errLine + " " + e);
            }
        }

        public void GroupsWindow(int WindowID) 
        {

           
            //if (AutoHideGroupsWin)
            //{
            //    GUI.DrawTexture(new Rect(6, 4, 78, 18), BtnTexRed);
            //}
            AGXBtnStyle.normal.background = AutoHideGroupsWin ? ButtonTextureRed : ButtonTexture;
            if (GUI.Button(new Rect(5, 3, 80, 20), "Auto-Hide", AGXBtnStyle))
            {
                AutoHideGroupsWin = !AutoHideGroupsWin;
                
            }
            AGXBtnStyle.normal.background = ButtonTexture;
            bool[] PageGrn = new bool[5];
            foreach (AGXAction AGact in CurrentVesselActions)
            {
                if (AGact.group <= 50)
                {
                    PageGrn[0] = true;
                }
                if (AGact.group >= 51 && AGact.group <= 100)
                {
                    PageGrn[1] = true;
                }
                if (AGact.group >= 101 && AGact.group <= 150)
                {
                    PageGrn[2] = true;
                }
                if (AGact.group >= 151 && AGact.group <= 200)
                {
                    PageGrn[3] = true;
                }
                if (AGact.group >= 201 && AGact.group <= 250)
                {
                    PageGrn[4] = true;
                }
            }

            //for (int i = 1; i <= 5; i = i + 1)
            //{
            //    if (PageGrn[i - 1] == true && GroupsPage != i)
            //    {
            //        GUI.DrawTexture(new Rect(96 + (i * 25), 4, 23, 18), BtnTexGrn);
            //    }
            //}


            //GUI.DrawTexture(new Rect(96 + (GroupsPage * 25), 4, 23, 18), BtnTexRed);
            //AGXBtnStyle.normal.background = PageGrn[0] ? ButtonTextureGreen : ButtonTexture;
            //AGXBtnStyle.normal.background = GroupsPage == 1 ? ButtonTextureRed : ButtonTexture;
            if (GroupsPage == 1)
            {
                AGXBtnStyle.normal.background = ButtonTextureRed;
            }
            else if (PageGrn[0])
            {
                AGXBtnStyle.normal.background = ButtonTextureGreen;
            }
            else
            {
                AGXBtnStyle.normal.background = ButtonTexture;
            }
            if (GUI.Button(new Rect(120, 3, 25, 20), "1",AGXBtnStyle))
            {
                GroupsPage = 1;
            }
            //AGXBtnStyle.normal.background = PageGrn[1] ? ButtonTextureGreen : ButtonTexture;
            //AGXBtnStyle.normal.background = GroupsPage == 2 ? ButtonTextureRed : ButtonTexture;
            if (GroupsPage == 2)
            {
                AGXBtnStyle.normal.background = ButtonTextureRed;
            }
            else if (PageGrn[1])
            {
                AGXBtnStyle.normal.background = ButtonTextureGreen;
            }
            else
            {
                AGXBtnStyle.normal.background = ButtonTexture;
            }
            if (GUI.Button(new Rect(145, 3, 25, 20), "2", AGXBtnStyle))
            {
                GroupsPage = 2;
            }
            //AGXBtnStyle.normal.background = PageGrn[2] ? ButtonTextureGreen : ButtonTexture;
            //AGXBtnStyle.normal.background = GroupsPage == 3 ? ButtonTextureRed : ButtonTexture;
            if (GroupsPage == 3)
            {
                AGXBtnStyle.normal.background = ButtonTextureRed;
            }
            else if (PageGrn[2])
            {
                AGXBtnStyle.normal.background = ButtonTextureGreen;
            }
            else
            {
                AGXBtnStyle.normal.background = ButtonTexture;
            }
            if (GUI.Button(new Rect(170, 3, 25, 20), "3", AGXBtnStyle))
            {
                GroupsPage = 3;
            }
            //AGXBtnStyle.normal.background = PageGrn[3] ? ButtonTextureGreen : ButtonTexture;
            //AGXBtnStyle.normal.background = GroupsPage == 4 ? ButtonTextureRed : ButtonTexture;
            if (GroupsPage == 4)
            {
                AGXBtnStyle.normal.background = ButtonTextureRed;
            }
            else if (PageGrn[3])
            {
                AGXBtnStyle.normal.background = ButtonTextureGreen;
            }
            else
            {
                AGXBtnStyle.normal.background = ButtonTexture;
            }
            if (GUI.Button(new Rect(195, 3, 25, 20), "4", AGXBtnStyle))
            {
                GroupsPage = 4;
            }
            //AGXBtnStyle.normal.background = PageGrn[4] ? ButtonTextureGreen : ButtonTexture;
            //AGXBtnStyle.normal.background = GroupsPage == 5 ? ButtonTextureRed : ButtonTexture;
            if (GroupsPage == 5)
            {
                AGXBtnStyle.normal.background = ButtonTextureRed;
            }
            else if (PageGrn[4])
            {
                AGXBtnStyle.normal.background = ButtonTextureGreen;
            }
            else
            {
                AGXBtnStyle.normal.background = ButtonTexture;
            }
            if (GUI.Button(new Rect(220, 3, 25, 20), "5", AGXBtnStyle))
            {
                GroupsPage = 5;
            }
            AGXBtnStyle.normal.background = ButtonTexture;
            ScrollGroups = GUI.BeginScrollView(new Rect(5, 25, 240, 500), ScrollPosSelParts, new Rect(0, 0, 240, 500));

            int ButtonID = new int();
            ButtonID = 1 + (50 * (GroupsPage -1));
             int ButtonPos = new int();
             ButtonPos = 1;
             TextAnchor TxtAnch3 = new TextAnchor();
             TxtAnch3 = GUI.skin.button.alignment;
             //GUI.skin.button.alignment = TextAnchor.MiddleLeft;
             AGXBtnStyle.alignment = TextAnchor.MiddleLeft;
             while (ButtonPos <= 25)
             {
                 if (ShowKeySetWin)
                 {
                     if (GUI.Button(new Rect(0, (ButtonPos - 1) * 20, 120, 20), ButtonID + " Key: " + AGXguiKeys[ButtonID].ToString(),AGXBtnStyle))
                     {
                         
                         AGXCurActGroup = ButtonID;
                         ShowKeyCodeWin = true;
                     }
                 }

                 else
                 {
                     //if (CurrentVesselActions.Any(pfd => pfd.group == ButtonID))
                     //{
                     //    GUI.DrawTexture(new Rect(1, ((ButtonPos - 1) * 20) + 1, 118, 18), BtnTexGrn);
                     //}

                     AGXBtnStyle.normal.background = CurrentVesselActions.Any(pfd => pfd.group == ButtonID) ? ButtonTextureGreen : ButtonTexture;
                     if (GUI.Button(new Rect(0, (ButtonPos - 1) * 20, 120, 20), ButtonID + ": " + AGXguiNames[ButtonID],AGXBtnStyle))
                     {
                         AGXCurActGroup = ButtonID;
                         TempShowGroupsWin = false;
                     }
                     AGXBtnStyle.normal.background = ButtonTexture;
                 }
                 ButtonPos = ButtonPos + 1;
                 ButtonID = ButtonID + 1;
             }
             while (ButtonPos <= 50)
             {
                 if (ShowKeySetWin)
                 {
                     if (GUI.Button(new Rect(120, (ButtonPos - 26) * 20, 120, 20), ButtonID + " Key: " + AGXguiKeys[ButtonID].ToString(),AGXBtnStyle))
                     {
                         AGXCurActGroup = ButtonID;
                         ShowKeyCodeWin = true;
                     }
                 }
                 else
                 {
                     //if (CurrentVesselActions.Any(pfd => pfd.group == ButtonID))
                     //{
                     //    GUI.DrawTexture(new Rect(121, ((ButtonPos - 26) * 20) + 1, 118, 18), BtnTexGrn);
                     //}
                     AGXBtnStyle.normal.background = CurrentVesselActions.Any(pfd => pfd.group == ButtonID) ? ButtonTextureGreen : ButtonTexture;
                     if (GUI.Button(new Rect(120, (ButtonPos - 26) * 20, 120, 20), ButtonID + ": " + AGXguiNames[ButtonID],AGXBtnStyle))
                     {


                         AGXCurActGroup = ButtonID;
                         TempShowGroupsWin = false;

                     }
                     AGXBtnStyle.normal.background = ButtonTexture;
                 }
                 ButtonPos = ButtonPos + 1;
                 ButtonID = ButtonID + 1;
             }
             //GUI.skin.button.alignment = TxtAnch3;
             AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
       
            GUI.EndScrollView();

            
            GUI.DragWindow();
        }


        public List<AGXPart> AGXAddSelectedPart(Part p, bool Sym)
        {
            List<Part> ToAdd = new List<Part>();
            List<AGXPart>RetLst = new List<AGXPart>();
            ToAdd.Add(p);
            if (Sym)
            {
                ToAdd.AddRange(p.symmetryCounterparts);
            }
            foreach(Part prt in ToAdd)
            {
                if(!AGEditorSelectedParts.Any(prt2 => prt2.AGPart == prt))
                {
                    RetLst.Add(new AGXPart(prt));
                }
            }
            AGEEditorSelectedPartsSame = true;
            foreach (AGXPart aprt in RetLst)
            {
                if (aprt.AGPart.partInfo.title != RetLst.First().AGPart.partInfo.title)
                {
                    AGEEditorSelectedPartsSame = false;
                }
            }
            if (AGEEditorSelectedPartsSame)
            {
                PartActionsList.Clear();
                PartActionsList.AddRange(p.Actions);
                foreach (ModuleAGExtData pm in p.Modules.OfType<ModuleAGExtData>())
                {
                    PartActionsList.AddRange(pm.partAllActions);
                }
 
            }
            return RetLst;

        }

        public void LoadGroupNames()
        {
            
            for (int i = 1; i <= 250; i = i + 1)
            {
                AGXguiNames[i] = "";
            }
           

            string LoadNames = "";
            
                foreach (ModuleAGExtData pm in EditorLogic.startPod.Modules.OfType<ModuleAGExtData>())
                {


                    LoadNames = pm.AGXNames;
                 
                   
                    if(LoadNames.Length > 0)
                    {
                        while (LoadNames[0] == '\u2023')
                        {
                         
                            int groupNum = new int();
                            string groupName = "";
                            LoadNames = LoadNames.Substring(1);
                            groupNum = Convert.ToInt32(LoadNames.Substring(0, 3));
                            LoadNames = LoadNames.Substring(3);

                            if (LoadNames.IndexOf('\u2023') == -1)
                            {
                              
                                groupName = LoadNames;
                            }
                            else
                            {
                              
                                groupName = LoadNames.Substring(0, LoadNames.IndexOf('\u2023'));
                                LoadNames = LoadNames.Substring(LoadNames.IndexOf('\u2023'));
                            }

                        
                            AGXguiNames[groupNum] = groupName;

                        }      
                        }
                    }
               
                
}


        public void LoadDefaultActionGroups()
        {
           
            List<KSPActionGroup> CustomActions = new List<KSPActionGroup>();
            CustomActions.Add(KSPActionGroup.Custom01); //how do you add a range from enum?
            CustomActions.Add(KSPActionGroup.Custom02);
            CustomActions.Add(KSPActionGroup.Custom03);
            CustomActions.Add(KSPActionGroup.Custom04);
            CustomActions.Add(KSPActionGroup.Custom05);
            CustomActions.Add(KSPActionGroup.Custom06);
            CustomActions.Add(KSPActionGroup.Custom07);
            CustomActions.Add(KSPActionGroup.Custom08);
            CustomActions.Add(KSPActionGroup.Custom09);
            CustomActions.Add(KSPActionGroup.Custom10);

            foreach (Part p in EditorLogic.SortedShipList)
            {
                string AddGroup = "";
                foreach (PartModule pm in p.Modules)
                {
                    foreach (BaseAction ba in pm.Actions)
                    {
                        foreach (KSPActionGroup agrp in CustomActions)
                        {
                          
                            if ((ba.actionGroup & agrp) == agrp)
                            
                            {
                               
                                AddGroup = AddGroup + '\u2023' + (CustomActions.IndexOf(agrp) +1).ToString("000") + ba.guiName;
                            }
                        }
                    }
                }
                foreach (PartModule pm in p.Modules.OfType<ModuleAGExtData>())
                {
                    pm.Fields.SetValue("AGXData", AddGroup);
               
                    

                }

            }




        }

        
        public void Update()
            
        {

           


            bool RootPartExists = new bool();
            try
            {
                if (EditorLogic.startPod != null)
                {
                }
                RootPartExists = true;
            }
            catch
            {
                RootPartExists = false;
            }
           
           
            if (RootPartExists)
            {
                
                if (AGXRoot != EditorLogic.startPod) //load keyset also
                {
                    LoadFinished = false;
                  
                    LoadCurrentKeySet();
                    LoadActionGroups();
                    LoadGroupNames();
                    LoadCurrentKeyBindings();
                    LoadGroupVisibility();

                    foreach (ModuleAGExtData agData in EditorLogic.startPod.Modules.OfType<ModuleAGExtData>())
                    {
                        ShowGroupInFlightNames = agData.LoadShowGroupNames();
                    }
                    LoadFinished = true;
                    



                    AGXRoot = EditorLogic.startPod;

                }
                if (NeedToLoadActions)
            {
                LoadActionGroups();
            }
            }
            
            
            EditorLogic ELCur = new EditorLogic();
            ELCur = EditorLogic.fetch;//get current editor logic instance

           
            
            if (AGXDoLock && ELCur.editorScreen != EditorLogic.EditorScreen.Actions)
            {
                ELCur.Unlock("AGXLock");
                AGXDoLock = false;
            }
            else if(AGXDoLock && !TrapMouse)
            {
                ELCur.Unlock("AGXLock");
                AGXDoLock = false;
            }
            else if (!AGXDoLock && TrapMouse && ELCur.editorScreen == EditorLogic.EditorScreen.Actions)
            {
                ELCur.Lock(false,false,false,"AGXLock");
                AGXDoLock = true;
            }
          
            
            if (ELCur.editorScreen == EditorLogic.EditorScreen.Actions) //only show mod if on actions editor screen
            {
              
                ShowSelectedWin = true;
            }
            else
            {
                
                ShowSelectedWin = false;
                
               
                  
                
                AGEditorSelectedParts.Clear();//mod is hidden, clear list so parts don't get orphaned in it
              
                PartActionsList.Clear();
              
                ActionsListDirty = true;
            }

          
            if (ShowSelectedWin) 
            {

               
                if (!LoadGroupsOnceCheck)
                {
                   
                    LoadActionGroups();
                    LoadGroupsOnceCheck = true;
                }

                if (EditorActionGroups.Instance.GetSelectedParts() != null) //on first run, list is null
                {

                    if (ActionsListDirty)
                    {
                        UpdateActionsListCheck();
                       
                    }
                    if (EditorActionGroups.Instance.GetSelectedParts().Count > 0) //are there parts selected?
                    {

                        
                        if (PreviousSelectedPart != EditorActionGroups.Instance.GetSelectedParts().First()) //has selected part changed?
                        {
                            
                            if(!AGEditorSelectedParts.Any(p => p.AGPart==EditorActionGroups.Instance.GetSelectedParts().First())) //make sure selected part is not already in AGEdSelParts
                            {

                                if (AGEditorSelectedParts.Count == 0) //no items in Selected Parts list, so just add selection
                                {
                                   AGEditorSelectedParts.AddRange(AGXAddSelectedPart(EditorActionGroups.Instance.GetSelectedParts().First(),SelPartsIncSym));
                                   

                                }
                                else if (AGEditorSelectedParts.First().AGPart.name == EditorActionGroups.Instance.GetSelectedParts().First().name) //selected part matches first part already in selected parts list, so just add selected part
                                {
                                    AGEditorSelectedParts.AddRange(AGXAddSelectedPart(EditorActionGroups.Instance.GetSelectedParts().First(),SelPartsIncSym));
                                   

                                }
                                else //part does not match first part in list, clear list before adding part
                                {
                                   
                                    AGEditorSelectedParts.Clear();
                                    AGEditorSelectedParts.AddRange(AGXAddSelectedPart(EditorActionGroups.Instance.GetSelectedParts().First(),SelPartsIncSym));
                                  


                                }
                            }
                            PreviousSelectedPart = EditorActionGroups.Instance.GetSelectedParts().First(); //remember selected part so logic does not run unitl another part selected
                        }
                        
                    }
                }
                
            }


           
       
            
            try
            {
                if(EditorLogic.SortedShipList.Count >= 1)
                {
                 
                    foreach (Part p in EditorLogic.SortedShipList)
                    {
                     
                    }
                    ShipListOk = true;
                }
             
            }
            catch
            {
            
                ShipListOk = false;
            }

            if (ShipListOk)
            {
                if (EditorLogic.SortedShipList.First<Part>() != null)
                {
               
                    ShipListOk = true;

                }
                else
                {
               
                    ShipListOk = false;
                }
            }





            if (ShipListOk)
            {
                foreach (Part p in EditorLogic.SortedShipList)
                {
                   

                    foreach (ModuleAGExtData agpm in p.Modules.OfType<ModuleAGExtData>())
                    {
                        if (agpm.partCurrentKeySet == 0)
                        {
                            agpm.partCurrentKeySet = 1;
                        }
                    }
                }
            }

            if (EditorLogic.fetch.editorScreen == EditorLogic.EditorScreen.Actions)
            {
                MonitorDefaultActions();
            }
            
            }

        public void MonitorDefaultActions()
        {
            
            if (EditorActionGroups.Instance.GetSelectedParts() != null) //is a part selected?
            {
              
                if (EditorActionGroups.Instance.GetSelectedParts().Count > 0) //list can exist with no parts in it if you selected then unselect one
                {
                   
                    if(SelectedWithSym.Count == 0 || SelectedWithSym.First() != EditorActionGroups.Instance.GetSelectedParts().First()) //check if there is a previously selected part, if so check if its changed
                    {
                       
                        //parts are different
                        SelectedWithSym.Clear(); //reset lastpart list
                        SelectedWithSym.AddRange(EditorActionGroups.Instance.GetSelectedParts());
                        SelectedWithSym.AddRange(EditorActionGroups.Instance.GetSelectedParts().First().symmetryCounterparts);
                        SelectedWithSymActions.Clear(); //reset actions list
                      
                        foreach (Part prt in SelectedWithSym)
                        {
                            
                            foreach (BaseAction bap in prt.Actions) //get part actions
                            {
                                SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = bap, agrp = bap.actionGroup }); //add actiongroup separate otherwise it links and so have nothing to compare
                            }
                           
                            foreach (PartModule pm in prt.Modules) //add actions from all partmodules
                            {
                                foreach (BaseAction bapm in pm.Actions)
                                {
                                    SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = bapm, agrp = bapm.actionGroup });
                                }
                            }
                        }
                        
                        
                    }
                    else //selected part is the same a previously selected part
                    {
                        
                        List<Part> PartsThisFrame = new List<Part>(); //get list of parts this update frame
                        PartsThisFrame.AddRange(EditorActionGroups.Instance.GetSelectedParts());
                        PartsThisFrame.AddRange(EditorActionGroups.Instance.GetSelectedParts().First().symmetryCounterparts);
                        List<BaseAction> ThisFrameActions = new List<BaseAction>(); //get actions fresh again this update frame
                        foreach (Part prt in PartsThisFrame)
                        {
                            
                            foreach (BaseAction bap in prt.Actions)
                            {
                                ThisFrameActions.Add(bap);
                            }
                            foreach (PartModule pm in prt.Modules)
                            {
                                foreach (BaseAction bapm in pm.Actions)
                                {
                                    ThisFrameActions.Add(bapm);
                                }
                            }
                        }
                       

                        foreach (BaseAction ba2 in ThisFrameActions) //check each action's actiongroup enum against last update frames actiongroup enum
                        {
                            
                            AGXDefaultCheck ActionLastFrame = new AGXDefaultCheck();
                            ActionLastFrame = SelectedWithSymActions.Find(a => a.ba == ba2);
                            if (ActionLastFrame.agrp != ba2.actionGroup) //actiongroup enum is different
                            {
                                int NewGroup = 0; //which actiongroup changed?
                                if (KSPActionGroup.Custom01 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                {
                                    NewGroup = 1;
                                }
                                else if (KSPActionGroup.Custom02 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                {
                                    NewGroup = 2;
                                }
                                else if (KSPActionGroup.Custom03 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                {
                                    NewGroup = 3;
                                }
                                else if (KSPActionGroup.Custom04 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                {
                                    NewGroup = 4;
                                }
                                else if (KSPActionGroup.Custom05 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                {
                                    NewGroup = 5;
                                }
                                else if (KSPActionGroup.Custom06 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                {
                                    NewGroup = 6;
                                }
                                else if (KSPActionGroup.Custom07 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                {
                                    NewGroup = 7;
                                }
                                else if (KSPActionGroup.Custom08 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                {
                                    NewGroup = 8;
                                }
                                else if (KSPActionGroup.Custom09 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                {
                                    NewGroup = 9;
                                }
                                else if (KSPActionGroup.Custom10 == (ActionLastFrame.agrp ^ ba2.actionGroup))
                                {
                                    NewGroup = 10;
                                }
                               

                                if (NewGroup != 0) //if one of the other actiongroups (gear, lights) has changed, ignore it. newgroup will be the actiongroup if I want to process it.
                                {
                                        AGXAction ToAdd = new AGXAction() { prt = ba2.listParent.part, ba = ba2, group = NewGroup, activated = false };
                                        List<AGXAction> Checking = new List<AGXAction>();
                                        Checking.AddRange(CurrentVesselActions);
                                        Checking.RemoveAll(p => p.group != ToAdd.group);
                                        Checking.RemoveAll(p => p.prt != ToAdd.prt);
                                        Checking.RemoveAll(p => p.ba != ToAdd.ba);

                                        if (Checking.Count == 0)
                                        {
                                            CurrentVesselActions.Add(ToAdd);
                                            SaveCurrentVesselActions();
                                        }
                                    
                                }
                                ActionLastFrame.agrp = KSPActionGroup.None;
                                ActionLastFrame.agrp = ActionLastFrame.agrp | ba2.actionGroup;
                                
                            }
                           
                        }
                        SelectedWithSymActions.Clear(); //reset actions list as one of the enums changed.
                       
                        foreach (Part prt in SelectedWithSym)
                        {
                            
                            foreach (BaseAction bap in prt.Actions) //get part actions
                            {
                                SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = bap, agrp = bap.actionGroup }); //add actiongroup separate otherwise it links and so have nothing to compare
                            }
                            foreach (PartModule pm in prt.Modules) //add actions from all partmodules
                            {
                                foreach (BaseAction bapm in pm.Actions)
                                {
                                    SelectedWithSymActions.Add(new AGXDefaultCheck() { ba = bapm, agrp = bapm.actionGroup });
                                }
                            }
                        }
                    }
                }
            }
        }

        public void LoadActionGroups()
        {
           
            if(CurrentVesselActions == null)
            {
               
                CurrentVesselActions = new List<AGXAction>();
            }
            else
            {
               
            CurrentVesselActions.Clear();
            }
           
            bool RootPartExists = new bool();
            try
            {
                if (EditorLogic.SortedShipList.Count >= 1)
                {
                }
                RootPartExists = true;
            }
            catch
            {
                RootPartExists = false;
            }

            
            if (RootPartExists)
            {
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    
                    foreach (ModuleAGExtData agpm in p.Modules.OfType<ModuleAGExtData>())
                    {
                        CurrentVesselActions.AddRange(agpm.partAGActions);
                    }
                }
            }
            NeedToLoadActions = false;
        }
           
           
        


        
        public static string SaveGroupNames(Part p, string str)
        {
            string errLine = "1";
            try
            {
                errLine = "2";
                if (p.missionID == EditorLogic.startPod.missionID)
                {
                    errLine = "3";
                    string SaveStringNames = "";
                    errLine = "4";

                    int GroupCnt = new int();
                    errLine = "5";
                    GroupCnt = 1;
                    errLine = "6";
                    while (GroupCnt <= 250)
                    {
                        errLine = "7";
                        if (AGXguiNames[GroupCnt].Length >= 1)
                        {
                            errLine = "8";
                            SaveStringNames = SaveStringNames + '\u2023' + GroupCnt.ToString("000") + AGXguiNames[GroupCnt];
                        }
                        errLine = "9";
                        GroupCnt = GroupCnt + 1;
                    }
                    errLine = "10";
                    return SaveStringNames;
                }
                else
                {
                    errLine = "11";
                    return str;
                }
            }
            catch (Exception e)
            {
                print("AGX Editor Fail (SaveGroupNames) " + errLine + " " + e);
                return str;
            }
        }

        public void UpdateActionsListCheck()  
        {
           List<AGXAction> KnownGood = new List<AGXAction>();
            KnownGood= new List<AGXAction>();

            foreach (AGXAction agxAct in CurrentVesselActions)
            {
                if (EditorLogic.SortedShipList.Contains(agxAct.prt))
                {
                    KnownGood.Add(agxAct);
                }
            }
            CurrentVesselActions = KnownGood;
          
            ActionsListDirty = false;
        }
                  
        public void AGXResetPartsList()
                        {
                            AGEEditorSelectedPartsSame = true;
                        AGEditorSelectedParts.Clear();
                        foreach (Part p in EditorLogic.SortedShipList)
                        {
                             AGEditorSelectedParts.Add(new AGXPart(p));
                        }
                        

                        AGXPart AGPcompare = new AGXPart();
                        AGPcompare = AGEditorSelectedParts.First();
                        
                        foreach (AGXPart p in AGEditorSelectedParts)
                        {
                            if (p.AGPart.ConstructID != AGPcompare.AGPart.ConstructID)
                            {
                                AGEEditorSelectedPartsSame = false;
                            }
                        }
        }

        public static string SaveGroupVisibilityNames(Part p, string str)
        {
            try
            {
                string StringToSave = ShowGroupInFlightNames[1];
                for (int i = 2; i <= 5; i++)
                {
                    StringToSave = StringToSave + '\u2023' + ShowGroupInFlightNames[i];
                }
                return StringToSave;
            }

            catch
            {
                return str;
            }
        }
        public static string SaveGroupVisibility(Part p, string str)
        {

            try
            {
                
                    string ReturnStr = ShowGroupInFlightCurrent.ToString(); //add currently show flightgroup

                    for (int i = 1; i <= 250; i++)
                    {
                        ReturnStr = ReturnStr + Convert.ToInt16(IsGroupToggle[i]).ToString(); //add toggle state for group
                        for (int i2 = 1; i2 <= 5; i2++)
                        {
                            ReturnStr = ReturnStr + Convert.ToInt16(ShowGroupInFlight[i2, i]).ToString(); //add flight state visibility for each group
                        }
                    }
                    return ReturnStr;
                
            }
            catch
            {
                return str;
            }
        }
    }
}
                    
                    
                   
