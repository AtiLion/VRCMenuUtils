using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using VRChat.UI;

namespace VRCMenuUtils
{
    // Use this as the API endpoint
    public static class VRCMenuUtils
    {
        #region VRChat Reflection
        private static MethodInfo _miVRCUiManagerGetInstace;
        #endregion

        #region VRCMenuUtils Variables
        private static bool _UIInitialized = false;
        private static GameObject _targetHook = null;
        private static UtilExecutor _targetExecutor = null;
        #endregion

        #region VRCMenuUtils Delegates
        public delegate void ElementChangeDelegate(Transform transform);
        #endregion

        #region UserInfo Variables
        public static VRCEUiVerticalScrollView _userInfoScrollView;
        private static VRCEUiButton _userInfoMoreButton;
        #endregion

        #region UserInfo Properties
        internal static List<Transform> UserInfoButtons = new List<Transform>();
        #endregion

        #region UserInfo Events
        public static event ElementChangeDelegate OnUserInfoButtonAdd;
        public static event ElementChangeDelegate OnUserInfoButtonRemove;
        #endregion

        static VRCMenuUtils()
        {
            // We can assume UnityEngine is loaded by this point
            _targetHook = new GameObject();
            _targetExecutor = _targetHook.AddComponent<UtilExecutor>();
            GameObject.DontDestroyOnLoad(_targetHook);

            _targetExecutor.StartCoroutine(SetupUI());
        }

        #region UserInfo Functions
        public static void AddUserInfoButton(VRCEUiButton button) =>
            AddUserInfoButton(button.Control);
        public static void AddUserInfoButton(Transform button)
        {
            if (!_UIInitialized)
                return;

            UserInfoButtons.Add(button);
            OnUserInfoButtonAdd?.Invoke(button);
        }

        public static void RemoveUserInfoButton(VRCEUiButton button) =>
            RemoveUserInfoButton(button.Control);
        public static void RemoveUserInfoButton(Transform button)
        {
            if (!_UIInitialized)
                return;

            UserInfoButtons.Remove(button);
            OnUserInfoButtonRemove?.Invoke(button);
        }
        #endregion

        #region MenuUtils Coroutine Functions
        public static IEnumerator WaitForInit()
        {
            // Dual loading sucks I swear
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
            MVRCLogger.Log("VRCUiManager has been loaded!");

            // Run UI checks
            while (VRCEUi.UserInfoScreen == null)
                yield return null;

            // Get UserInfo defaults
            Transform[] userInfoButtons = new Transform[]
            {
                VRCEUi.InternalUserInfoScreen.PlaylistsButton,
                VRCEUi.InternalUserInfoScreen.FavoriteButton,
                VRCEUi.InternalUserInfoScreen.ReportButton
            };
            if (userInfoButtons.Any(a => a == null))
            {
                MVRCLogger.LogError("Failed to get UserInfo default buttons!");
                yield break;
            }
            Vector3 userInfoButtonPos = userInfoButtons[0].GetComponent<RectTransform>().localPosition;

            // Load UserInfo UI
            MVRCLogger.Log("Loading UserInfo UI...");
            _userInfoScrollView = new VRCEUiVerticalScrollView("MoreScroll", new Vector2(userInfoButtonPos.x, userInfoButtonPos.y - 75f), new Vector2(200f, 75f * 3f), 35f, new RectOffset(0, 0, 17, 17), VRCEUi.InternalUserInfoScreen.UserPanel);
            _userInfoScrollView.Control.gameObject.SetActive(false);

            _userInfoMoreButton = new VRCEUiButton("More", new Vector2(userInfoButtonPos.x, userInfoButtonPos.y + 75f), "More", VRCEUi.InternalUserInfoScreen.UserPanel);
            _userInfoMoreButton.Button.onClick.AddListener(() =>
            {
                if (_userInfoMoreButton.Text.text == "More")
                {
                    // Modify VRChat buttons
                    foreach (Transform button in userInfoButtons)
                        button.gameObject.SetActive(false);

                    // Change Scrollview
                    _userInfoScrollView.Control.gameObject.SetActive(true);

                    // Change text
                    _userInfoMoreButton.Text.text = "Less";
                }
                else
                {
                    // Modify VRChat buttons
                    foreach (Transform button in userInfoButtons)
                        button.gameObject.SetActive(true);

                    // Change Scrollview
                    _userInfoScrollView.Control.gameObject.SetActive(false);

                    // Change text
                    _userInfoMoreButton.Text.text = "More";
                }
            });
            MVRCLogger.Log("UserInfo UI has been loaded!");

            // Finish
            OnUserInfoButtonAdd += _UserInfoButtonAdded;
            _UIInitialized = true;
        }
        #endregion

        #region Control Events
        private static void _UserInfoButtonAdded(Transform button)
        {
            // Setup button internals
            button.SetParent(_userInfoScrollView.ContentControl, false);

            // Setup button UI
            Vector2 scale = button.GetComponent<RectTransform>().sizeDelta;
            LayoutElement element = button.gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = scale.x;
            element.preferredHeight = scale.y;
        }
        #endregion
    }
}
