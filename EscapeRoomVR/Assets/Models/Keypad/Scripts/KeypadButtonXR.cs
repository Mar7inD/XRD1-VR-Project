using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace NavKeypad
{
    public class KeypadButtonXR : MonoBehaviour
    {
        [SerializeField] private XRSimpleInteractable interactable;
        private KeypadButton btn;

        void Reset()
        {
            interactable = GetComponent<XRSimpleInteractable>();
        }

        void Awake()
        {
            btn = GetComponent<KeypadButton>();
            
            if (interactable == null)
            {
                interactable = GetComponent<XRSimpleInteractable>();
            }
        }

        void OnEnable()
        {
            if (interactable != null)
            {
                interactable.selectEntered.AddListener(OnSelectEntered);
            }
        }

        void OnDisable()
        {
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(OnSelectEntered);
            }
        }

        void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (btn != null)
            {
                btn.PressButton();
            }
        }
    }
}