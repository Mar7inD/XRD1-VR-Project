using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KeyXR : MonoBehaviour
{
    [SerializeField] private string keyID = "ChestKey";
    [SerializeField] private XRGrabInteractable grabInteractable;
    
    public string KeyID => keyID;
    
    void Reset()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }
    
    void Start()
    {
        
    }
}