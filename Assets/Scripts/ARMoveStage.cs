using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.ARDK.AR;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Utilities.Input.Legacy;

public class ARMoveStage : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    public ARHitTestResultType HitTestType = ARHitTestResultType.ExistingPlane;
    [SerializeField] private GameObject _danceStage;
    private IARSession _session;
    private bool _isPlacingStage;
    public delegate void PlacedDanceStageEventHandler();
    public event PlacedDanceStageEventHandler PlacedDanceStage;

    // Start is called before the first frame update
    void Start()
    {
        ARSessionFactory.SessionInitialized += OnAnyARSessionDidInitialized;
        _isPlacingStage = true;
    }

    private void OnAnyARSessionDidInitialized(AnyARSessionInitializedArgs args)
    {
        _session = args.Session;
        _session.Deinitialized += OnSessionDeinitialized;
    }

    private void OnSessionDeinitialized(ARSessionDeinitializedArgs args)
    {
        _danceStage.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        ARSessionFactory.SessionInitialized -= OnAnyARSessionDidInitialized;
        _session = null;
        _danceStage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_session == null) return;
        if (!_isPlacingStage) return;
        if (PlatformAgnosticInput.touchCount <= 0) return;
        var touch = PlatformAgnosticInput.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            MoveStage(touch);
        }
    }

    public void EnablePlacement()
    {
        _isPlacingStage = true;
    }

    void MoveStage(Touch touch)
    {
        var currentFrame = _session.CurrentFrame;
        if (currentFrame == null) return;

        var result = currentFrame.HitTest(_camera.pixelWidth, _camera.pixelHeight, touch.position, HitTestType);
        if (result.Count == 0) return;

        var closestHit = result[0];
        var hitPosition = closestHit.WorldTransform.ToPosition();
        var hitRotation = closestHit.WorldTransform.ToRotation();

        _danceStage.transform.position = hitPosition;
        _danceStage.transform.rotation = hitRotation;
        PlacedDanceStage?.Invoke();
        _isPlacingStage = false;
    }
    
}
