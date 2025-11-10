using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private ParticleSystem openParticles;
    
    [Header("Animation Control")]
    [SerializeField] private string animationStateName = "Entry";
    [SerializeField] private float openSpeed = 1f; 
    [SerializeField] private bool canPause = true;
    [SerializeField] private bool canReverse = true;
    
    [Header("Stop at Specific Point")]
    [SerializeField] private bool stopAtPoint = true; 
    [SerializeField] private float stopAtNormalizedTime = 0.5f; 
    
    private AudioSource audioSource;
    private bool isOpen = false;
    private bool isPaused = false;
    private float currentAnimationTime = 0f;
    private Coroutine animationCoroutine;
    
    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }
        
        // Disable animator initially so it doesn't play on start
        if (animator != null)
        {
            animator.enabled = false;
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    public void Open()
    {
        if (isOpen)
        {
            return;
        }
        
        isOpen = true;
        isPaused = false;

        // Enable animator to play the entry animation
        if (animator != null)
        {
            animator.enabled = true;
            animator.speed = openSpeed;

            // Start coroutine to monitor animation progress
            if (stopAtPoint)
            {
                if (animationCoroutine != null)
                {
                    StopCoroutine(animationCoroutine);
                }
                animationCoroutine = StartCoroutine(MonitorAnimationProgress());
            }
        }
        
        // Play sound
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
        
        // Play particles
        if (openParticles != null)
        {
            openParticles.Play();
        }
    }
    
    private IEnumerator MonitorAnimationProgress()
    {
        // Wait one frame to ensure animation has started
        yield return null;
        
        while (animator.enabled)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float progress = stateInfo.normalizedTime;
            
            // Check if we've reached the target point
            if (progress >= stopAtNormalizedTime)
            {
                // Stop at the exact point
                animator.speed = 0f;
                animator.Play(stateInfo.fullPathHash, 0, stopAtNormalizedTime);
                
                yield break;
            }
            
            yield return null;
        }
    }
    
    public void OpenToPoint(float normalizedTime)
    {
        // Open to a specific point (0 to 1)
        stopAtNormalizedTime = Mathf.Clamp01(normalizedTime);
        stopAtPoint = true;
        Open();
    }
    
    public void OpenHalfway()
    {
        // Convenience method to open to 50%
        OpenToPoint(0.5f);
    }
    
    public void OpenFully()
    {
        // Open completely without stopping
        stopAtPoint = false;
        Open();
    }
    
    public void ContinueOpening()
    {
        // Continue animation from current position
        if (animator != null && animator.enabled)
        {
            stopAtPoint = false;
            animator.speed = openSpeed;
        }
    }
    
    public void Close()
    {
        if (!isOpen)
        {
            return;
        }
        
        isOpen = false;
        isPaused = false;
        
        // Stop any monitoring coroutine
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        if (animator != null)
        {
            // Reverse the animation
            animator.speed = -openSpeed;
            animator.enabled = true;
        }
    }
    
    public void PauseAnimation()
    {
        if (!canPause || !animator.enabled) return;
        
        isPaused = !isPaused;
        
        if (isPaused)
        {
            animator.speed = 0f;
        }
        else
        {
            animator.speed = openSpeed;
        }
    }
    
    public void StopAnimation()
    {
        if (animator != null)
        {
            animator.enabled = false;
        }
        
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
    }
    
    public void SetAnimationProgress(float normalizedTime)
    {
        // Set animation to specific point (0 = start, 1 = end)
        if (animator != null)
        {
            if (!animator.enabled)
            {
                animator.enabled = true;
            }
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            animator.Play(stateInfo.fullPathHash, 0, Mathf.Clamp01(normalizedTime));
            animator.speed = 0;
        }
    }
    
    public void SetAnimationSpeed(float speed)
    {
        openSpeed = speed;
        if (animator != null && animator.enabled)
        {
            animator.speed = speed;
        }
    }
    
    public float GetAnimationProgress()
    {
        if (animator != null && animator.enabled)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime;
        }
        return 0f;
    }
    
    public bool IsAnimationPlaying()
    {
        if (animator != null && animator.enabled)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime < 1f && animator.speed != 0;
        }
        return false;
    }
}