using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRCMenuUtils;

namespace VRChat.UI.Scrolling
{
    public class VRCEUiGridScrollView
    {
        #region VRCUI Properties
        public bool Success { get; private set; }
        #endregion

        #region UI Properties
        public Transform Control { get; private set; }
        public Transform ContentControl { get; private set; }

        public RectTransform Position { get; private set; }
        public RectTransform ContentPosition { get; private set; }
        #endregion

        #region Control Properties
        public ScrollRect ScrollRectObject { get; private set; }
        public ContentSizeFitter SizeFitterObject { get; private set; }
        public GridLayoutGroup LayoutGroupObject { get; private set; }
        public Mask MaskObject { get; private set; }
        public Image ImageObject { get; private set; }
        #endregion

        public VRCEUiGridScrollView(string name, Vector2 position, Vector2 size, float spacing, Vector2 cellSize, int cellCount, RectOffset padding = null, Transform parent = null)
        {
            // Create game objects
            GameObject goControl = new GameObject(name);
            GameObject goContentControl = new GameObject("Content");

            // Get positions
            Position = goControl.GetOrAddComponent<RectTransform>();
            ContentPosition = goContentControl.GetOrAddComponent<RectTransform>();

            // Create control properties
            ScrollRectObject = goControl.AddComponent<ScrollRect>();
            SizeFitterObject = goContentControl.AddComponent<ContentSizeFitter>();
            LayoutGroupObject = goContentControl.AddComponent<GridLayoutGroup>();
            MaskObject = goControl.AddComponent<Mask>();
            ImageObject = goControl.AddComponent<Image>();

            // Set UI properties
            Control = goControl.transform;
            ContentControl = goContentControl.transform;

            // Set required parts
            if (parent != null)
                Control.SetParent(parent);

            // Setup ScrollView
            Control.localScale = Vector3.one;
            Control.localRotation = Quaternion.identity;
            Control.localPosition = Vector3.zero;
            Position.localPosition = new Vector3(position.x, position.y, 0f);
            Position.sizeDelta = size;
            ScrollRectObject.vertical = true;
            ScrollRectObject.horizontal = false;

            // Setup mask
            MaskObject.showMaskGraphic = false;
            Texture2D texture = new Texture2D(2, 2);
            Color color = new Color(0f, 0f, 0f, 1f);
            texture.SetPixels(new Color[] { color, color, color, color });
            texture.Apply();
            ImageObject.sprite = Sprite.Create(texture, new Rect(0f, 0f, 2f, 2f), new Vector2(0f, 0f));

            // Setup Content
            ContentControl.SetParent(Control);
            ContentControl.localScale = Vector3.one;
            ContentControl.localRotation = Quaternion.identity;
            ContentControl.localPosition = Vector3.zero;
            ContentPosition.localPosition = new Vector3(0f, 0f, 0f);
            ContentPosition.anchorMin = new Vector2(0.5f, 1f);
            ContentPosition.anchorMax = new Vector2(0.5f, 1f);
            ContentPosition.pivot = new Vector2(0.5f, 1f);
            ContentPosition.sizeDelta = new Vector2(spacing, spacing);
            SizeFitterObject.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            LayoutGroupObject.spacing = new Vector2(spacing, spacing);
            LayoutGroupObject.childAlignment = TextAnchor.UpperLeft;
            LayoutGroupObject.startCorner = GridLayoutGroup.Corner.UpperLeft;
            LayoutGroupObject.startAxis = GridLayoutGroup.Axis.Horizontal;
            LayoutGroupObject.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            LayoutGroupObject.constraintCount = cellCount;
            LayoutGroupObject.cellSize = cellSize;
            if (padding != null)
                LayoutGroupObject.padding = padding;

            ScrollRectObject.content = ContentPosition;
            ScrollRectObject.viewport = Position;

            // Finish
            Success = true;
        }
    }
}
