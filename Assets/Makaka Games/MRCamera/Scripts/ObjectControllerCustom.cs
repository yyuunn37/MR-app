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

namespace MakakaGames.MRCamera
{
    /// <summary>
    /// Controls target objects behaviour.
    /// </summary>
    [HelpURL("https://makaka.org/unity-assets")]
    public class ObjectControllerCustom : MonoBehaviour
    {
        /// <summary>
        /// The material to use when this object is inactive (not being gazed at).
        /// </summary>
        public Material InactiveMaterial;

        /// <summary>
        /// The material to use when this object is active (gazed at).
        /// </summary>
        public Material GazedAtMaterial;

        private const float _minObjectDistance = 3.0f;
        // The objects are about 1 meter in radius, so the min/max target distance are
        // set so that the objects are always within the room (which is about 5 meters
        // across).
        private const float _maxObjectDistance = 3.5f;
        private const float _minObjectHeight = 0.5f;
        private const float _maxObjectHeight = 3.5f;

        [SerializeField]
        private Renderer renderer3D;

        [Space]
		[SerializeField]
		private UnityEvent OnClick;

        private void Start()
        {
            SetMaterial(false);
        }

        /// <summary>
        /// Teleports this instance randomly when triggered by a pointer click.
        /// </summary>
        public void TeleportRandomly()
        {
            
            // New object's location.
            float angle = Random.Range(-90, 90);
            float distance = Random.Range(_minObjectDistance, _maxObjectDistance);
            float height = Random.Range(_minObjectHeight, _maxObjectHeight);
            
            Vector3 newPos = new(
                Mathf.Cos(angle) * distance, height, Mathf.Sin(angle) * distance);
        
            transform.localPosition = newPos;

            SetMaterial(false);
        }

        /// <summary>
        /// This method is called by the Main Camera when it starts gazing at this GameObject.
        /// </summary>
        public void OnPointerEnter()
        {
            SetMaterial(true);
        }

        /// <summary>
        /// This method is called by the Main Camera when it stops gazing at this GameObject.
        /// </summary>
        public void OnPointerExit()
        {
            SetMaterial(false);
        }

        /// <summary>
        /// This method is called by the Main Camera when it is gazing at this GameObject and the screen
        /// is touched.
        /// </summary>
        public void OnPointerClick()
        {
            OnClick.Invoke();
        }

        /// <summary>
        /// Sets this instance's material according to gazedAt status.
        /// </summary>
        ///
        /// <param name="gazedAt">
        /// Value `true` if this object is being gazed at, `false` otherwise.
        /// </param>
        private void SetMaterial(bool gazedAt)
        {
            if (InactiveMaterial != null && GazedAtMaterial != null)
            {
                renderer3D.material = gazedAt ? GazedAtMaterial : InactiveMaterial;
            }
        }
    }
}