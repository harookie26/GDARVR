using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AnchorPlacer : MonoBehaviour
{
    ARAnchorManager anchorManager;
    ARRaycastManager raycastManager;

    [SerializeField] private GameObject anchorPrefab;
    [SerializeField] private float yOffset = 0.0f;

    static readonly List<ARRaycastHit> s_Hits = new();

    private void Start()
    {
        anchorManager = FindFirstObjectByType<ARAnchorManager>();
        raycastManager = FindFirstObjectByType<ARRaycastManager>();

        if (anchorManager == null)
            Debug.LogWarning("ARAnchorManager not found in scene. Anchors may not be tracked properly.");

        if (anchorPrefab == null)
            Debug.LogWarning("anchorPrefab is not assigned on AnchorPlacer.");
    }

    private void Update()
    {
        if (Input.touchCount == 0)
            return;

        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began)
            return;

        // Prefer AR raycast to planes
        if (raycastManager != null && raycastManager.Raycast(touch.position, s_Hits, TrackableType.Planes))
        {
            var hitPose = s_Hits[0].pose;
            // optionally apply yOffset relative to world up
            hitPose.position += Vector3.up * yOffset;
            AnchorObject(hitPose);
            return;
        }

        // Fallback: a world ray from camera (keeps original yOffset behavior)
        if (Camera.main != null)
        {
            Vector3 spawnPos = Camera.main.ScreenPointToRay(touch.position).GetPoint(yOffset);
            var pose = new Pose(spawnPos, Quaternion.identity);
            AnchorObject(pose);
        }
    }

    // Creates a tracked ARAnchor via ARAnchorManager and attaches the prefab to it.
    public async void AnchorObject(Pose pose)
    {
        if (anchorPrefab == null)
        {
            Debug.LogWarning("Cannot place anchor: anchorPrefab is null.");
            return;
        }

        if (anchorManager != null)
        {
            try
            {
                var awaitable = anchorManager.TryAddAnchorAsync(pose);
                var result = await awaitable; // ARFoundation Awaitable

                // Result has .status and .value on AR Foundation; check success and value
                if (result.status.IsSuccess() && result.value != null)
                {
                    ARAnchor createdAnchor = result.value;
                    GameObject obj = Instantiate(anchorPrefab, createdAnchor.transform);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    Debug.LogWarning($"TryAddAnchorAsync failed: {result.status}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Exception while creating anchor via ARAnchorManager: {ex.Message}");
            }

            return;
        }

        // If no ARAnchorManager available, fall back to creating a GameObject with ARAnchor component.
        GameObject newAnchor = new GameObject("Anchor");
        newAnchor.transform.parent = null;
        newAnchor.transform.position = pose.position;
        newAnchor.transform.rotation = pose.rotation;
        newAnchor.AddComponent<ARAnchor>();

        GameObject instantiated = Instantiate(anchorPrefab, newAnchor.transform);
        instantiated.transform.localPosition = Vector3.zero;
        instantiated.transform.localRotation = Quaternion.identity;
    }
}