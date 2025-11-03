using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlight;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isFlashlightOn = false;

    // Registered texts currently inside this trigger
    private HashSet<TextMeshPro> registeredTexts = new HashSet<TextMeshPro>();

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
            isFlashlightOn = flashlight.gameObject.activeSelf;
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
        flashlight.gameObject.SetActive(isFlashlightOn);
        Debug.Log($"[FlashlightToggle] Light is now: {(isFlashlightOn ? "ON" : "OFF")}");

        // Update all registered texts to reflect new flashlight state
        if (registeredTexts.Count > 0)
        {
            // copy to list to avoid potential modification during iteration
            var texts = new List<TextMeshPro>(registeredTexts);
            foreach (var t in texts)
            {
                if (t == null)
                {
                    registeredTexts.Remove(t);
                    continue;
                }
                SetTextAlpha(t, isFlashlightOn ? 1f : 0f);
            }
        }

        // Optional: update any reactive text currently in immediate range as well
        UpdateReactiveTextInRange(isFlashlightOn);
    }

    // When this object's trigger touches a ReactiveText collider, register it and reveal/hide based on flashlight state
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[FlashlightToggle] OnTriggerEnter called with: {other.name} tag: {other.tag}");
        if (!other.CompareTag("ReactiveText")) return;

        Debug.Log("[FlashlightToggle] Entered trigger with ReactiveText");
        var text = other.GetComponent<TextMeshPro>();
        if (text == null) return;

        // register
        registeredTexts.Add(text);

        // show only if flashlight is on, otherwise remain hidden
        SetTextAlpha(text, isFlashlightOn ? 1f : 0f);
        Debug.Log($"[FlashlightToggle] ReactiveText registered and set to alpha {(isFlashlightOn ? "1" : "0")}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("ReactiveText")) return;

        var text = other.GetComponent<TextMeshPro>();
        if (text == null) return;

        // unregister and hide
        registeredTexts.Remove(text);
        SetTextAlpha(text, 0f); // hide when leaving trigger
        Debug.Log("[FlashlightToggle] ReactiveText unregistered and hidden on exit");
    }

    // Helper to set alpha safely
    private void SetTextAlpha(TextMeshPro text, float alpha)
    {
        Color c = text.color;
        c.a = Mathf.Clamp01(alpha);
        text.color = c;
    }

    // Optional: when toggling the flashlight, reveal/hide any nearby ReactiveText colliders overlapping this object
    private void UpdateReactiveTextInRange(bool reveal)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("ReactiveText")) continue;
            var text = hit.GetComponent<TextMeshPro>();
            if (text == null) continue;
            SetTextAlpha(text, reveal ? 1f : 0f);
        }
    } 
}