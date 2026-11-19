using UnityEngine;
using UnityEngine.InputSystem;

namespace Kyle
{
    public class ShieldSpell : MonoBehaviour
    {
        [Header("Input Actions")]
        [Tooltip("Map to: <XRController>{RightHand}/devicePosition")]
        public InputActionProperty handPositionAction;

        [Tooltip("Map to: <XRController>{RightHand}/deviceRotation")]
        public InputActionProperty handRotationAction;

        [Tooltip("Map to: <XRController>{RightHand}/select or grip")]
        public InputActionProperty gripAction;

        [Header("Earth Bending Tuning")]
        public float upwardVelocityThreshold = 1.5f;
        public float palmUpTolerance = 0.5f;
        public GameObject rockPrefab;

        private bool isGrabbing = false;
        private Vector3 previousPosition;

        void Start()
        {

        }

        void Update()
        {
            Vector3 currentPosition = handPositionAction.action.ReadValue<Vector3>();
            Quaternion currentRotation = handRotationAction.action.ReadValue<Quaternion>();
            float gripValue = gripAction.action.ReadValue<float>();

            float velocityY = (currentPosition.y - previousPosition.y) / Time.deltaTime;
            previousPosition = currentPosition;

            Vector3 palmNormal = currentRotation * Vector3.up;
            bool isPalmUp = Vector3.Dot(palmNormal, Vector3.up) > palmUpTolerance;

            if (gripValue > 0.8f && isPalmUp && !isGrabbing)
            {
                isGrabbing = true;
            }
            else if (gripValue < 0.2f)
            {
                isGrabbing = false;
            }

            if (isGrabbing && velocityY > upwardVelocityThreshold)
            {
                CastEarthBend(currentPosition);

                isGrabbing = false;
            }
        }

        void CastEarthBend(Vector3 spawnPosition)
        {
            Debug.Log("Earth Bending Triggered!");
            if (rockPrefab != null)
            {
                Instantiate(rockPrefab, spawnPosition + (Vector3.down * 1.5f), Quaternion.identity);
            }
        }
    }
}

