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
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

using TMPro;

using MakakaGames.Publisher.Debugging;

namespace MakakaGames.MRCamera
{
    /// <summary>
    /// Sends messages to gazed GameObject.
    /// </summary>
    [HelpURL("https://makaka.org/unity-assets")]
    public class CameraPointerCustom : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI reticleByDefault = null;

        [SerializeField]
        private TextMeshProUGUI reticleDetected = null;

        [SerializeField]
        private float _maxDistance = 10f;

        [SerializeField]
        private bool isDebugLogging = false;

        private GameObject _gazedAtObject = null;

        /// <summary>
        /// Mask used to indicate interactive objects.
        /// </summary>
        public LayerMask ReticleInteractionLayerMask = 1 << _RETICLE_INTERACTION_DEFAULT_LAYER;

        /// <summary>
        /// Default layer for interactive game objects.
        /// </summary>
        private const int _RETICLE_INTERACTION_DEFAULT_LAYER = 8;

        /// <summary>
        /// It's needed for Worlds Space UI.
        /// Null Data Beacuse in VR we need only the fact of tap to screen.
        /// </summary>
        private PointerEventData pointerEventData;

        private void Awake()
        {
            pointerEventData = new PointerEventData(null);
        }

        private void Update()
        {
            // Casts ray towards camera's forward direction,
            //to detect if a GameObject is being gazed at.
            RaycastHit hit;

            if (Physics.Raycast(
                transform.position, transform.forward, out hit, _maxDistance))
            {
                // GameObject detected in front of the camera.
                if (_gazedAtObject != hit.transform.gameObject
                    && IsInteractive(_gazedAtObject))
                {
                    if (isDebugLogging)
                    {
                        DebugPrinter.Print("VR: GameObject detected.");
                    }

                    _gazedAtObject?.SendMessage(
                        "OnPointerExit",
                        pointerEventData,
                        SendMessageOptions.DontRequireReceiver);

                    _gazedAtObject = hit.transform.gameObject;

                    _gazedAtObject.SendMessage(
                        "OnPointerEnter",
                        pointerEventData,
                        SendMessageOptions.DontRequireReceiver);

                    SetReticleDetected(true);
                }
            }
            else if (IsInteractive(_gazedAtObject))
            {
                if (isDebugLogging)
                {
                    DebugPrinter.Print("VR: !!! No GameObject detected.");
                }

                _gazedAtObject?.SendMessage(
                    "OnPointerExit",
                    pointerEventData,
                    SendMessageOptions.DontRequireReceiver);

                _gazedAtObject = null;

                SetReticleDetected(false);
            }

#if UNITY_EDITOR

            // Checks for mouse touches.
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {

#elif GOOGLE_CARDBOARD_XR_PLUGIN_EXISTS

            // Checks for screen touches.
            if (Google.XR.Cardboard.Api.IsTriggerPressed)
            {
            
#endif

#if UNITY_EDITOR || GOOGLE_CARDBOARD_XR_PLUGIN_EXISTS

                if (isDebugLogging)
                {
                    DebugPrinter.Print("VR: OnPointerClick.");
                }

                if (IsInteractive(_gazedAtObject))
                {
                    _gazedAtObject?.SendMessage(
                        "OnPointerClick",
                        pointerEventData,
                        SendMessageOptions.DontRequireReceiver);
                }
            }

#endif

        }

        private void SetReticleDetected(bool isDetected)
        {
            reticleByDefault.enabled = !isDetected;

            reticleDetected.enabled = isDetected;
        }

        public void PrintTest(string message)
        {
            if (isDebugLogging)
            {
                DebugPrinter.Print(message);
            }
        }

        /// <summary>
        /// Evaluates if the provided GameObject is interactive based on its layer.
        /// </summary>
        ///
        /// <param name="gameObject">The game object on which to check if its layer is interactive.</param>
        ///
        /// <returns>Whether or not a GameObject's layer is interactive.</returns>
        private bool IsInteractive(GameObject gameObject)
        {
            return (1 << gameObject?.layer & ReticleInteractionLayerMask) != 0;
        }
    }
}