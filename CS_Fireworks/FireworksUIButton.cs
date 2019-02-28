using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace CS_Fireworks
{
    class FireworksUIButton : UIButton
    {
        bool mouse_down = false;
        bool dragging = false;
        Vector2 start_pos;
        Vector2 m_down_pos;

        public override void Start()
        {
            base.Start();

            atlas = FireworksUI.GetAtlas("Ingame");
            size = new Vector2(35f, 35f);
            textScale = 0.8f;
            text = "F";
            normalBgSprite = "InfoIconBaseNormal";
            hoveredBgSprite = "InfoIconBaseHovered";
            pressedBgSprite = "InfoIconBasePressed";
            canFocus = false;
            position = new Vector3(FireworksManager.instance.btnX.value, FireworksManager.instance.btnY.value);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            FireworksManager.instance.btnX.value = position.x;
            FireworksManager.instance.btnY.value = position.y;
        }

        protected override void OnClick(UIMouseEventParameter p)
        {

        }

        protected override void OnMouseDown(UIMouseEventParameter p)
        {
            m_down_pos = p.position;
            mouse_down = true;
        }

        protected override void OnMouseMove(UIMouseEventParameter p)
        {
            Vector2 delta = p.position - m_down_pos;
            if(delta.magnitude > 10 && mouse_down && !dragging)
            {
                dragging = true;
                start_pos = position;
            }
            if (dragging)
            {
                Vector3 newposition = start_pos + delta;
                newposition.x = Mathf.Clamp(newposition.x, -Screen.width / 2, Screen.width / 2 - size.x);
                newposition.y = Mathf.Clamp(newposition.y, -Screen.height / 2 + size.y, Screen.height);
                position = newposition;
            }
        }

        protected override void OnMouseUp(UIMouseEventParameter p)
        {
            if (!dragging)
            {
                FireworksUI.RefreshPrefabName();
                FireworksUI.RefreshColor();
                FireworksUI.panel_main.gameObject.SetActive(!FireworksUI.panel_main.gameObject.activeInHierarchy);
            }
            else
            {
                FireworksManager.instance.btnX.value = position.x;
                FireworksManager.instance.btnY.value = position.y;
            }
            mouse_down = false;
            dragging = false;
        }

        //protected override void OnDragStart(UIDragEventParameter p)
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        FireworksManager.LogMsg("[OnDragStart]" + p.position);
        //    }
        //}

        //protected override void OnDragEnd(UIDragEventParameter p)
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        FireworksManager.LogMsg("[OnDragEnd]" + p.position);
        //        position = p.position;
        //    }
        //}

        //protected override void OnDragDrop(UIDragEventParameter p)
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        FireworksManager.LogMsg("[OnDragDrop]" + p.position);
        //    }
        //}

        //protected override void OnDragLeave(UIDragEventParameter p)
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        FireworksManager.LogMsg("[OnDragLeave]" + p.position);
        //    }
        //}

        //protected override void OnDragEnter(UIDragEventParameter p)
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        FireworksManager.LogMsg("[OnDragEnter]" + p.position);
        //    }
        //}

        //protected override void OnDragOver(UIDragEventParameter p)
        //{
        //    if(Input.GetKey(KeyCode.LeftShift))
        //    {
        //        FireworksManager.LogMsg("[OnDragOver]" + p.position);
        //    }
        //}
    }
}
