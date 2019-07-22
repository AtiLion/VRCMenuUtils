using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using VRCTools;
using VRCModLoader;

using VRChat.UI;

namespace VRCMenuUtils
{
    [VRCModInfo("VRCMenuUtils", "0.0.1.0", "VRChat Nerds")]
    internal class VRCMenuUtilsMod : VRCMod
    {
        #region UI Properties
        public VRCEUiButton UserInfoMore { get; private set; }
        #endregion

        #region VRCMod Functions
        void OnApplicationStart()
        {
            MVRCLogger.Log("Starting VRCMenuUtils...");

            ModManager.StartCoroutine(WaitForUIManager());

            MVRCLogger.Log("Started VRCMenuUtils!");
        }
        #endregion

        #region Coroutine Functions
        private IEnumerator WaitForUIManager()
        {
            yield return VRCUiManagerUtils.WaitForUiManagerInit();

            SetupUserInfo();
        }
        #endregion

        #region UI Functions
        private void SetupUserInfo()
        {
            // User info check
            if (VRCEUi.UserInfoScreen == null)
            {
                MVRCLogger.LogError("Failed to find UserInfo screen!");
                return;
            }

            Transform[] buttons = new Transform[]
            {
                VRCEUi.InternalUserInfoScreen.PlaylistsButton,
                VRCEUi.InternalUserInfoScreen.FavoriteButton,
                VRCEUi.InternalUserInfoScreen.ReportButton
            };
            if (buttons.Any(a => a == null))
            {
                MVRCLogger.LogError("Failed to get required button!");
                return;
            }
            Vector3 pos = buttons[0].GetComponent<RectTransform>().localPosition;

            // It would be a smart idea to make this scrollable for unlimited buttons?
            UserInfoMore = new VRCEUiButton("More", new Vector2(pos.x, pos.y + 75f), "More", VRCEUi.InternalUserInfoScreen.UserPanel);
            UserInfoMore.Button.onClick.AddListener(() =>
            {
                if(UserInfoMore.Text.text == "More")
                {
                    // Modify VRChat buttons
                    foreach (Transform button in buttons)
                        button.gameObject.SetActive(false);

                    // Change text
                    UserInfoMore.Text.text = "Less";
                }
                else
                {
                    // Modify VRChat buttons
                    foreach (Transform button in buttons)
                        button.gameObject.SetActive(true);

                    // Change text
                    UserInfoMore.Text.text = "More";
                }
            });
        }
        #endregion
    }
}
