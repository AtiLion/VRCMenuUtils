using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRCMenuUtils;

namespace VRChat.UI
{
    public class VRCEUiQuickMenu
    {
        #region VRCUI Properties
        public bool Success { get; private set; }
        #endregion

        #region UI Properties
        public Transform Control { get; private set; }
        #endregion

        public VRCEUiQuickMenu(string name, bool ignoreBack = true)
        {
            // Get required information
            Transform orgControl = VRCEUi.InternalQuickMenu.CameraMenu;
            if (orgControl == null)
            {
                MVRCLogger.LogError("Could not find Camera Menu!");
                Success = false;
                return;
            }

            // Duplicate object
            GameObject goControl = GameObject.Instantiate(orgControl.gameObject, VRCEUi.QuickMenu.transform);
            if (goControl == null)
            {
                MVRCLogger.LogError("Could not duplicate Camera Menu!");
                Success = false;
                return;
            }

            // Set UI properties
            Control = goControl.transform;

            // Set required parts
            goControl.name = name;

            foreach (Transform button in Control)
            {
                if (button == null)
                    continue;
                if (button.name == "BackButton" && ignoreBack)
                    continue;
                GameObject.Destroy(button.gameObject);
            }
        }
    }
}
