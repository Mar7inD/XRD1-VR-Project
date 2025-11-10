using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LockSocketXR : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor socketInteractor;
    [SerializeField] private Chest chest;
    [SerializeField] private string requiredKeyID = "ChestKey";
    [SerializeField] private bool destroyKeyOnUnlock = true;
    [SerializeField] private float destroyDelay = 0.5f;
    [SerializeField] private bool destroyLockOnUnlock = true;
    [SerializeField] private float lockDestroyDelay = 1f;
    [SerializeField] private AudioClip insertSound;
    [SerializeField] private AudioClip unlockSound;
    
    private AudioSource audioSource;
    private bool isUnlocked = false;
    
    void Reset()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
    }
    
    void Start()
    {
        if (socketInteractor == null)
        {
            socketInteractor = GetComponent<XRSocketInteractor>();
        }
        
        if (socketInteractor == null)
        {
            return;
        }
        
        
        if (chest == null)
        {
            chest = GetComponentInParent<Chest>();
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
    }
    
    void OnEnable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.AddListener(OnKeyInserted);
        }
    }
    
    void OnDisable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.RemoveListener(OnKeyInserted);
        }
    }
    
    void OnKeyInserted(SelectEnterEventArgs args)
    {
        
        // Check if it's the correct key
        KeyXR key = args.interactableObject.transform.GetComponent<KeyXR>();

        if (key == null)
        {
            return;
        }
        
        if (audioSource != null && insertSound != null)
        {
            audioSource.PlayOneShot(insertSound);
        }
        
        if (key.KeyID == requiredKeyID && !isUnlocked)
        {
            
            UnlockChest(key.gameObject);
        }
    }
    
    void UnlockChest(GameObject keyObject)
    {
        isUnlocked = true;
        
        // Play sounds
        
        if (audioSource != null && unlockSound != null)
        {
            audioSource.PlayOneShot(unlockSound);
        }
        
        // Open the chest
        if (chest != null)
        {
            chest.Open();
        }
        
        // Destroy the key after a delay
        if (destroyKeyOnUnlock && keyObject != null)
        {
            Destroy(keyObject, destroyDelay);
        }
        
        // Disable the socket
        socketInteractor.enabled = false;
        
        // Destroy the lock after a delay
        if (destroyLockOnUnlock)
        {
            Destroy(gameObject, lockDestroyDelay);
        }
    }
}