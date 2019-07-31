using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRCTools;
using VRCModLoader;

using VRChat.UI;

namespace VRCMenuUtils
{
    // THIS IS A DEBUG ONLY CLASS!
    [VRCModInfo("VRCMenuUtils", "0.0.1.0", "VRChat Nerds")]
    internal class VRCMenuUtilsMod : VRCMod
    {
        void OnApplicationStart()
        {
            MVRCLogger.Log("Starting VRCMenuUtils...");

            ModManager.StartCoroutine(ExecuteUI());

            MVRCLogger.Log("Started VRCMenuUtils!");
        }

        private IEnumerator ExecuteUI()
        {
            yield return VRCMenuUtils.WaitForInit();

            // Grab all the buttons
            /*foreach (Button button in VRCEUi.UserInfoScreen.GetComponentsInChildren<Button>(true))
            {
                if (button.transform.parent == null)
                    continue;
                string buttonPosition = button.transform.name;
                Transform position = button.transform.parent;

                while(position.name != VRCEUi.UserInfoScreen.name)
                {
                    buttonPosition += " -> " + position.name;
                    if (position.parent != null)
                        position = position.parent;
                }
                MVRCLogger.Log(buttonPosition);
            }*/

            // User Info
            for(int i = 0; i < 10; i++)
            {
                VRCEUiButton button = new VRCEUiButton("Test " + i, new Vector2(0f, 0f), "Test " + i);

                VRCMenuUtils.AddUserInfoButton(button);
            }
            MVRCLogger.Log("Added UserInfo buttons!");
        }
    }
}
