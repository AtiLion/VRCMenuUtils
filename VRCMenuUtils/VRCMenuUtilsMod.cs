﻿using System;
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
    [VRCModInfo("VRCMenuUtils", "0.0.1.0", "VRChat Nerds")]
    internal class VRCMenuUtilsMod : VRCMod
    {
        #region UI Properties
        public VRCEUiButton UserInfoMore { get; private set; }
        public VRCEUiScrollView UserInfoScroll { get; private set; }
        #endregion

        #region VRCMod Functions
        void OnApplicationStart()
        {
            MVRCLogger.Log("Starting VRCMenuUtils...");

            ModManager.StartCoroutine(ExecuteUI());

            MVRCLogger.Log("Started VRCMenuUtils!");
        }
        #endregion

        private IEnumerator ExecuteUI()
        {
            yield return VRCMenuUtils.WaitForUtilsLoad();

            for(int i = 0; i < 10; i++)
            {
                VRCEUiButton button = new VRCEUiButton("Test " + i, new Vector2(0f, 0f), "Test " + i);

                VRCMenuUtils.AddUserInfoButton(button);
            }
            MVRCLogger.Log("Added buttons!");
        }

        /*#region Coroutine Functions
        private IEnumerator WaitForUIManager()
        {
            yield return VRCUiManagerUtils.WaitForUiManagerInit();

            SetupUserInfo();

            for (int i = 0; i < 24; i++)
            {
                VRCEUiButton button = new VRCEUiButton("TestBtn" + i, new Vector2(0f, 0f), "Test Button " + i, UserInfoScroll.ContentControl);
                LayoutElement element = button.Control.gameObject.AddComponent<LayoutElement>();

                element.preferredWidth = 170f;
                element.preferredHeight = 40f;
            }
            MVRCLogger.Log("Added buttons!");
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
            Vector2 size = buttons[0].GetComponent<RectTransform>().sizeDelta;

            UserInfoScroll = new VRCEUiScrollView("MoreScroll", new Vector2(pos.x, pos.y - 75f), new Vector2(200f, 75f * 3f), 35f, new RectOffset(0, 0, 17, 17), VRCEUi.InternalUserInfoScreen.UserPanel);
            UserInfoScroll.Control.gameObject.SetActive(false);

            // It would be a smart idea to make this scrollable for unlimited buttons?
            UserInfoMore = new VRCEUiButton("More", new Vector2(pos.x, pos.y + 75f), "More", VRCEUi.InternalUserInfoScreen.UserPanel);
            UserInfoMore.Button.onClick.AddListener(() =>
            {
                if(UserInfoMore.Text.text == "More")
                {
                    // Modify VRChat buttons
                    foreach (Transform button in buttons)
                        button.gameObject.SetActive(false);

                    // Change Scrollview
                    UserInfoScroll.Control.gameObject.SetActive(true);

                    // Change text
                    UserInfoMore.Text.text = "Less";
                }
                else
                {
                    // Modify VRChat buttons
                    foreach (Transform button in buttons)
                        button.gameObject.SetActive(true);

                    // Change Scrollview
                    UserInfoScroll.Control.gameObject.SetActive(false);

                    // Change text
                    UserInfoMore.Text.text = "More";
                }
            });
        }
        #endregion*/
    }
}