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

using MakakaGames.Publisher.Debugging;

namespace MakakaGames.Publisher.Movement
{
    [HelpURL("https://makaka.org/unity-assets")]
    public class TranslationByKeysControl : MonoBehaviour
    {
        [Header("X")]
        public bool isXAxisParent = false;
        public Transform xAxis;
        public float xAxisSpeed = 4f;

        [SerializeField]
        private float inputHorizontalGravity = 6f;

        [SerializeField]
        private float inputHorizontalSensitivity = 1.5f;

        private float inputHorizontal = 0f;
        private float inputHorizontalTarget = 0f;

        [Header("Y (Q & E keys)")]
        public bool isYAxisParent = false;
        public Transform yAxis;
        public float yAxisSpeed = 5f;

        [Header("Z")]
        public bool isZAxisParent = false;
        public Transform zAxis;
        public float zAxisSpeed = 4f;

        [Tooltip("Object for Vertical Rotation")]
        [SerializeField]
        private float inputVerticalGravity = 4f;

        [SerializeField]
        private float inputVerticalSensitivity = 1f;

        private float inputVertical = 0f;
        private float inputVerticalTarget = 0f;

        private Keyboard keyboard;

        private readonly Key[] positiveHorizontalKeys = new[] { Key.D, Key.RightArrow };
        private readonly Key[] negativeHorizontalKeys = new[] { Key.A, Key.LeftArrow };

        private readonly Key[] positiveVerticalKeys = new[] { Key.W, Key.UpArrow };
        private readonly Key[] negativeVerticalKeys = new[] { Key.S, Key.DownArrow };

        private void Start()
        {   
            keyboard = Keyboard.current;

            if (keyboard == null)
            {
                DebugPrinter.Print($"Keyboard not found.");
            }
        }

        private void LateUpdate()
        {
            if (keyboard != null)
            {
                // Determine target input values
                
                inputHorizontalTarget =
                    GetAxis(positiveHorizontalKeys, negativeHorizontalKeys);

                inputVerticalTarget =
                    GetAxis(positiveVerticalKeys, negativeVerticalKeys);

                // Smooth the inputs

                inputHorizontal = Mathf.MoveTowards(inputHorizontal,
                    inputHorizontalTarget * inputHorizontalSensitivity,
                    inputHorizontalGravity * Time.deltaTime);

                inputVertical = Mathf.MoveTowards(inputVertical,
                    inputVerticalTarget * inputVerticalSensitivity,
                    inputVerticalGravity * Time.deltaTime);

                // Apply Translation

                if (xAxis)
                {
                    (isXAxisParent ? xAxis.parent : xAxis).Translate(
                        inputHorizontal * xAxisSpeed * Time.deltaTime,
                        0f,
                        0f);
                }

                if (keyboard.qKey.isPressed)
                {
                    TranslateYAxis(-yAxisSpeed);
                }

                if (keyboard.eKey.isPressed)
                {
                    TranslateYAxis(yAxisSpeed);
                }

                if (zAxis)
                {
                    (isZAxisParent ? zAxis.parent : zAxis).Translate(
                        0f,
                        0f,
                        inputVertical * zAxisSpeed * Time.deltaTime);
                }
            }
        }


        private void TranslateYAxis(float speed)
        {
            if (yAxis)
            {
                (isYAxisParent ? yAxis.parent : yAxis).
                    Translate(0f, speed * Time.deltaTime, 0f);
            }
        }

        /// <summary>
        /// Returns an axis value in the range [-1, 1] by checking
        /// any number of positive and negative keys.
        /// </summary>
        private float GetAxis(Key[] positiveKeys, Key[] negativeKeys)
        {   
            bool positivePressed = false, negativePressed = false;

            foreach (var key in positiveKeys)
                positivePressed |= keyboard[key].isPressed;

            foreach (var key in negativeKeys)
                negativePressed |= keyboard[key].isPressed;

            return (positivePressed ? 1f : 0f) - (negativePressed ? 1f : 0f);
        }
    }
}