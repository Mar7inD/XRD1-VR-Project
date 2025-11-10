using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Button))]
public class VRButton : MonoBehaviour
{
    private Button button;
    private XRSimpleInteractable interactable;

    void Awake()
    {
        button = GetComponent<Button>();
        
        // Add XR Simple Interactable if not present
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
        {
            interactable = gameObject.AddComponent<XRSimpleInteractable>();
        }

        // Add collider if not present
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            BoxCollider boxCol = gameObject.AddComponent<BoxCollider>();
            // Match collider to button size
            RectTransform rect = GetComponent<RectTransform>();
            if (rect != null)
            {
                boxCol.size = new Vector3(rect.rect.width, rect.rect.height, 10f);
            }
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
        // Trigger the button click
        if (button != null && button.interactable)
        {
            button.onClick.Invoke();
        }
    }
}