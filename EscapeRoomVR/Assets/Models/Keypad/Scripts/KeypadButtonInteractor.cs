using UnityEngine;

namespace NavKeypad
{
    public class KeypadButtonInteractor : MonoBehaviour
    {
        [Tooltip("Origin transform to cast the ray from (controller tip / camera)")]
        public Transform rayOrigin;
        [Tooltip("Button to press (mouse/keyboard for editor)")]
        public KeyCode fireKey = KeyCode.Mouse0;
        public float maxDistance = 5f;
        public LayerMask interactableMask = ~0;

        void Reset()
        {
            if (rayOrigin == null && Camera.main != null) rayOrigin = Camera.main.transform;
        }

        void Update()
        {
            if (rayOrigin == null) return;

            if (Input.GetKeyDown(fireKey))
            {
                if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out var hit, maxDistance, interactableMask))
                {
                    var btn = hit.collider.GetComponentInParent<KeypadButton>();
                    btn?.PressButton();
                }
            }
        }
    }
}