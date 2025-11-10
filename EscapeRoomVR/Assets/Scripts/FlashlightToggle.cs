using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlight;
    public AudioClip flashlightOnSound;
    public AudioClip flashlightOffSound;
    private AudioSource audioSource;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isFlashlightOn = false;

    // Registered texts currently inside this trigger
    private HashSet<TextMeshPro> registeredTexts = new HashSet<TextMeshPro>();

    // Start is called before the first frame update
    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
        
        if (grabInteractable == null)
        {
            return;
        }

        if (flashlight != null)
        {
            isFlashlightOn = flashlight.gameObject.activeSelf;
        }

        grabInteractable.activated.AddListener(ToggleFlashlight);
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.activated.RemoveListener(ToggleFlashlight);
    }

    private void ToggleFlashlight(ActivateEventArgs args)
    {
        
        if (flashlight == null)
        {
            return;
        }

        isFlashlightOn = !isFlashlightOn;
        flashlight.gameObject.SetActive(isFlashlightOn);

        // Play sound based on flashlight state
        PlayToggleSound(isFlashlightOn);

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

        UpdateReactiveTextInRange(isFlashlightOn);
    }

    private void PlayToggleSound(bool turnedOn)
    {
        if (audioSource == null) return;

        AudioClip clipToPlay = turnedOn ? flashlightOnSound : flashlightOffSound;
        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }

    // When this object's trigger touches a ReactiveText collider, register it and reveal/hide based on flashlight state
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("ReactiveText")) return;

        var text = other.GetComponent<TextMeshPro>();
        if (text == null) return;

        // register
        registeredTexts.Add(text);

        // show only if flashlight is on, otherwise remain hidden
        SetTextAlpha(text, isFlashlightOn ? 1f : 0f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("ReactiveText")) return;

        var text = other.GetComponent<TextMeshPro>();
        if (text == null) return;

        // unregister and hide
        registeredTexts.Remove(text);
        SetTextAlpha(text, 0f);
    }

    private void SetTextAlpha(TextMeshPro text, float alpha)
    {
        Color c = text.color;
        c.a = Mathf.Clamp01(alpha);
        text.color = c;
    }

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