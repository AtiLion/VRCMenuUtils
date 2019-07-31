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
        #endregion

        #region VRCMenuUtils Delegates
        public delegate void ElementChangeDelegate(Transform transform);
        #endregion

        #region UserInfo Variables
        public static VRCEUiScrollView _userInfoScrollView;
        private static VRCEUiButton _userInfoMoreButton;
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

        #region Coroutine Functions
        public static IEnumerator WaitForInit()
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
            _userInfoScrollView = new VRCEUiScrollView("MoreScroll", new Vector2(userInfoButtonPos.x, userInfoButtonPos.y - 75f), new Vector2(200f, 75f * 3f), 35f, new RectOffset(0, 0, 17, 17), VRCEUi.InternalUserInfoScreen.UserPanel);
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
            try
            {
                foreach (Button button in _userInfoMoreButton.Button.transform.parent.GetComponentsInChildren<Button>())
                {
                    if (button != _userInfoMoreButton.Button.GetComponent<Button>())
                    {
                        GameObject buttonObj = button.gameObject;
                        Transform buttonTrans = button.transform;
                        if (Vector3.Distance(buttonTrans.position, _userInfoMoreButton.Position.position) < 5)
                        {
                            MVRCLogger.Log("Found an overlapping button");
                            AddUserInfoButton(buttonTrans);
                        }
                    }
                }
            } catch (Exception ex)
            {
                MVRCLogger.Log("Nearby buttons couldn't be found: " + ex.ToString());
            }
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
            LayoutElement element = button.gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = 170f;
            element.preferredHeight = 40f;
        }
        #endregion
    }
}
