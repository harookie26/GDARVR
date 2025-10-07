using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageObjectHandler : MonoBehaviour
{
    [Header("Prefab to spawn over detected image")]
    [SerializeField] private GameObject virtualObjectPrefab;
    [SerializeField] private float yOffset = 0.05f;
    [SerializeField] private bool scaleToImageSize = false;
    [SerializeField] private bool verboseLogging = true;

    private ARTrackedImageManager _imageManager;
    private readonly Dictionary<TrackableId, GameObject> _spawned = new();

    private void Awake()
    {
        _imageManager = GetComponent<ARTrackedImageManager>();
        Log($"Awake. ReferenceLibrary assigned? {_imageManager.referenceLibrary != null}");
    }

    private void OnEnable()
    {
        if (_imageManager == null)
        {
            LogError("ARTrackedImageManager missing.");
            enabled = false;
            return;
        }
        _imageManager.trackablesChanged.AddListener(OnTrackablesChanged);
        Log("Subscribed to trackablesChanged (generic, non‑obsolete).");
    }

    private void OnDisable()
    {
        if (_imageManager != null)
            _imageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }

    private void Start()
    {
        if (virtualObjectPrefab == null)
            LogError("virtualObjectPrefab not assigned.");
        if (_imageManager.referenceLibrary == null)
            LogError("ReferenceLibrary is NULL at runtime.");
    }

    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        HandleAdded(args.added);
        HandleUpdated(args.updated);
        HandleRemoved(args.removed);
    }

    private void HandleAdded(IReadOnlyList<ARTrackedImage> added)
    {
        foreach (var trackedImage in added)
        {
            Log($"Added: {trackedImage.referenceImage.name} state={trackedImage.trackingState}");
            if (virtualObjectPrefab == null) continue;
            if (_spawned.ContainsKey(trackedImage.trackableId)) continue;

            var instance = Instantiate(
                virtualObjectPrefab,
                trackedImage.transform.position + Vector3.up * yOffset,
                trackedImage.transform.rotation);

            instance.name = $"Spawn_{trackedImage.referenceImage.name}_{trackedImage.trackableId}";
            instance.transform.SetParent(trackedImage.transform, worldPositionStays: true);

            if (scaleToImageSize)
            {
                var size = trackedImage.size;
                float uniform = Mathf.Min(size.x, size.y);
                instance.transform.localScale = Vector3.one * uniform;
            }

            _spawned[trackedImage.trackableId] = instance;
            UpdateVisibility(trackedImage);
        }
    }

    private void HandleUpdated(IReadOnlyList<ARTrackedImage> updated)
    {
        foreach (var trackedImage in updated)
        {
            if (_spawned.TryGetValue(trackedImage.trackableId, out var instance))
            {
                instance.transform.position = trackedImage.transform.position + Vector3.up * yOffset;
                instance.transform.rotation = trackedImage.transform.rotation;
                UpdateVisibility(trackedImage);
            }
        }
    }

    private void HandleRemoved(IReadOnlyList<KeyValuePair<TrackableId, ARTrackedImage>> removed)
    {
        foreach (var pair in removed)
        {
            var id = pair.Key;
            var trackedImage = pair.Value;
            Log($"Removed: {(trackedImage != null ? trackedImage.referenceImage.name : id.ToString())}");
            if (_spawned.TryGetValue(id, out var instance))
            {
                Destroy(instance);
                _spawned.Remove(id);
            }
        }
    }

    private void UpdateVisibility(ARTrackedImage trackedImage)
    {
        if (_spawned.TryGetValue(trackedImage.trackableId, out var instance))
        {
            bool visible = trackedImage.trackingState == TrackingState.Tracking;
            if (instance.activeSelf != visible)
                instance.SetActive(visible);
        }
    }

    private void Log(string msg)
    {
        if (verboseLogging)
            Debug.Log($"[ImageTracking] {msg}");
    }

    private void LogError(string msg)
    {
        Debug.LogError($"[ImageTracking] {msg}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        foreach (var kvp in _spawned)
        {
            if (kvp.Value != null)
                Gizmos.DrawWireCube(kvp.Value.transform.position, Vector3.one * 0.02f);
        }
    }
}