
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
        }

        void OnEnable()
        {
            if (interactable != null) interactable.selectEntered.AddListener(OnSelectEntered);
        }

        void OnDisable()
        {
            if (interactable != null) interactable.selectEntered.RemoveListener(OnSelectEntered);
        }

        void OnSelectEntered(SelectEnterEventArgs args)
        {
            btn?.PressButton();
        }
    }
}