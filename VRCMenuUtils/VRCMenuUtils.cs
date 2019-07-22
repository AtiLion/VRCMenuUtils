using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRChat.UI;

namespace VRCMenuUtils
{
    // Use this as the API endpoint
    public static class VRCMenuUtils
    {
        #region UserInfo Properties
        internal static List<VRCEUiButton> UserInfoButtons = new List<VRCEUiButton>();
        #endregion

        #region UserInfo Functions
        public static void AddUserInfoButton(VRCEUiButton button) =>
            UserInfoButtons.Add(button);
        #endregion
    }
}
