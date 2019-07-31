using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRCMenuUtils;

namespace VRChat.UI
{
    public class VRCEUiQuickButton
    {
        #region VRCUI Properties
        public bool Success { get; private set; }
        #endregion

        #region UI Properties
        public Transform Control { get; private set; }
        public Transform TextControl { get; private set; }

        public RectTransform Position { get; private set; }
        #endregion

        #region Control Properties
        public Button Button { get; private set; }
        public Text Text { get; private set; }
        public UiTooltip Tooltip { get; private set; }
        #endregion

        public VRCEUiQuickButton(string name, Vector2 position, string text, string tooltip, Transform parent = null)
        {
            // Get required information
            Transform orgControl = VRCEUi.InternalQuickMenu.ReportWorldButton;
            if (orgControl == null)
            {
                MVRCLogger.LogError("Could not find Report World button!");
                Success = false;
                return;
            }

            // Duplicate object
            GameObject goControl = GameObject.Instantiate(orgControl.gameObject);
            if (goControl == null)
            {
                MVRCLogger.LogError("Could not duplicate Report World button!");
                Success = false;
                return;
            }

            // Set UI properties
            Control = goControl.transform;
            TextControl = Control.GetComponentInChildren<Text>().transform;

            // Remove components that may cause issues
            GameObject.DestroyImmediate(Control.GetComponent<RectTransform>());
            GameObject.DestroyImmediate(Control.GetComponentsInChildren<Image>(true).First(a => a.transform != Control));

            // Set control properties
            Button = Control.GetComponent<Button>();
            Text = TextControl.GetComponent<Text>();
            Tooltip = Control.GetComponent<UiTooltip>();

            // Set required parts
            if (parent != null)
                Control.SetParent(parent);
            goControl.name = name;

            // Modify RectTransform
            Position = Control.GetComponent<RectTransform>();
            RectTransform tmpRT = orgControl.GetComponent<RectTransform>();

            Position.localScale = tmpRT.localScale;
            //Position.anchoredPosition = tmpRT.anchoredPosition;
            Position.sizeDelta = tmpRT.sizeDelta;
            Position.localPosition = new Vector3(position.x, position.y, 0f);
            Position.localRotation = tmpRT.localRotation;

            // Change UI properties
            Text.text = text;
            Button.onClick = new Button.ButtonClickedEvent();
            Tooltip.text = tooltip;

            // Finish
            Success = true;
        }
    }
}
