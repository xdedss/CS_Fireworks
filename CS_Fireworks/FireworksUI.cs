using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using System.IO;

namespace CS_Fireworks
{
    class FireworksUI
    {
        public static GameObject gameUI;

        public static FireworksUIButton button_toggle;
        public static UIPanel panel_main;
        //public static UIScrollablePanel scroll_prefab;
        //public static UIScrollablePanel scroll_preset;
        public static UILabel label_prefabname;
        public static UIButton btn_prev;
        public static UIButton btn_next;
        public static UILabel label_r;
        public static UISlider slider_r;
        public static UILabel label_g;
        public static UISlider slider_g;
        public static UILabel label_b;
        public static UISlider slider_b;
        public static UILabel panel_color;
        public static UILabel label_h_from;
        public static UISlider slider_h_from;
        public static UILabel label_h_to;
        public static UISlider slider_h_to;
        public static UILabel label_size;
        public static UISlider slider_size;
        public static UILabel label_startvel;
        public static UISlider slider_startvel;
        public static Vector2 panel_size = new Vector2(240, 210);

        public static UITextureAtlas[] atlasallarr;
        public static Dictionary<string, UITextureAtlas> atlasall;

        public static void Init()
        {
            gameUI = Singleton<UIView>.instance.gameObject;

            //StreamWriter templog = File.CreateText("C:\\my\\temp\\atlaslist.txt");//
            atlasallarr = Resources.FindObjectsOfTypeAll<UITextureAtlas>();
            atlasall = new Dictionary<string, UITextureAtlas>();
            foreach (UITextureAtlas atlas in atlasallarr)
            {
                if (atlas.name != "" && !atlasall.ContainsKey(atlas.name))
                    atlasall.Add(atlas.name, atlas);
                //templog.WriteLine(atlas.name);//
                //foreach (string nm in atlas.spriteNames)//
                //{//
                //    templog.WriteLine("--" + nm);//
                //}//
                //templog.WriteLine();//
                //FireworksManager.LogMsg("[atlas]" + atlas.name);//
            }
            //templog.Flush();//
            //templog.Close();//

            button_toggle = new GameObject("FireworksButton").AddComponent<FireworksUIButton>();
            button_toggle.transform.parent = gameUI.transform;

            panel_main = new GameObject("FireworksPanel").AddComponent<FireworksUIPanel>();
            panel_main.transform.parent = gameUI.transform;

            //scroll_prefab = panel_main.AddUIComponent<UIScrollablePanel>();
            //scroll_prefab.transform.parent = panel_main.transform;
            //scroll_prefab.atlas = GetAtlas("Ingame");
            //scroll_prefab.backgroundSprite = "GenericPanelWhite";
            //scroll_prefab.relativePosition = new Vector3(10, 10, 0);
            //scroll_prefab.size = new Vector2(75, 280);

            float y = 5;
            float div = 5;

            label_prefabname = panel_main.AddUIComponent<UILabel>();
            label_prefabname.autoSize = false;
            label_prefabname.text = "[fireworkname]";
            label_prefabname.textAlignment = UIHorizontalAlignment.Center;
            label_prefabname.verticalAlignment = UIVerticalAlignment.Middle;
            label_prefabname.textScale = 0.9f;
            label_prefabname.size = new Vector2(panel_size.x - 70, 20);
            label_prefabname.relativePosition = new Vector2(35, y + 5);

            btn_prev = panel_main.AddUIComponent<UIButton>();
            btn_prev.size = new Vector2(30, 30);
            btn_prev.relativePosition = new Vector2(5, y);
            btn_prev.atlas = GetAtlas("Ingame");
            btn_prev.normalFgSprite = "ArrowLeft";
            btn_prev.hoveredFgSprite = "ArrowLeftHovered";
            btn_prev.pressedFgSprite = "ArrowLeftPressed";
            btn_prev.eventClicked += new MouseEventHandler((c, p) =>
            {
                FireworksManager.PrefabPrev();
                FireworksManager.LogMsg("prev");
                RefreshPrefabName();
            });

            btn_next = panel_main.AddUIComponent<UIButton>();
            btn_next.size = new Vector2(30, 30);
            btn_next.relativePosition = new Vector2(panel_size.x - 35, y);
            btn_next.atlas = GetAtlas("Ingame");
            btn_next.normalFgSprite = "ArrowRight";
            btn_next.hoveredFgSprite = "ArrowRightHovered";
            btn_next.pressedFgSprite = "ArrowRightPressed";
            btn_next.eventClicked += new MouseEventHandler((c, p) =>
            {
                FireworksManager.PrefabNext();
                FireworksManager.LogMsg("next");
                RefreshPrefabName();
            });
            y += 30;
            y += div;

            label_r = panel_main.AddUIComponent<UILabel>();
            label_r.textScale = 0.8f;
            label_r.text = "R:" + FireworksManager.instance.colorR.value;
            label_r.size = new Vector2(panel_size.x - 10, 15);
            label_r.relativePosition = new Vector2(5, y);
            y += 15;

            slider_r = ColorSlider(panel_main, FireworksManager.instance.colorR.value);
            slider_r.size = new Vector2(panel_size.x - 10, 10);
            slider_r.relativePosition = new Vector2(5, y);
            slider_r.eventValueChanged += new PropertyChangedEventHandler<float>((c, v) =>
            {
                label_r.text = "R:" + v;
                FireworksManager.instance.colorR.value = v;
                RefreshColor();
            });
            y += 10;
            y += div;

            label_g = panel_main.AddUIComponent<UILabel>();
            label_g.textScale = 0.8f;
            label_g.text = "G:" + FireworksManager.instance.colorG.value;
            label_g.size = new Vector2(panel_size.x - 10, 15);
            label_g.relativePosition = new Vector2(5, y);
            y += 15;

            slider_g = ColorSlider(panel_main, FireworksManager.instance.colorG.value);
            slider_g.size = new Vector2(panel_size.x - 10, 10);
            slider_g.relativePosition = new Vector2(5, y);
            slider_g.eventValueChanged += new PropertyChangedEventHandler<float>((c, v) =>
            {
                label_g.text = "G:" + v;
                FireworksManager.instance.colorG.value = v;
                RefreshColor();
            });
            y += 10;
            y += div;

            label_b = panel_main.AddUIComponent<UILabel>();
            label_b.textScale = 0.8f;
            label_b.text = "B:" + FireworksManager.instance.colorB.value;
            label_b.size = new Vector2(panel_size.x - 10, 15);
            label_b.relativePosition = new Vector2(5, y);
            y += 15;

            slider_b = ColorSlider(panel_main, FireworksManager.instance.colorB.value);
            slider_b.size = new Vector2(panel_size.x - 10, 10);
            slider_b.relativePosition = new Vector2(5, y);
            slider_b.eventValueChanged += new PropertyChangedEventHandler<float>((c, v) =>
            {
                label_b.text = "B:" + v;
                FireworksManager.instance.colorB.value = v;
                RefreshColor();
            });
            y += 10;
            y += div;

            panel_color = panel_main.AddUIComponent<UILabel>();
            panel_color.atlas = GetAtlas("Ingame");
            panel_color.backgroundSprite = "GenericPanelWhite";
            panel_color.relativePosition = new Vector2(5, y);
            panel_color.size = new Vector2(panel_size.x - 10, 30);
            y += 30;
            y += div;

            label_h_from = panel_main.AddUIComponent<UILabel>();
            label_h_from.textScale = 0.8f;
            label_h_from.text = "Random Height(from):" + FireworksManager.instance.heightFrom.value;
            label_h_from.size = new Vector2(panel_size.x - 10, 15);
            label_h_from.relativePosition = new Vector2(5, y);
            y += 15;

            slider_h_from = CommonSlider(panel_main, 10, 300, 1, FireworksManager.instance.heightFrom.value);
            slider_h_from.size = new Vector2(panel_size.x - 10, 10);
            slider_h_from.relativePosition = new Vector2(5, y);
            slider_h_from.eventValueChanged += new PropertyChangedEventHandler<float>((c, v) =>
            {
                label_h_from.text = "Random Height(from):" + v;
                FireworksManager.instance.heightFrom.value = v;
                FireworksManager.defaultstyle.height_from = v;
            });
            y += 10;
            y += div;

            label_h_to = panel_main.AddUIComponent<UILabel>();
            label_h_to.textScale = 0.8f;
            label_h_to.text = "Random Height(to):" + FireworksManager.instance.heightTo.value;
            label_h_to.size = new Vector2(panel_size.x - 10, 15);
            label_h_to.relativePosition = new Vector2(5, y);
            y += 15;

            slider_h_to = CommonSlider(panel_main, 10, 300, 1, FireworksManager.instance.heightTo.value);
            slider_h_to.size = new Vector2(panel_size.x - 10, 10);
            slider_h_to.relativePosition = new Vector2(5, y);
            slider_h_to.eventValueChanged += new PropertyChangedEventHandler<float>((c, v) =>
            {
                label_h_to.text = "Random Height(to):" + v;
                FireworksManager.instance.heightTo.value = v;
                FireworksManager.defaultstyle.height_to = v;
            });
            y += 10;
            y += div;


            label_size = panel_main.AddUIComponent<UILabel>();
            label_size.textScale = 0.8f;
            label_size.text = "Particle Size:" + Mathf.Pow(10, FireworksManager.instance.mulSize.value).ToString("G2");
            label_size.size = new Vector2(panel_size.x - 10, 15);
            label_size.relativePosition = new Vector2(5, y);
            y += 15;

            slider_size = CommonSlider(panel_main, -0.5f, 0.7f, 0.01f, FireworksManager.instance.mulSize.value);
            slider_size.size = new Vector2(panel_size.x - 10, 10);
            slider_size.relativePosition = new Vector2(5, y);
            slider_size.eventValueChanged += new PropertyChangedEventHandler<float>((c, v) =>
            {
                float realsize = Mathf.Pow(10, v);
                label_size.text = "Particle Size:" + realsize.ToString("G2");
                FireworksManager.instance.mulSize.value = v;
                FireworksManager.defaultstyle.mul_particle_size = realsize;
            });
            y += 10;
            y += div;

            label_startvel = panel_main.AddUIComponent<UILabel>();
            label_startvel.textScale = 0.8f;
            label_startvel.text = "Exploding Speed:" + Mathf.Pow(10, FireworksManager.instance.mulExpVel.value).ToString("G2");
            label_startvel.size = new Vector2(panel_size.x - 10, 15);
            label_startvel.relativePosition = new Vector2(5, y);
            y += 15;

            slider_startvel = CommonSlider(panel_main, -0.5f, 1, 0.01f, FireworksManager.instance.mulExpVel.value);
            slider_startvel.size = new Vector2(panel_size.x - 10, 10);
            slider_startvel.relativePosition = new Vector2(5, y);
            slider_startvel.eventValueChanged += new PropertyChangedEventHandler<float>((c, v) =>
            {
                float realvel = Mathf.Pow(10, v);
                label_startvel.text = "Exploding Speed:" + realvel.ToString("G2");
                FireworksManager.instance.mulExpVel.value = v;
                FireworksManager.defaultstyle.mul_exp_vel = realvel;
            });
            y += 10;
            y += div;

            UILabel label_tip = panel_main.AddUIComponent<UILabel>();
            label_tip.autoSize = false;
            label_tip.relativePosition = new Vector2(5, y);
            label_tip.size = new Vector2(panel_size.x - 10, 50);
            label_tip.textScale = 0.8f;
            label_tip.text = "Select type and color\nClick anywhere to launch";
            label_tip.textAlignment = UIHorizontalAlignment.Center;
            y += 50;

            panel_size.y = y;
            panel_main.size = panel_size;
        }

        public static UISlider CommonSlider(UIComponent parent, float min, float max, float step, float value)
        {
            UISlider slider = parent.AddUIComponent<UISlider>();
            slider.atlas = GetAtlas("Ingame");
            slider.backgroundSprite = "BudgetSlider";
            slider.orientation = UIOrientation.Horizontal;
            slider.minValue = min;
            slider.maxValue = max;
            slider.stepSize = step;
            slider.value = value;
            UISlicedSprite slider_s = slider.AddUIComponent<UISlicedSprite>();
            slider.thumbObject = slider_s;
            slider_s.atlas = GetAtlas("Ingame");
            slider_s.spriteName = "SliderBudget";
            slider_s.size = new Vector2(15, 15);
            return slider;
        }

        public static UISlider ColorSlider(UIComponent parent, float value)
        {
            return CommonSlider(parent, 0, 255, 1, value);
        }

        public static void RefreshColor()
        {
            Color color = new Color(slider_r.value / 255f, slider_g.value / 255f, slider_b.value / 255f);
            FireworksManager.defaultstyle.color_from = color;// TODO separate colors
            FireworksManager.defaultstyle.color_to = color;
            panel_color.color = color;
        }

        public static void RefreshPrefabName()
        {
            label_prefabname.text = FireworksManager.PrefabCurrent().name;
        }

        public static UITextureAtlas GetAtlas(string name)
        {
            UITextureAtlas res;
            if(atlasall.TryGetValue(name, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }
    }
}
