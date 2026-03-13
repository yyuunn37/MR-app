/*
================================
Assets for Unity by Makaka Games
================================
 
[Online  Docs -> Updated]: https://makaka.org/unity-assets
[Offline Docs - PDF file]: find it in the package folder.

[Support]: https://makaka.org/support

Copyright © 2025 Andrey Sirota (Makaka Games)
*/

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Management;

using System.Collections;

#if GOOGLE_CARDBOARD_XR_PLUGIN_EXISTS

using Google.XR.Cardboard;

#endif

using MakakaGames.Publisher.Debugging;

namespace MakakaGames.MRCamera
{
    /// <summary>
    /// Initializes Cardboard XR Plugin.
    /// </summary>
    [HelpURL("https://makaka.org/unity-assets")]
    public class CardboardStartupCustom : MonoBehaviour
    {
        [Space]
        [SerializeField]
        private UnityEvent OnRuntimeStartedInEditor = null;

        [Space]
        [SerializeField]
        private UnityEvent OnRuntimeStartedNotInEditor = null;

        [Space]
		[SerializeField]
		private UnityEvent OnXRStopped = null;

#if !GOOGLE_CARDBOARD_XR_PLUGIN_EXISTS

        private readonly string messageErrorOfInstallation =
            "Correct Version of Google Cardboard XR Plugin" +
            " is NOT INSTALLED — Learn PDF-file in the package" +
            " folder or the Latest Docs Online —" +
            " <a href=\"https://makaka.org/mr-camera\">" +
            "https://makaka.org/mr-camera</a>";

#endif

        private readonly string messageErrorOfXRInit =
            "Initializing XR Failed. Check Editor or Player log for details:" +
            " <a href=\"https://docs.unity3d.com/Manual/log-files.html\">" +
            "https://docs.unity3d.com/Manual/log-files.html</a>";

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public IEnumerator Start()
        {
            // Configures the app to not shut down the screen and sets the brightness to maximum.
            // Brightness control is expected to work only in iOS, see:
            // https://docs.unity3d.com/ScriptReference/Screen-brightness.html.
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.brightness = 1.0f;

#if !GOOGLE_CARDBOARD_XR_PLUGIN_EXISTS

            DebugPrinter.PrintError(messageErrorOfInstallation);

#endif

            // Manual Initializing of XR to allow Non-XR view before this scene
            // https://docs.unity3d.com/Packages/com.unity.xr.management@4.5/manual/EndUser.html
            // https://developers.google.com/cardboard/develop/unity/quickstart#turn_vr_mode_on_and_off
            
            if (Application.isEditor || XRGeneralSettings.Instance == null)
            {
                DebugPrinter.Print("XR will not be initialized — it's Editor" +
                    " or Plug-in Provider (Cardboard XR Plugin) is not" +
                    " selected in Edit > Project Settings > XR Plug-in Management");

                yield return null;
            }
            else
            {       
                DebugPrinter.Print("Initializing XR...");

                yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

                if (XRGeneralSettings.Instance.Manager.activeLoader == null)
                {
                    DebugPrinter.Print(messageErrorOfXRInit);
                }
                else
                {
                    DebugPrinter.Print("Starting XR...");

                    XRGeneralSettings.Instance.Manager.StartSubsystems();

#if GOOGLE_CARDBOARD_XR_PLUGIN_EXISTS

                    if (Api.HasNewDeviceParams())
                    {
                        Api.ReloadDeviceParams();
                    }

#endif

                }

            }

            if (Application.isEditor)
            {
                OnRuntimeStartedInEditor.Invoke();
            }
            else
            {
                OnRuntimeStartedNotInEditor.Invoke();                
            }
        }

        public void StopXR()
        {
            StopXRBase();

            OnXRStopped.Invoke();
        }

        private void StopXRBase()
        {
            if (!Application.isEditor)
            {
                DebugPrinter.Print("Stopping XR...");

                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();

                DebugPrinter.Print("XR stopped completely.");
            }
        }

        private void Update()
        {
            if (!Application.isEditor && 
                XRGeneralSettings.Instance != null &&
                XRGeneralSettings.Instance.Manager != null &&
                XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                UpdateNotInEditor();
            }
        }

        public void UpdateNotInEditor()
        {

#if GOOGLE_CARDBOARD_XR_PLUGIN_EXISTS

            if (Api.IsGearButtonPressed)
            {
                Api.ScanDeviceParams();
            }

            if (Api.IsCloseButtonPressed)
            {
                StopXR();

                return;
            }

            if (Api.IsTriggerHeldPressed)
            {
                Api.Recenter();
            }

            Api.UpdateScreenParams();

#endif

        }

    }
}