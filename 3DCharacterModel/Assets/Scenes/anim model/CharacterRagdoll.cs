using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterRagdoll : MonoBehaviour
{
    public CharacterController characterController;

    // every bone Rigidbody on the character
    private Rigidbody[] boneRigidbodies;

    private Animator animator;

    // The movement script
    private StarterAssets.ThirdPersonController movementScript;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Vector3[] boneStartPositions;
    private Quaternion[] boneStartRotations;

    private void Awake()
    {
        // Find all Rigidbodies on the character and its children
        boneRigidbodies = GetComponentsInChildren<Rigidbody>();

        animator = GetComponentInChildren<Animator>();

        // assign starting position and bone poses for respawn later
        movementScript = GetComponent<StarterAssets.ThirdPersonController>();
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        boneStartPositions = new Vector3[boneRigidbodies.Length];
        boneStartRotations = new Quaternion[boneRigidbodies.Length];
        for (int i = 0; i < boneRigidbodies.Length; i++)
        {
            boneStartPositions[i] = boneRigidbodies[i].transform.localPosition;
            boneStartRotations[i] = boneRigidbodies[i].transform.localRotation;
        }

        // Start with ragdoll OFF
        SetRagdollActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Activate();
        }

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            Respawn();
        }
    }

    public void Activate()
    {
        SetRagdollActive(true);
    }

    public void Respawn()
    {
        // Stop every bone from moving
        foreach (Rigidbody rb in boneRigidbodies)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Put every bone back in its starting pose
        for (int i = 0; i < boneRigidbodies.Length; i++)
        {
            boneRigidbodies[i].transform.localPosition = boneStartPositions[i];
            boneRigidbodies[i].transform.localRotation = boneStartRotations[i];
        }

        // Move back to the spawn point
        if (characterController != null) {
            characterController.enabled = false;
        }

        transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        // Back to normal mode
        SetRagdollActive(false);
    }

    private void SetRagdollActive(bool isActive)
    {
        // Turn off the animator when ragdolling (animation and physics fight otherwise)
        if (animator != null) animator.enabled = !isActive;

        // Turn off the CharacterController when ragdolling (its capsule shoves the bones around otherwise)
        if (characterController != null) characterController.enabled = !isActive;

        // Turn off the movement script when ragdolling (it keeps calling CharacterController.Move otherwise)
        if (movementScript != null) movementScript.enabled = !isActive;

        // Flip every bone's kinematic state
        foreach (Rigidbody rb in boneRigidbodies)
        {
            rb.isKinematic = !isActive;
        }
    }
}
