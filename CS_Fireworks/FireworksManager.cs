//using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using System;

namespace CS_Fireworks
{

    class FireworksManager : MonoBehaviour
    {
        public static FireworksManager instance;

        static List<GameObject> prefabs = new List<GameObject>();
        static List<FireworkStyle> presets = new List<FireworkStyle>();
        static int prefabs_index = 0;

        public static FireworkStyle defaultstyle;

        static bool initialized = false;

        static string path = GetPath() + "/Fireworks/";
        static string fireworkab = "fireworkparticle.unity3d";

        TerrainManager terrainmanager;

        Vector3 mousehit;

        public bool paused;

        public SavedFloat btnX;
        public SavedFloat btnY;
        public SavedFloat panelX;
        public SavedFloat panelY;
        public SavedInputKey hotkey;
        public SavedFloat colorR;
        public SavedFloat colorG;
        public SavedFloat colorB;
        public SavedFloat heightFrom;
        public SavedFloat heightTo;
        public SavedFloat mulExpVel;
        public SavedFloat mulSize;

        private void Start()
        {
            try
            {
                if (instance == null)
                {
                    instance = this;
                }
                else
                {
                    Destroy(gameObject);
                    return;
                }
                terrainmanager = Singleton<TerrainManager>.instance;

                defaultstyle = new FireworkStyle();

                LoadSaved();
                if (!initialized)
                {
                    initialized = true;
                    FireworksUI.Init();
                    ExtractResources();
                    StartCoroutine(LoadResources());
                }
            }
            catch(System.Exception ex)
            {
                LogErr("[startERR]" + ex.ToString() + "  -|-  " + ex.StackTrace);
            }
        }

        private void Update()
        {
            try
            {
                paused = SimulationManager.instance == null ? true : SimulationManager.instance.SimulationPaused;

                //if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftShift))
                //{
                //    LogMsg(FireworksUI.gameUI.GetComponent<UIInput>().isActiveAndEnabled.ToString());
                //    //if (UIInput.hoveredComponent.GetType().Equals(typeof(UIButton)) || Input.GetKey(KeyCode.RightShift))
                //    //{
                //    //    UIButton btn = (UIButton)UIInput.hoveredComponent;
                //    //    LogMsg(btn.normalBgSprite);
                //    //    LogMsg(btn.hoveredBgSprite);
                //    //    LogMsg(btn.pressedBgSprite);
                //    //}
                //    //if (UIInput.hoveredComponent.GetType().Equals(typeof(UIPanel)) || Input.GetKey(KeyCode.RightShift))
                //    //{
                //    //    UIPanel panel = (UIPanel)UIInput.hoveredComponent;
                //    //    LogMsg(panel.backgroundSprite);
                //    //}
                //}

                if (UIInput.hoveredComponent == null && FireworksUI.panel_main.gameObject.activeInHierarchy)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (Cast(Camera.main.ScreenPointToRay(Input.mousePosition), out mousehit))
                        {
                            LogMsg(mousehit.ToString());
                            CreateFirework(defaultstyle, FireworkControlMode.Once, mousehit, prefabs[prefabs_index]);
                        }
                    }
                    //if (Input.GetMouseButtonDown(1))
                    //{
                    //    if (Cast(Camera.main.ScreenPointToRay(Input.mousePosition), out mousehit))
                    //    {
                    //        LogMsg(mousehit.ToString());
                    //        CreateFirework(defaultstyle, mousehit, prefabs[1]);
                    //    }
                    //}
                }
            }
            catch (System.Exception ex)
            {
                if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift))
                    LogErr("[updateERR]" + ex.ToString() + "  -|-  " + ex.StackTrace);
            }
        }

        private void OnGUI()
        {
            //GUI.Label(new Rect(0, 0, 500, 100), "FireworksManager exists(" + btnX.value + "," + btnY.value + ")" + (hotkey.IsKeyUp()).ToString());
        }

        void LoadSaved()
        {
            try
            {
                SettingsFile settingsfile = new SettingsFile();
                settingsfile.fileName = FireworksMod.settingsfilename;
                GameSettings.AddSettingsFile(settingsfile);
                //FireworksUI.Init();
                LogMsg("file created");
            }
            catch (Exception ex)
            {
                LogErr(ex.ToString() + ex.StackTrace);
            }
            btnX = new SavedFloat("btnx", FireworksMod.settingsfilename, 100);
            btnY = new SavedFloat("btny", FireworksMod.settingsfilename, 100);
            panelX = new SavedFloat("panelx", FireworksMod.settingsfilename, 200);
            panelY = new SavedFloat("panely", FireworksMod.settingsfilename, 100);
            colorR = new SavedFloat("colorr", FireworksMod.settingsfilename, 255);
            colorG = new SavedFloat("colorg", FireworksMod.settingsfilename, 255);
            colorB = new SavedFloat("colorb", FireworksMod.settingsfilename, 255);
            heightFrom = new SavedFloat("heightfrom", FireworksMod.settingsfilename, 50);
            heightTo = new SavedFloat("heightto", FireworksMod.settingsfilename, 60);
            mulExpVel = new SavedFloat("mulexpvel", FireworksMod.settingsfilename, 0);
            mulSize = new SavedFloat("mulsize", FireworksMod.settingsfilename, 0);
            defaultstyle.height_from = heightFrom.value;
            defaultstyle.height_to = heightTo.value;
            defaultstyle.mul_exp_vel = Mathf.Pow(10, mulExpVel.value);
            defaultstyle.mul_particle_size = Mathf.Pow(10, mulSize.value);
            hotkey = new SavedInputKey("key", FireworksMod.settingsfilename, SavedInputKey.Encode(KeyCode.F, true, true, false));
        }

        FireworkControl CreateFirework(FireworkStyle style, FireworkControlMode mode, Vector3 startpoint, GameObject prefab)
        {
            if(prefab == null)
            {
                LogErr("prefab not loaded");
                return null;
            }
            GameObject newparticle = Instantiate(prefab);
            newparticle.transform.position = startpoint;

            ParticleSystem mainparticle = newparticle.GetComponent<ParticleSystem>();

            ParticleSystem.MainModule mainm = mainparticle.main;
            mainm.startSpeed = new ParticleSystem.MinMaxCurve(style.vmin, style.vmax);
            mainm.startColor = new ParticleSystem.MinMaxGradient(style.color_from, style.color_to);
            mainm.startLifetime = new ParticleSystem.MinMaxCurve(style.vmin / 9.81f, style.vmax / 9.81f);

            ParticleSystem.ShapeModule shapem = mainparticle.shape;
            shapem.angle = style.angle;

            ParticleSystem.SubEmittersModule subm = mainparticle.subEmitters;
            //if (subm.subEmittersCount != 0)
            //{
            //    ParticleSystem subparticle = subm.GetSubEmitterSystem(0);
            //    ParticleSystem.MainModule submainm = subparticle.main;
            //    submainm.startSpeed = new ParticleSystem.MinMaxCurve(style.exp_vel_from, style.exp_vel_to);
            //}

            FireworkControl.MulSize(mainparticle, style.mul_particle_size);
            FireworkControl.MulExpVel(mainparticle, style.mul_exp_vel, false, true);

            FireworkControl control = newparticle.AddComponent<FireworkControl>();
            control.particle = mainparticle;
            control.mode = mode;

            return control;

            //mainparticle.Play();
            //Destroy(newparticle, 100);
        }

        void ExtractResources()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if(!File.Exists(path + fireworkab))
            {
                byte[] abbytes = Properties.Resources.fireworkparticle;
                FileStream fs = File.Create(path + fireworkab);
                fs.Write(abbytes, 0, abbytes.Length);
                fs.Flush();
                fs.Close();
            }
        }

        IEnumerator LoadResources()
        {
            //string path = Application.dataPath + "/AssetsBundles/fireworkparticle.unity3d";
            WWW www = new WWW("file:///" + path + fireworkab);
            yield return www;
            try
            {
                if (www == null)
                {
                    LogErr("WWW is null");
                }
                else if (www.assetBundle == null)
                {
                    LogErr("AssetBundle is null");
                }
                else
                {
                    GameObject prefab_standard = www.assetBundle.LoadAsset<GameObject>("assets/prefabs/standardfirework.prefab");
                    GameObject prefab_big = www.assetBundle.LoadAsset<GameObject>("assets/prefabs/bigfirework.prefab");
                    GameObject prefab_met = www.assetBundle.LoadAsset<GameObject>("assets/prefabs/meteoroid.prefab");
                    //GameObject prefab_button = www.assetBundle.LoadAsset<GameObject>("assets/prefabs/fireworksbutton.prefab");
                    if (prefab_standard == null)
                    {
                        LogErr("firework_s prefab is null");
                    }
                    else if (prefab_big == null)
                    {
                        LogErr("firework_b prefab is null");
                    }
                    else if (prefab_met == null)
                    {
                        LogErr("firework_m prefab is null");
                    }
                    //else if (prefab_button == null)
                    //{
                    //    LogErr("fireworksbutton prefab is null");
                    //}
                    else
                    {
                        //button = Instantiate(prefab_button);
                        ////button.transform.parent = GameObject.Find("Canvas").transform;
                        //button.GetComponent<RectTransform>().position = new Vector2(200, 200);
                        //button.AddComponent<FireworksButton>();
                        prefabs.Add(prefab_standard);
                        prefabs.Add(prefab_big);
                        prefabs.Add(prefab_met);
                        LogMsg("firework loaded");//OK
                    }
                }
                File.Delete(path + fireworkab);
            }
            catch (System.Exception ex)
            {
                if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift))
                    LogErr("[CoroutineERR1]" + ex.ToString() + "  -|-  " + ex.StackTrace);
            }

            string[] customab = Directory.GetFiles(path, "*.unity3d");
            foreach (string ab in customab)
            {
                //if (ab.Substring(ab.LastIndexOf('.')) == "unity3d")
                //{
                WWW cwww = new WWW("file:///" + ab);
                yield return cwww;
                try
                {
                    if (cwww == null)
                    {
                        LogErr("can not load " + ab);
                    }
                    else if (cwww.assetBundle == null)
                    {
                        LogErr(ab + " does not have a asset bundle inside");
                    }
                    else
                    {
                        GameObject[] abcontent = www.assetBundle.LoadAllAssets<GameObject>();
                        foreach (GameObject obj in abcontent)
                        {
                            if (obj.GetComponent<ParticleSystem>() == null)
                            {
                                LogErr(obj.name + " in " + ab + "  does not have a particle system");
                            }
                            else
                            {
                                prefabs.Add(obj);
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift))
                        LogErr("[CoroutineERR2]" + ex.ToString() + "  -|-  " + ex.StackTrace);
                }
                //}
            }

            LogMsg("Coroutine OK " + prefabs.Count);
        }

        private static string GetPath()
        {
            string str = System.Environment.CurrentDirectory;
            return str;
        }

        public static void PrefabNext()
        {
            prefabs_index++;
            if (prefabs_index >= prefabs.Count)
            {
                prefabs_index = 0;
            }
        }

        public static void PrefabPrev()
        {
            prefabs_index--;
            if (prefabs_index < 0)
            {
                prefabs_index = prefabs.Count - 1;
            }
        }

        public static GameObject PrefabCurrent()
        {
            LogMsg("current index: " + prefabs_index);
            return prefabs[prefabs_index];
        }

        bool Cast(Ray ray, out Vector3 hitp)
        {
            Vector3 hit;
            if (terrainmanager.RayCast(new Segment3(ray.origin, ray.origin + ray.direction * 2000), out hit))
            {
                hitp = hit;
                return true;
            }
            else
            {
                hitp = Vector3.zero;
                return false;
            }
        }

        public static void LogErr(string msg)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Error, "[FireworksMod]" + msg);
            DebugOutputPanel.Show();
        }
        public static void LogMsg(string msg)
        {
            //DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, "[FireworksMod]" + msg);
            //DebugOutputPanel.Show();
        }
        public static void WriteLog(string msg)
        {
            string txt = path + "FireworksLog.txt";
            StreamWriter sw;
            if (File.Exists(txt))
            {
                sw = File.AppendText(txt);
            }
            else
            {
                sw = File.CreateText(txt);
            }
            sw.WriteLine(msg);
            sw.Flush();
            sw.Close();
        }
    }

    public class FireworkStyle
    {
        public string name;

        public Color color_from;
        public Color color_to;
        public float height_from;
        public float height_to;
        public float angle;
        public float exp_vel_from;
        public float exp_vel_to;
        public float mul_particle_size;
        public float mul_exp_vel;

        public FireworkStyle()
        {
            //color_from = new Color(1.0f, 1.0f, 0.2f);
            //color_to = new Color(0.2f, 1.0f, 0.5f);
            color_from = Color.white;
            color_to = Color.white;
            height_from = 50;
            height_to = 60;
            angle = 10;
            //exp_vel_from = 8;
            //exp_vel_to = 9;
            mul_particle_size = 1;
            mul_exp_vel = 1;
        }

        public float vmin { get { return Mathf.Sqrt(2 * 9.81f * height_from); } }
        public float vmax { get { return Mathf.Sqrt(2 * 9.81f * height_to); } }

    }

}
