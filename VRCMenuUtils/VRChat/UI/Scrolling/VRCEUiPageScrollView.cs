using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

namespace VRChat.UI.Scrolling
{
    public class VRCEUiPageScrollView
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
        public int ItemsPerPage { get; private set; }
        public int CurrentPage { get; private set; } = 0;
        public float Spacing { get; private set; }
        public float Padding { get; private set; }
        public bool HasItems => Pages.Count > 0 && !Pages[0].IsEmpty;

        public RectTransform Position { get; private set; }
        public RectTransform ContentPosition { get; private set; }
        public RectTransform UpButtonPosition { get; private set; }
        public RectTransform DownButtonPosition { get; private set; }
        #endregion

        #region Control Properties
        public Button UpButton { get; private set; }
        public Button DownButton { get; private set; }
        #endregion

        public VRCEUiPageScrollView(string name, Vector2 position, Vector2 size, float spacing, int itemsPerPage, float padding = 0f, Transform parent = null)
        {
            // Create game objects
            GameObject goControl = new GameObject(name);
            GameObject goContentControl = new GameObject("Content");

            // Get positions
            Position = goControl.GetOrAddComponent<RectTransform>();
            ContentPosition = goContentControl.GetOrAddComponent<RectTransform>();

            // Set UI properties
            Control = goControl.transform;
            ContentControl = goContentControl.transform;

            // Set required parts
            if (parent != null)
                Control.SetParent(parent);

            // Setup Control
            Control.localScale = Vector3.one;
            Control.localRotation = Quaternion.identity;
            Control.localPosition = Vector3.zero;
            Position.anchorMin = new Vector2(0.5f, 0f);
            Position.anchorMax = new Vector2(0.5f, 0f);
            Position.pivot = new Vector2(0.5f, 1f);
            Position.localPosition = new Vector3(position.x, position.y, 0f);
            Position.sizeDelta = size;

            // Create buttons
            VRCEUiButton buttonUp = new VRCEUiButton("UpButton", new Vector2(0f, -30f), "ʌ Up ʌ", goControl.transform);
            VRCEUiButton buttonDown = new VRCEUiButton("DownButton", new Vector2(0f, (30f - size.y)), "v Down v", goControl.transform);

            // Setup UpButton
            UpButtonPosition = buttonUp.Position;
            UpButton = buttonUp.Button;
            UpButtonControl = buttonUp.Control;
            UpButton.interactable = false;
            UpButtonPosition.sizeDelta -= new Vector2(0f, Mathf.Ceil(UpButtonPosition.sizeDelta.y / 2f));
            UpButton.onClick.AddListener(() =>
            {
                if (CurrentPage < 1)
                    return;

                SetPage(CurrentPage - 1);
            });

            // Setup DownButton
            DownButtonPosition = buttonDown.Position;
            DownButton = buttonDown.Button;
            DownButtonControl = buttonDown.Control;
            DownButton.interactable = false;
            DownButtonPosition.sizeDelta -= new Vector2(0f, Mathf.Ceil(DownButtonPosition.sizeDelta.y / 2f));
            DownButton.onClick.AddListener(() =>
            {
                if (CurrentPage >= (Pages.Count - 1))
                    return;

                SetPage(CurrentPage + 1);
            });

            // Setup Content
            ContentControl.SetParent(Control);
            ContentControl.localScale = Vector3.one;
            ContentControl.localRotation = Quaternion.identity;
            ContentControl.localPosition = Vector3.zero;
            ContentPosition.anchorMin = new Vector2(0.5f, 0f);
            ContentPosition.anchorMax = new Vector2(0.5f, 0f);
            ContentPosition.pivot = new Vector2(0.5f, 1f);
            ContentPosition.localPosition = new Vector3(0f, -60f, 0f);
            ContentPosition.sizeDelta = new Vector2(size.x, size.y - (UpButtonPosition.sizeDelta.y + 100f));

            // Finish
            ItemsPerPage = itemsPerPage;
            Spacing = spacing;
            Padding = padding;
            VRCEUiScrollPage page = new VRCEUiScrollPage(itemsPerPage, spacing, padding, this);
            page.SetActive(true);
            Pages.Add(page);
            Success = true;
        }

        #region UI Functions
        public bool AddItem(Transform item)
        {
            VRCEUiScrollPage page = Pages[Pages.Count - 1];
            if (!page.CanAddItems)
            {
                page = new VRCEUiScrollPage(ItemsPerPage, Spacing, Padding, this);

                DownButton.interactable = true;
                Pages.Add(page);
            }

            return page.AddItem(item);
        }
        public bool HasItem(Transform item) =>
            Pages.Any(a => a.HasItem(item));
        public bool SetPage(int id)
        {
            if (id > Pages.Count)
                return false;

            Pages[CurrentPage].SetActive(false);
            Pages[id].SetActive(true);
            CurrentPage = id;

            if (CurrentPage > 0)
                UpButton.interactable = true;
            else
                UpButton.interactable = false;
            if ((Pages.Count - 1) <= CurrentPage)
                DownButton.interactable = false;
            else
                DownButton.interactable = true;
            return true;
        }
        #endregion

        #region SubClasses
        public class VRCEUiScrollPage
        {
            private List<Transform> _items = new List<Transform>();
            private float _spacing;
            private float _padding;
            private VRCEUiPageScrollView _parent;

            public int MaxItems { get; private set; }
            public bool CanAddItems => _items.Count < MaxItems;
            public bool IsEmpty => _items.Count < 1;
            public bool IsActive { get; private set; } = false;

            public VRCEUiScrollPage(int numItems, float spacing, float padding, VRCEUiPageScrollView parent)
            {
                MaxItems = numItems;
                _spacing = spacing;
                _padding = padding;
                _parent = parent;
            }

            public void SetActive(bool active)
            {
                foreach(Transform item in _items)
                {
                    if (item == null || item.gameObject == null)
                        continue;
                    item.gameObject.SetActive(active);
                }
                IsActive = active;
            }
            public bool AddItem(Transform item)
            {
                if (!CanAddItems)
                    return false;
                RectTransform position = item.GetComponent<RectTransform>();

                if (position == null)
                    return false;

                item.SetParent(_parent.ContentControl, false);
                item.gameObject.SetActive(IsActive);
                position.anchorMin = new Vector2(0.5f, 0f);
                position.anchorMax = new Vector2(0.5f, 0f);
                position.pivot = new Vector2(0.5f, 1f);
                position.localPosition = new Vector3(0f, -((_items.Count * _spacing) + _padding), 0f);

                _items.Add(item);
                return true;
            }
            public bool HasItem(Transform item) =>
                _items.Contains(item);
        }
        #endregion
    }
}
