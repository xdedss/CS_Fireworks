using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace CS_Fireworks
{
    class FireworksUIPanel : UIPanel
    {
        bool mouse_down = false;
        bool dragging = false;
        Vector2 start_pos;
        Vector2 m_down_pos;
        
        public override void Start()
        {
            base.Start();

            atlas = FireworksUI.GetAtlas("Ingame");
            //size = FireworksUI.panel_size;
            backgroundSprite = "GenericPanel";
            position = new Vector3(FireworksManager.instance.panelX.value, FireworksManager.instance.panelY.value, 0);
            gameObject.SetActive(false);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            FireworksManager.instance.panelX.value = position.x;
            FireworksManager.instance.panelY.value = position.y;
        }

        protected override void OnMouseDown(UIMouseEventParameter p)
        {
            m_down_pos = p.position;
            mouse_down = true;
        }

        protected override void OnMouseMove(UIMouseEventParameter p)
        {
            Vector2 delta = p.position - m_down_pos;
            if (delta.magnitude > 10 && mouse_down && !dragging)
            {
                dragging = true;
                start_pos = position;
            }
            if (dragging)
            {
                Vector3 newposition = start_pos + delta;
                newposition.x = Mathf.Clamp(newposition.x, -Screen.width/2, Screen.width/2 - size.x);
                newposition.y = Mathf.Clamp(newposition.y, -Screen.height/2 + size.y, Screen.height);
                position = newposition;
            }
        }

        protected override void OnMouseUp(UIMouseEventParameter p)
        {
            if (dragging)
            {
                FireworksManager.instance.panelX.value = position.x;
                FireworksManager.instance.panelY.value = position.y;
            }
            mouse_down = false;
            dragging = false;
        }
    }
}
