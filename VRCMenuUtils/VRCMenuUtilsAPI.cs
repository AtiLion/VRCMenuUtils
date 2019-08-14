using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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
        #region VRCMenuUtils Variables
        private static bool _UIInitialized = false;
        private static bool _StartedUp = false;
        private static bool _FlowManagerPaused = false;
        private static bool _FlowManagerFinished = false;

        private static Queue<IEnumerator> _PreFlowExectution = new Queue<IEnumerator>();
        #endregion
        #region VRCMenuUtils Properties
        public static bool IsIntialized => _UIInitialized;
        public static string Version => "0.2.1";
        #endregion
        #region VRCMenuUtils Delegates
        public delegate void ElementChangeDelegate(Transform transform);
        public delegate void QuickMenuElementChangeDelegate(VRCEUiQuickButton button);
        #endregion
        #region VRCMenuUtils Coroutine Functions
        public static IEnumerator WaitForInit()
        {
            VRCFlowManager flowManager = Resources.FindObjectsOfTypeAll<VRCFlowManager>().FirstOrDefault();

            if (flowManager == null)
            {
                SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
                {
                    if (!_FlowManagerFinished && !_FlowManagerPaused)
                    {
                        // Disable FlowManager
                        DisableFlowManager();

                        if (!_StartedUp)
                        {
                            _StartedUp = true;

                            Resources.FindObjectsOfTypeAll<VRCFlowManager>().First().StartCoroutine(SetupUI());
                        }
                    }
                };
            }
            else
            {
                if (!_FlowManagerFinished && !_FlowManagerPaused)
                {
                    // Disable FlowManager
                    DisableFlowManager();
                }
                if (!_StartedUp)
                {
                    _StartedUp = true;

                    flowManager.StartCoroutine(SetupUI());
                }
            }

            while (!_UIInitialized)
                yield return null;
        }
        #endregion
        #region VRCMenuUtils Functions
        public static void RunBeforeFlowManager(IEnumerator func)
        {
            if (_FlowManagerFinished)
            {
                MVRCLogger.LogError("Attmpted to run function, after flow manager has been started!");
                return;
            }
            VRCFlowManager flowManager = Resources.FindObjectsOfTypeAll<VRCFlowManager>().FirstOrDefault();

            if(flowManager == null)
            {
                SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
                {
                    if (!_FlowManagerFinished && !_FlowManagerPaused)
                    {
                        // Disable FlowManager
                        DisableFlowManager();

                        if(!_StartedUp)
                        {
                            _StartedUp = true;

                            Resources.FindObjectsOfTypeAll<VRCFlowManager>().First().StartCoroutine(SetupUI());
                        }
                    }
                };
            }
            else
            {
                if (!_FlowManagerPaused)
                {
                    // Disable FlowManager
                    DisableFlowManager();
                }
                if (!_StartedUp)
                {
                    _StartedUp = true;

                    flowManager.StartCoroutine(SetupUI());
                }
            }

            _PreFlowExectution.Enqueue(func);
        }
        #endregion

        #region VRChat Reflection
        private static MethodInfo _miVRCUiManagerGetInstace;
        private static MethodInfo _miVRCUiPopupManagerGetInstance;
        #endregion
        #region VRChat Properties
        public static VRCUiManager VRCUiManager => (VRCUiManager)_miVRCUiManagerGetInstace?.Invoke(null, null);
        public static VRCUiPopupManager VRCUiPopupManager => (VRCUiPopupManager)_miVRCUiPopupManagerGetInstance?.Invoke(null, null);
        #endregion
        #region VRChat Functions
        public static void ShowUIPage(VRCUiPage page, bool removeHeader = true)
        {
            IEnumerator placeUi()
            {
                for (int i = 0; i < 4; i++)
                    yield return null;
                VRCUiManager.PlaceUi();

                if (removeHeader)
                    VRCEUi.ScreenHeader.SetActive(false);
            }

            VRCUiManager.ShowUi(false, true);
            VRCUiManager.StartCoroutine(placeUi());
            VRCUiManager.ShowScreen(page);
        }

        internal static void EnableFlowManager()
        {
            foreach (VRCFlowManager flowManager in Resources.FindObjectsOfTypeAll<VRCFlowManager>())
                flowManager.enabled = true;
            _FlowManagerPaused = false;
            MVRCLogger.LogWarning("Enabled Flow Manager!");
        }
        internal static void DisableFlowManager()
        {
            foreach (VRCFlowManager flowManager in Resources.FindObjectsOfTypeAll<VRCFlowManager>())
                flowManager.enabled = false;
            if (GameObject.Find("UserInterface") == null)
                SceneManager.LoadScene("ui", LoadSceneMode.Single);
            _FlowManagerPaused = true;
            MVRCLogger.LogWarning("Disabled Flow Manager!");
        }
        #endregion

        #region UserInfo Variables
        private static VRCEUiPageScrollView _userInfoScrollView;
        private static VRCEUiButton _userInfoMoreButton;
        private static Transform[] _userInfoDefaultButtons;
        #endregion
        #region UserInfo Events
        public static event ElementChangeDelegate OnUserInfoButtonAdd;
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
                _userInfoMoreButton.Text = "Less";
            else
                _userInfoMoreButton.Text = "More";
        }
        #endregion

        #region QuickMenu Variables
        private static VRCEUiQuickButton _quickMenuMoreButton;
        private static VRCEUiQuickScrollMenu _quickMenuMoreMenu;
        private static VRCEUiQuickButton _quickMenuLessButton;
        #endregion
        #region QuickMenu Events
        public static event QuickMenuElementChangeDelegate OnQuickMenuButtonAdd;
        #endregion
        #region QuickMenu Functions
        public static void AddQuickMenuButton(VRCEUiQuickButton button)
        {
            if (!_UIInitialized)
                return;

            OnQuickMenuButtonAdd?.Invoke(button);
        }
        public static void ShowQuickMenuPage(string page)
        {
            if (string.IsNullOrEmpty(page))
                return;
            Transform tPage = VRCEUi.QuickMenu?.transform.Find(page);
            if(tPage == null)
                MVRCLogger.LogError("Could not find QuickMenu page with name " + page);

            ShowQuickMenuPage(tPage);
        }
        public static void ShowQuickMenuPage(Transform page)
        {
            if (page == null)
                return;

            VRCEUi.InternalQuickMenu.CurrentPage.SetActive(false);
            VRCEUi.InternalQuickMenu.InfoBar.gameObject.SetActive(false);
            VRCEUi.InternalQuickMenu.CurrentPage = page.gameObject;
            page.gameObject.SetActive(true);
        }
        #endregion

        #region Control Coroutine Functions
        private static IEnumerator SetupUI()
        {
            if (_miVRCUiManagerGetInstace != null && _UIInitialized)
                yield break;

            // Grab VRCUiManager
            _miVRCUiManagerGetInstace = typeof(VRCUiManager).GetMethod("get_Instance", BindingFlags.Public | BindingFlags.Static);
            if(_miVRCUiManagerGetInstace == null)
            {
                MVRCLogger.LogError("Failed to find get_Instance in VRCUiManager!");
                yield break;
            }
            MVRCLogger.Log("Waiting for VRCUiManager to load...");
            while (VRCUiManager == null)
                yield return null;
            if(_miVRCUiPopupManagerGetInstance == null)
            {
                _miVRCUiPopupManagerGetInstance = typeof(VRCUiPopupManager).GetMethod("get_Instance", BindingFlags.Public | BindingFlags.Static);
                if (_miVRCUiPopupManagerGetInstance == null)
                {
                    MVRCLogger.LogError("Failed to find get_Instance in VRCUiPopupManager!");
                    yield break;
                }

                while (VRCUiPopupManager == null)
                    yield return null;
            }
            MVRCLogger.Log("VRCUiManager has been loaded!");

#if !DEBUG
            // Check for updates
            RunBeforeFlowManager(CheckForUpdates());
#endif

            // Setup UserInfo
            yield return SetupUserInfo();

            // Setup Quick Menu
            yield return SetupQuickMenu();

            // Run pre-flow manager functions
            while (!_FlowManagerPaused)
                yield return null;
            while (_PreFlowExectution.Count > 0)
                yield return _PreFlowExectution.Dequeue();

            // Finish
            EnableFlowManager();
            _FlowManagerFinished = true;
            OnUserInfoButtonAdd += _UserInfoButtonAdded;
            OnQuickMenuButtonAdd += _QuickMenuButtonAdded;
            _UIInitialized = true;
        }
        private static IEnumerator CheckForUpdates()
        {
            string newVersion = null;
            bool hasPopup = true;

            MVRCLogger.Log("Checking for updates...");
            using(UnityWebRequest request = UnityWebRequest.Get("https://api.github.com/repos/AtiLion/VRCMenuUtils/releases/latest"))
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

                    newVersion = (string)version;
                }
                catch (Exception ex)
                {
                    MVRCLogger.LogError("Version check failed! Invalid format!", ex);
                    yield break;
                }
            }

            if (newVersion != null)
            {
                // Show update
                MVRCLogger.Log("New update has been found! Version: " + newVersion);
                VRCUiPopupManager.ShowStandardPopup(
                        "VRCMenuUtils Update",
                        "A new VRCMenuUtils update is now available! Please update as soon as you can.",
                        "Close", () => { VRCUiPopupManager.HideCurrentPopup(); hasPopup = false; },
                        "Open", () => System.Diagnostics.Process.Start("https://github.com/AtiLion/VRCMenuUtils/releases")
                    );
                while (hasPopup) yield return null;
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
            _userInfoMoreButton.Control.gameObject.SetActive(_userInfoScrollView.HasItems);
            _userInfoMoreButton.OnClick += () =>
                SetUserInfoUIState(_userInfoMoreButton.Text == "More");
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
            _quickMenuMoreMenu = new VRCEUiQuickScrollMenu("MoreMenu", false);

            _quickMenuMoreButton = new VRCEUiQuickButton("MoreButton", new Vector2(quickMenuButtonPos.x, quickMenuButtonPos.y + 840f), "More", "Shows more Quick Menu buttons that mods have added.", VRCEUi.InternalQuickMenu.ShortcutMenu);
            _quickMenuMoreButton.Control.gameObject.SetActive(_quickMenuMoreMenu.HasButtons);
            _quickMenuMoreButton.OnClick += () =>
            {
                if (VRCEUi.InternalQuickMenu.CurrentPage == null)
                    return;

                ShowQuickMenuPage(_quickMenuMoreMenu.Control);
            };
            _quickMenuLessButton = new VRCEUiQuickButton("LessButton", new Vector2(quickMenuButtonPos.x, quickMenuButtonPos.y + 420f), "Less", "Takes you back to the main Quick Menu screen.", _quickMenuMoreMenu.Control);
            _quickMenuLessButton.OnClick += () =>
            {
                if (VRCEUi.QuickMenu == null)
                    return;

                VRCEUi.QuickMenu.SetMenuIndex(0);
            };
            MVRCLogger.Log("QuickMenu UI has been loaded!");
        }
        #endregion
        #region Control Events
        private static void _UserInfoButtonAdded(Transform button)
        {
            _userInfoScrollView.AddItem(button);

            if (_userInfoScrollView.HasItems)
                _userInfoMoreButton.Control.gameObject.SetActive(true);
            else
                _userInfoMoreButton.Control.gameObject.SetActive(false);
        }
        private static void _QuickMenuButtonAdded(VRCEUiQuickButton button)
        {
            _quickMenuMoreMenu.AddButton(button);

            if (_quickMenuMoreMenu.HasButtons)
                _quickMenuMoreButton.Control.gameObject.SetActive(true);
            else
                _quickMenuMoreButton.Control.gameObject.SetActive(false);
        }
        #endregion
    }
}
