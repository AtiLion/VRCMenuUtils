using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace VRCMenuUtils
{
    internal class UserInfoActivityManager : MonoBehaviour
    {
        void OnDisable()
        {
            if (!VRCMenuUtilsAPI.IsIntialized)
                return;

            VRCMenuUtilsAPI.SetUserInfoUIState(false);
        }
    }
}
