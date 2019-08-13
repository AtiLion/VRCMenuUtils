using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRCMenuUtils;

namespace VRChat.UI
{
    public class VRCEUiButton
    {
        #region VRCUI Properties
        public bool Success { get; private set; }
        #endregion

        #region UI Properties
        public Transform Control { get; private set; }
        public Transform ButtonControl { get; private set; }
        public Transform ImageControl { get; private set; }
        public Transform TextControl { get; private set; }

        public RectTransform Position { get; private set; }
        #endregion

        #region Control Properties
        public Button ButtonObject { get; private set; }
        public Text TextObject { get; private set; }
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

        public VRCEUiButton(string name, Vector2 position, string text, Transform parent = null)
        {
            // Get required information
            Transform orgControl = VRCEUi.InternalUserInfoScreen.FavoriteButton;
            if(orgControl == null)
            {
                MVRCLogger.LogError("Could not find Favorite button!");
                Success = false;
                return;
            }

            // Duplicate object
            GameObject goControl = GameObject.Instantiate(orgControl.gameObject);
            if(goControl == null)
            {
                MVRCLogger.LogError("Could not duplicate Favorite button!");
                Success = false;
                return;
            }

            // Set UI properties
            Control = goControl.transform;
            ButtonControl = Control.Find("FavoriteButton");
            ImageControl = ButtonControl.Find("Image");
            TextControl = Control.GetComponentInChildren<Text>().transform;

            // Remove components that may cause issues
            GameObject.DestroyImmediate(Control.GetComponent<RectTransform>());
            GameObject.DestroyImmediate(ButtonControl.GetComponent<VRCUiButton>());

            // Set control properties
            ButtonObject = ButtonControl.GetComponent<Button>();
            TextObject = TextControl.GetComponent<Text>();

            // Set required parts
            if (parent != null)
                Control.SetParent(parent);
            goControl.name = name;
            ButtonControl.name = name + "Button";

            // Modify RectTransform
            Position = Control.GetComponent<RectTransform>();
            RectTransform tmpRT = orgControl.GetComponent<RectTransform>();

            Position.localScale = tmpRT.localScale;
            //Position.anchoredPosition = tmpRT.anchoredPosition;
            Position.sizeDelta = tmpRT.sizeDelta;
            Position.localPosition = new Vector3(position.x, position.y, 0f);
            Position.localRotation = tmpRT.localRotation;

            // Change UI properties
            Text = text;
            ButtonObject.onClick = new Button.ButtonClickedEvent();
            ButtonObject.onClick.AddListener(() => OnClick?.Invoke());

            // Finish
            Success = true;
        }
    }
}
