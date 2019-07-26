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

            ModManager.StartCoroutine(WaitForUIManager());

            MVRCLogger.Log("Started VRCMenuUtils!");
        }
        #endregion

        #region Coroutine Functions
        private IEnumerator WaitForUIManager()
        {
            yield return VRCUiManagerUtils.WaitForUiManagerInit();

            SetupUserInfo();

            // Debug
            /*Transform target = VRCEUi.InternalUserInfoScreen.UserPanel;
            MVRCLogger.Log("Transform: " + target.name);
            foreach (Component component in target.GetComponents<Component>())
                MVRCLogger.Log(" - " + component);
            for (int i = 0; i < target.childCount; i++)
            {
                MVRCLogger.Log("Transform: " + target.GetChild(i).name);
                foreach (Component component in target.GetChild(i).GetComponents<Component>())
                    MVRCLogger.Log(" - " + component);
            }*/
            for(int i = 0; i < 6; i++)
            {
                //VRCEUiButton button = new VRCEUiButton("TestBtn" + i, new Vector2(0f, 75f - (75f * i)), "Test Button " + i, UserInfoScroll.ContentControl);
                VRCEUiButton button = new VRCEUiButton("TestBtn" + i, new Vector2(0f, 0f), "Test Button " + i, UserInfoScroll.ContentControl);
                LayoutElement element = button.Control.gameObject.AddComponent<LayoutElement>();

                element.preferredWidth = 170f;
                element.preferredHeight = 40f;
                //UserInfoScroll.ContentPosition.sizeDelta += new Vector2(0f, 75f);
            }
            UserInfoScroll.ScrollRect.verticalNormalizedPosition = 0f;
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

            MVRCLogger.Log(size);
            UserInfoScroll = new VRCEUiScrollView("MoreScroll", new Vector2(pos.x, pos.y - 75f), new Vector2(200f, 75f * 3f), 40f, VRCEUi.InternalUserInfoScreen.UserPanel);
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

            // Add required events
            VRCMenuUtils.OnUserInfoButtonAdd += button =>
            {
                if (UserInfoMore == null || button == null)
                    return;


            };
        }
        #endregion
    }
}
