using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlight;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isFlashlightOn = false;

    // Start is called before the first frame update
    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        if (grabInteractable == null)
        {
            Debug.LogError("[FlashlightToggle] XRGrabInteractable component not found!");
            return;
        }

        if (flashlight == null)
        {
            Debug.LogError("[FlashlightToggle] Light not assigned in Inspector!");
        }
        else
        {
            Debug.Log($"[FlashlightToggle] Light found: {flashlight.name}, initial state: {flashlight.enabled}");
        }

        grabInteractable.activated.AddListener(ToggleFlashlight);
        Debug.Log("[FlashlightToggle] Script initialized and listener added");
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.activated.RemoveListener(ToggleFlashlight);
    }

    private void ToggleFlashlight(ActivateEventArgs args)
    {
        Debug.Log("[FlashlightToggle] ToggleFlashlight called!");
        
        if (flashlight == null)
        {
            Debug.LogError("[FlashlightToggle] Light is null!");
            return;
        }

        isFlashlightOn = !isFlashlightOn;
        flashlight.enabled = isFlashlightOn;
        Debug.Log($"[FlashlightToggle] Light is now: {(isFlashlightOn ? "ON" : "OFF")}");
    }
        
}