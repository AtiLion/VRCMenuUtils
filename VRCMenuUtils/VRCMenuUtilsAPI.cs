using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using VRChat.UI;
using VRChat.UI.QuickMenuUI;
using VRChat.UI.Scrolling;

namespace VRCMenuUtils
{
    // Use this as the API endpoint
    public static class VRCMenuUtilsAPI
    {
        #region VRChat Reflection
        private static MethodInfo _miVRCUiManagerGetInstace;
        private static MethodInfo _miVRCUiPopupManagerGetInstance;
        #endregion

        #region VRCMenuUtils Variables
        private static bool _UIInitialized = false;
        private static bool _StartedUp = false;
        #endregion
        #region VRCMenuUtils Properties
        public static bool IsIntialized => _UIInitialized;
        public static string Version => "0.1.0";
        #endregion

        #region UserInfo Variables
        private static VRCEUiPageScrollView _userInfoScrollView;
        private static VRCEUiButton _userInfoMoreButton;
        private static Transform[] _userInfoDefaultButtons;
        #endregion

        #region QuickMenu Variables
        private static VRCEUiQuickButton _quickMenuMoreButton;
        private static VRCEUiQuickScrollMenu _quickMenuMoreMenu;
        private static VRCEUiQuickButton _quickMenuLessButton;
        #endregion

        #region VRCMenuUtils Delegates
        public delegate void ElementChangeDelegate(Transform transform);
        public delegate void QuickMenuElementChangeDelegate(VRCEUiQuickButton button);
        #endregion

        #region UserInfo Events
        public static event ElementChangeDelegate OnUserInfoButtonAdd;
        #endregion

        #region QuickMenu Events
        public static event QuickMenuElementChangeDelegate OnQuickMenuButtonAdd;
        #endregion

        #region UserInfo Functions
        public static void AddUserInfoButton(VRCEUiButton button) =>
            AddUserInfoButton(button.Control);
        public static void AddUserInfoButton(Transform button)
        {
            if (!_UIInitialized)
                return;

            OnUserInfoButtonAdd?.Invoke(button);
        }
        #endregion

        #region QuickMenu Functions
        public static void AddQuickMenuButton(VRCEUiQuickButton button)
        {
            if (!_UIInitialized)
                return;

            OnQuickMenuButtonAdd?.Invoke(button);
        }
        #endregion

        #region UserInfo Control Functions
        internal static void SetUserInfoUIState(bool active)
        {
            // Modify VRChat buttons
            foreach (Transform button in _userInfoDefaultButtons)
                button.gameObject.SetActive(!active);

            // Change Scrollview
            _userInfoScrollView.Control.gameObject.SetActive(active);

            // Change text
            if (active)
                _userInfoMoreButton.Text.text = "Less";
            else
                _userInfoMoreButton.Text.text = "More";
        }
        #endregion

        #region MenuUtils Coroutine Functions
        public static IEnumerator WaitForInit()
        {
            if(!_StartedUp)
            {
                _StartedUp = true;
                yield return SetupUI();
            }
            while (!_UIInitialized)
                yield return null;
        }
        #endregion

        #region Control Coroutine Functions
        private static IEnumerator SetupUI()
        {
            // Grab VRCUiManager
            if (_miVRCUiManagerGetInstace != null && _UIInitialized)
                yield break;
            _miVRCUiManagerGetInstace = typeof(VRCUiManager).GetMethod("get_Instance", BindingFlags.Public | BindingFlags.Static);

            if(_miVRCUiManagerGetInstace == null)
            {
                MVRCLogger.LogError("Failed to find get_Instance in VRCUiManager!");
                yield break;
            }
            MVRCLogger.Log("Waiting for VRCUiManager to load...");
            while (_miVRCUiManagerGetInstace.Invoke(null, null) == null)
                yield return null;
            if(_miVRCUiPopupManagerGetInstance == null)
            {
                _miVRCUiPopupManagerGetInstance = typeof(VRCUiPopupManager).GetMethod("get_Instance", BindingFlags.Public | BindingFlags.Static);
                if (_miVRCUiPopupManagerGetInstance == null)
                {
                    MVRCLogger.LogError("Failed to find get_Instance in VRCUiPopupManager!");
                    yield break;
                }

                while (_miVRCUiPopupManagerGetInstance.Invoke(null, null) == null)
                    yield return null;
            }
            MVRCLogger.Log("VRCUiManager has been loaded!");

            // Check for updates
            yield return CheckForUpdates();

            // Setup UserInfo
            yield return SetupUserInfo();

            // Setup Quick Menu
            yield return SetupQuickMenu();

            // Finish
            OnUserInfoButtonAdd += _UserInfoButtonAdded;
            OnQuickMenuButtonAdd += _QuickMenuButtonAdded;
            _UIInitialized = true;
        }
        private static IEnumerator CheckForUpdates()
        {
            MVRCLogger.Log("Checking for updates...");
            using(UnityWebRequest request = UnityWebRequest.Get("https://api.github.com/AtiLion/VRCMenuUtils/releases/latest"))
            {
                yield return request.SendWebRequest();

                if(request.isNetworkError)
                {
                    MVRCLogger.LogError("Network error! Failed to check for updates!");
                    yield break;
                }
                if(request.isHttpError)
                {
                    MVRCLogger.LogError("HTTP error! Failed to check for updates!");
                    yield break;
                }
                try
                {
                    JObject data = JObject.Parse(request.downloadHandler.text);
                    JToken version;

                    if(!data.TryGetValue("tag_name", out version))
                    {
                        MVRCLogger.LogError("No version data found!");
                        yield break;
                    }
                    if((string)version == Version)
                    {
                        MVRCLogger.Log("No updates found!");
                        yield break;
                    }

                    MVRCLogger.Log("New update has been found! Version: " + (string)version);
                    VRCUiPopupManager popupManager = (VRCUiPopupManager)_miVRCUiPopupManagerGetInstance.Invoke(null, null);
                    popupManager.ShowStandardPopup(
                            "VRCMenuUtils Update",
                            "A new VRCMenuUtils update is now available! Please update as soon as you can.",
                            "Close", () => popupManager.HideCurrentPopup(),
                            "Open in browser", () => System.Diagnostics.Process.Start("https://github.com/AtiLion/VRCMenuUtils/releases")
                        );
                }
                catch (Exception ex)
                {
                    MVRCLogger.LogError("Version check failed! Invalid format!", ex);
                    yield break;
                }
            }
        }

        private static IEnumerator SetupUserInfo()
        {
            // Run UI checks
            while (VRCEUi.UserInfoScreen == null)
                yield return null;

            // Get UserInfo defaults
            _userInfoDefaultButtons = new Transform[]
            {
                VRCEUi.InternalUserInfoScreen.PlaylistsButton,
                VRCEUi.InternalUserInfoScreen.FavoriteButton,
                VRCEUi.InternalUserInfoScreen.ReportButton,
                VRCEUi.InternalUserInfoScreen.OnlineVoteKickButton,
                VRCEUi.InternalUserInfoScreen.OnlineJoinButton,
                VRCEUi.InternalUserInfoScreen.OfflineJoinButton
            };
            if (_userInfoDefaultButtons.Any(a => a == null))
            {
                MVRCLogger.LogError("Failed to get UserInfo default buttons!");
                yield break;
            }
            Vector3 userInfoButtonPos = _userInfoDefaultButtons[0].GetComponent<RectTransform>().localPosition;

            // Load UserInfo UI
            MVRCLogger.Log("Loading UserInfo UI...");
            _userInfoScrollView = new VRCEUiPageScrollView("MoreScroll", new Vector2(userInfoButtonPos.x, userInfoButtonPos.y + 35f), new Vector2(200f, 75f * 5f), 75f, 3, 33f, VRCEUi.InternalUserInfoScreen.UserPanel);
            _userInfoScrollView.Control.gameObject.SetActive(false);

            _userInfoMoreButton = new VRCEUiButton("More", new Vector2(userInfoButtonPos.x, userInfoButtonPos.y + 75f), "More", VRCEUi.InternalUserInfoScreen.UserPanel);
            _userInfoMoreButton.Button.onClick.AddListener(() =>
                SetUserInfoUIState(_userInfoMoreButton.Text.text == "More"));
            VRCEUi.UserInfoScreen.AddComponent<UserInfoActivityManager>();
            MVRCLogger.Log("UserInfo UI has been loaded!");
        }
        private static IEnumerator SetupQuickMenu()
        {
            // Run UI checks
            while (VRCEUi.QuickMenu == null)
                yield return null;

            // Get QuickMenu defaults
            Vector2 quickMenuButtonPos = VRCEUi.InternalQuickMenu.ReportWorldButton.GetComponent<RectTransform>().localPosition;

            // Load QuickMenu UI
            MVRCLogger.Log("Loading QuickMenu UI...");
            _quickMenuMoreButton = new VRCEUiQuickButton("MoreButton", new Vector2(quickMenuButtonPos.x, quickMenuButtonPos.y + 840f), "More", "Shows more Quick Menu buttons that mods have added.", VRCEUi.InternalQuickMenu.ShortcutMenu);
            _quickMenuMoreButton.Button.onClick.AddListener(() =>
            {
                if (VRCEUi.InternalQuickMenu.CurrentPage == null)
                    return;

                VRCEUi.InternalQuickMenu.CurrentPage.SetActive(false);
                VRCEUi.InternalQuickMenu.InfoBar.gameObject.SetActive(false);
                VRCEUi.InternalQuickMenu.CurrentPage = _quickMenuMoreMenu.Control.gameObject;
                _quickMenuMoreMenu.Control.gameObject.SetActive(true);
            });

            _quickMenuMoreMenu = new VRCEUiQuickScrollMenu("MoreMenu", false);
            _quickMenuLessButton = new VRCEUiQuickButton("LessButton", new Vector2(quickMenuButtonPos.x, quickMenuButtonPos.y + 420f), "Less", "Takes you back to the main Quick Menu screen.", _quickMenuMoreMenu.Control);
            _quickMenuLessButton.Button.onClick.AddListener(() =>
            {
                if (VRCEUi.QuickMenu == null)
                    return;

                VRCEUi.QuickMenu.SetMenuIndex(0);
            });
            MVRCLogger.Log("QuickMenu UI has been loaded!");
        }
        #endregion

        #region Control Events
        private static void _UserInfoButtonAdded(Transform button) =>
            _userInfoScrollView.AddItem(button);
        private static void _QuickMenuButtonAdded(VRCEUiQuickButton button) =>
            _quickMenuMoreMenu.AddButton(button);
        #endregion
    }
}
