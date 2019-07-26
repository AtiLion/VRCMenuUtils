using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using VRChat.UI;

namespace VRCMenuUtils
{
    // Use this as the API endpoint
    public static class VRCMenuUtils
    {
        #region VRCMenuUtils Delegates
        public delegate void ElementChangeDelegate(Transform transform);
        #endregion

        #region UserInfo Properties
        internal static List<Transform> UserInfoButtons = new List<Transform>();
        #endregion

        #region UserInfo Events
        public static event ElementChangeDelegate OnUserInfoButtonAdd;
        public static event ElementChangeDelegate OnUserInfoButtonRemove;
        #endregion

        #region UserInfo Functions
        public static void AddUserInfoButton(VRCEUiButton button) =>
            AddUserInfoButton(button.Control);
        public static void AddUserInfoButton(Transform button)
        {
            UserInfoButtons.Add(button);
            OnUserInfoButtonAdd?.Invoke(button);
        }

        public static void RemoveUserInfoButton(VRCEUiButton button) =>
            RemoveUserInfoButton(button.Control);
        public static void RemoveUserInfoButton(Transform button)
        {
            UserInfoButtons.Remove(button);
            OnUserInfoButtonRemove?.Invoke(button);
        }
        #endregion
    }
}
