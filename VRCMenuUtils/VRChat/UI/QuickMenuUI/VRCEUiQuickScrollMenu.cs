using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRCMenuUtils;

namespace VRChat.UI.QuickMenuUI
{
    public class VRCEUiQuickScrollMenu
    {
        #region VRCUI Properties
        public bool Success { get; private set; }
        #endregion

        #region UI Properties
        public Transform Control { get; private set; }
        public Transform ContentControl { get; private set; }
        public Transform UpButtonControl { get; private set; }
        public Transform DownButtonControl { get; private set; }

        public List<VRCEUiScrollPage> Pages { get; private set; } = new List<VRCEUiScrollPage>();
        public int CurrentPage { get; private set; } = 0;
        public bool HasButtons => Pages.Count > 0 && !Pages[0].IsEmpty;

        public RectTransform ContentPosition { get; private set; }
        #endregion

        #region Control Properties
        public Button UpButtonObject { get; private set; }
        public Button DownButtonObject { get; private set; }
        #endregion

        public VRCEUiQuickScrollMenu(string name, bool addBack = true)
        {
            // Get required information
            Transform orgControl = VRCEUi.InternalQuickMenu.EmojiMenu;
            if (orgControl == null)
            {
                MVRCLogger.LogError("Could not find Emoji Menu!");
                Success = false;
                return;
            }

            // Duplicate object
            GameObject goControl = GameObject.Instantiate(orgControl.gameObject, VRCEUi.QuickMenu.transform);
            if (goControl == null)
            {
                MVRCLogger.LogError("Could not duplicate Emoji Menu!");
                Success = false;
                return;
            }

            // Create game object
            GameObject goContent = new GameObject("Content");

            // Get positions
            ContentPosition = goContent.GetOrAddComponent<RectTransform>();

            // Set UI properties
            Control = goControl.transform;
            ContentControl = goContent.transform;

            // Get buttons
            UpButtonObject = Control.Find("PageUp").GetComponent<Button>();
            DownButtonObject = Control.Find("PageDown").GetComponent<Button>();

            // Setup UpButton
            UpButtonObject.onClick = new Button.ButtonClickedEvent();
            UpButtonObject.interactable = false;
            UpButtonObject.onClick.AddListener(() =>
            {
                if (CurrentPage < 1)
                    return;

                SetPage(CurrentPage - 1);
            });

            // Setup DownButton
            DownButtonObject.onClick = new Button.ButtonClickedEvent();
            DownButtonObject.interactable = false;
            DownButtonObject.onClick.AddListener(() =>
            {
                if (CurrentPage >= (Pages.Count - 1))
                    return;

                SetPage(CurrentPage + 1);
            });

            // Set required parts
            goControl.name = name;

            // Setup Content
            RectTransform rtBtn = UpButtonObject.transform.GetComponent<RectTransform>();
            ContentControl.SetParent(Control);
            ContentControl.localScale = Vector3.one;
            ContentControl.localRotation = Quaternion.identity;
            ContentControl.localPosition = Vector3.zero;
            ContentPosition.anchorMin = new Vector2(0f, 0f);
            ContentPosition.anchorMax = new Vector2(0f, 0f);
            ContentPosition.pivot = new Vector2(0f, 1f);
            ContentPosition.sizeDelta = new Vector2(rtBtn.sizeDelta.x * 3f, (rtBtn.sizeDelta.y - 10f) * 3f);
            ContentPosition.localPosition = new Vector3(-((ContentPosition.sizeDelta.x / 2f) + (rtBtn.sizeDelta.x / 2f)), ContentPosition.sizeDelta.y, 0f);

            // Clear menu
            foreach (Transform button in ContentControl)
            {
                if (button == null)
                    continue;
                GameObject.Destroy(button.gameObject);
            }
            foreach (Transform button in Control)
            {
                if (button == null)
                    continue;
                if (button.name == "Content")
                    continue;
                if (button.name == "BackButton" && addBack)
                    continue;
                if (button.name == "PageUp")
                    continue;
                if (button.name == "PageDown")
                {
                    if(!addBack)
                        button.GetComponent<RectTransform>().localPosition = orgControl.transform.Find("BackButton").GetComponent<RectTransform>().localPosition;
                    continue;
                }
                GameObject.Destroy(button.gameObject);
            }

            // Finish
            VRCEUiScrollPage page = new VRCEUiScrollPage(this);
            page.SetActive(true);
            Pages.Add(page);
            Success = true;
        }

        #region UI Functions
        public bool AddButton(VRCEUiQuickButton button)
        {
            VRCEUiScrollPage page = Pages[Pages.Count - 1];
            if (!page.CanAddItems)
            {
                page = new VRCEUiScrollPage(this);

                DownButtonObject.interactable = true;
                Pages.Add(page);
            }

            return page.AddButton(button);
        }
        public bool HasButton(VRCEUiQuickButton button) =>
            Pages.Any(a => a.HasButton(button));
        public bool SetPage(int id)
        {
            if (id > Pages.Count)
                return false;

            Pages[CurrentPage].SetActive(false);
            Pages[id].SetActive(true);
            CurrentPage = id;

            if (CurrentPage > 0)
                UpButtonObject.interactable = true;
            else
                UpButtonObject.interactable = false;
            if ((Pages.Count - 1) <= CurrentPage)
                DownButtonObject.interactable = false;
            else
                DownButtonObject.interactable = true;
            return true;
        }
        #endregion

        #region SubClasses
        public class VRCEUiScrollPage
        {
            private List<VRCEUiQuickButton> _items = new List<VRCEUiQuickButton>();
            private VRCEUiQuickScrollMenu _parent;
            private int _row = 0;
            private int _column = 0;

            public bool CanAddItems => _items.Count < 9;
            public bool IsEmpty => _items.Count < 1;
            public bool IsActive { get; private set; } = false;

            public VRCEUiScrollPage(VRCEUiQuickScrollMenu parent) =>
                _parent = parent;

            public void SetActive(bool active)
            {
                foreach (VRCEUiQuickButton item in _items)
                {
                    if (item.Control == null || item.Control.gameObject == null)
                        continue;
                    item.Control.gameObject.SetActive(active);
                }
                IsActive = active;
            }
            public bool AddButton(VRCEUiQuickButton button)
            {
                if (!CanAddItems)
                    return false;
                RectTransform position = button.Position;

                if (position == null)
                    return false;

                button.Control.SetParent(_parent.ContentControl, false);
                button.Control.gameObject.SetActive(IsActive);
                position.anchorMin = new Vector2(0f, 0f);
                position.anchorMax = new Vector2(0f, 0f);
                position.pivot = new Vector2(0f, 1f);
                position.localPosition = new Vector3(_column * position.sizeDelta.x, -(_row * (position.sizeDelta.y - 15f)), 0f);
                
                if(_column >= 2)
                {
                    _column = 0;
                    _row++;
                }
                else
                    _column++;
                _items.Add(button);
                return true;
            }
            public bool HasButton(VRCEUiQuickButton item) =>
                _items.Contains(item);
        }
        #endregion
    }
}
