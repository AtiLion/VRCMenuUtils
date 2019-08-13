using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRCMenuUtils;

namespace VRChat.UI.QuickMenuUI
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
        public Button ButtonObject { get; private set; }
        public Text TextObject { get; private set; }
        public UiTooltip TooltipObject { get; private set; }
        #endregion

        #region Control Access Properties
        public event Action OnClick;
        public string Text
        {
            get => TextObject?.text;
            set
            {
                if (TextObject != null)
                    TextObject.text = value;
            }
        }
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
            ButtonObject = Control.GetComponent<Button>();
            TextObject = TextControl.GetComponent<Text>();
            TooltipObject = Control.GetComponent<UiTooltip>();

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
            TextObject.text = text;
            ButtonObject.onClick = new Button.ButtonClickedEvent();
            ButtonObject.onClick.AddListener(() => OnClick?.Invoke());
            TooltipObject.text = tooltip;

            // Finish
            Success = true;
        }
    }
}
