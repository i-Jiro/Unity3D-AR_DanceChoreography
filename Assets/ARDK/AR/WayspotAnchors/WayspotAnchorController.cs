// Copyright 2022 Niantic, Inc. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;

using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Utilities.Logging;

using UnityEngine;

namespace Niantic.ARDK.AR.WayspotAnchors
{
  public class WayspotAnchorController
  {
    /// Called when the localization status has changed
    public ArdkEventHandler<LocalizationStateUpdatedArgs> LocalizationStateUpdated;

    /// Called when new anchors have been created
    public ArdkEventHandler<WayspotAnchorsCreatedArgs> WayspotAnchorsCreated;

    /// Called when wayspot anchors report a new position/rotation
    public ArdkEventHandler<WayspotAnchorsResolvedArgs> WayspotAnchorsTrackingUpdated;

    /// Called when the status of wayspot anchors has changed
    public ArdkEventHandler<WayspotAnchorStatusUpdatedArgs> WayspotAnchorStatusUpdated;

    private IARSession _arSession;
    private _IWayspotAnchorControllerImplementation _wayspotAnchorControllerImplementation;
    private LocalizationState _localizationState;

    /// Creates a new wayspot anchor API to consume
    /// @param arSession The AR session required by the WayspotAnchorController to run VPS.
    /// @param locationService The location service required by the WayspotAnchorController to run VPS.
    public WayspotAnchorController(IARSession arSession, ILocationService locationService)
    {
      _arSession = arSession;
      _arSession.SetupLocationService(locationService);
      _arSession.Deinitialized += HandleSessionDeinitialized;
      _wayspotAnchorControllerImplementation = CreateWayspotAnchorController();
    }

    /// Starts the virtual position system
    /// @param wayspotAnchorsConfiguration The configuration to start VPS with
    public void StartVps(IWayspotAnchorsConfiguration wayspotAnchorsConfiguration)
    {
      _wayspotAnchorControllerImplementation.StartVPS(wayspotAnchorsConfiguration);
      ARLog._Debug($"Started VPS for Stage ID: {_arSession.StageIdentifier}");
    }

    /// Stops the virtual position system
    /// @note This will reset the state and stop all pending creations and trackings
    public void StopVps()
    {
      _wayspotAnchorControllerImplementation.StopVPS();
    }

    /// Creates new wayspot anchors based on position and rotations
    /// @param localPoses The position and rotation used to create new wayspot anchors with
    /// @return The IDs of the newly created wayspot anchors
    public Guid[] CreateWayspotAnchors(params Matrix4x4[] localPoses)
    {
      if (_localizationState != LocalizationState.Localized)
      {
        ARLog._Error
        (
          $"Failed to create wayspot anchor, because the Localization State is {_localizationState}."
        );

        return Array.Empty<Guid>();
      }

      return _wayspotAnchorControllerImplementation.SendWayspotAnchorsCreateRequest(localPoses);
    }

    /// Pauses the tracking of wayspot anchors.  This can be used to conserve resources for wayspot anchors which you currently do not care about,
    /// but may again in the future
    /// @param wayspotAnchors The wayspot anchors to pause tracking for
    public void PauseTracking(params IWayspotAnchor[] wayspotAnchors)
    {
      _wayspotAnchorControllerImplementation.StopResolvingWayspotAnchors(wayspotAnchors);
      foreach (var wayspotAnchor in wayspotAnchors)
      {
        var trackable = ((_IInternalTrackable)wayspotAnchor);
        trackable.SetTrackingEnabled(false);
      }
    }

    /// Resumes the tracking of previously paused wayspot anchors
    /// @param wayspotAnchors The wayspot anchors to resume tracking for
    public void ResumeTracking(params IWayspotAnchor[] wayspotAnchors)
    {
      _wayspotAnchorControllerImplementation.StartResolvingWayspotAnchors(wayspotAnchors);
      foreach (var wayspotAnchor in wayspotAnchors)
      {
        var trackable = ((_IInternalTrackable)wayspotAnchor);
        trackable.SetTrackingEnabled(true);
      }
    }

    /// Restores previously created wayspot anchors via their payloads.
    /// @note
    ///   Anchors will have 'WayspotAnchorStatusCode.Pending' status, where its
    ///   Position and Rotation values are invalid, until they are resolved and
    ///   reach 'WayspotAnchorStatusCode.Success' status.
    /// @param wayspotAnchorPayloads The payloads of the wayspot anchors to restore
    /// @return The restored wayspot anchors
    public IWayspotAnchor[] RestoreWayspotAnchors(params WayspotAnchorPayload[] wayspotAnchorPayloads)
    {
      var wayspotAnchors = new List<IWayspotAnchor>();
      foreach (var wayspotAnchorPayload in wayspotAnchorPayloads)
      {
        byte[] blob = wayspotAnchorPayload._Blob;
        var wayspotAnchor = _WayspotAnchorFactory.Create(blob);
        wayspotAnchors.Add(wayspotAnchor);
      }

      return wayspotAnchors.ToArray();
    }

    private void HandleSessionDeinitialized(ARSessionDeinitializedArgs arSessionDeinitializedArgs)
    {
      _arSession.Deinitialized -= HandleSessionDeinitialized;
      _wayspotAnchorControllerImplementation.LocalizationStateUpdated -= HandleLocalizationStateUpdated;
      _wayspotAnchorControllerImplementation.WayspotAnchorsCreated -= HandleWayspotAnchorsCreated;
      _wayspotAnchorControllerImplementation.WayspotAnchorsResolved -= HandleWayspotAnchorsResolved;
      _wayspotAnchorControllerImplementation.WayspotAnchorStatusUpdated -= HandleWayspotAnchorStatusUpdated;

      _wayspotAnchorControllerImplementation.Dispose();
    }

    private _IWayspotAnchorControllerImplementation CreateWayspotAnchorController()
    {
      _IWayspotAnchorControllerImplementation impl;
      switch (_arSession.RuntimeEnvironment)
      {
        case RuntimeEnvironment.Default:
          // will never happen, Default value is invalid value for IARSession.RuntimeEnvironment
          ARLog._Error("Invalid ARSession.RuntimeEnvironment value");
          return null;

        case RuntimeEnvironment.Remote:
          throw new NotImplementedException($"Remote runtime environment not yet supported.");

        case RuntimeEnvironment.Playback:
        case RuntimeEnvironment.LiveDevice:
          impl = new _NativeWayspotAnchorControllerImplementation(_arSession);
          break;

        case RuntimeEnvironment.Mock:
          impl = new _MockWayspotAnchorControllerImplementation(_arSession);
          break;

        default:
          throw new InvalidEnumArgumentException($"Out of range RuntimeEnvironment enum: {_arSession.RuntimeEnvironment}.");
      }

      impl.LocalizationStateUpdated += HandleLocalizationStateUpdated;
      impl.WayspotAnchorsCreated += HandleWayspotAnchorsCreated;
      impl.WayspotAnchorsResolved += HandleWayspotAnchorsResolved;
      impl.WayspotAnchorStatusUpdated += HandleWayspotAnchorStatusUpdated;

      return impl;
    }

    private void HandleLocalizationStateUpdated(LocalizationStateUpdatedArgs args)
    {
      ARLog._DebugFormat
      (
        "VPS LocalizationState updated to {0} (reason: {1})",
        false,
        args.State,
        args.FailureReason
      );

      _localizationState = args.State;
      LocalizationStateUpdated?.Invoke(args);
    }

    private void HandleWayspotAnchorsCreated(WayspotAnchorsCreatedArgs args)
    {
      WayspotAnchorsCreated?.Invoke(args);
    }

    private void HandleWayspotAnchorsResolved(WayspotAnchorsResolvedArgs args)
    {
      foreach (var resolution in args.Resolutions)
      {
        var anchor = _WayspotAnchorFactory.GetOrCreateFromIdentifier(resolution.ID);
        ((_IInternalTrackable)anchor).SetTransform(resolution.Position, resolution.Rotation);
      }

      WayspotAnchorsTrackingUpdated?.Invoke(args);
    }

    private void HandleWayspotAnchorStatusUpdated(WayspotAnchorStatusUpdatedArgs args)
    {
      foreach (var statusUpdate in args.WayspotAnchorStatusUpdates)
      {
        var anchor = _WayspotAnchorFactory.GetOrCreateFromIdentifier(statusUpdate.ID);
        ((_IInternalTrackable)anchor).SetStatusCode(statusUpdate.Code);
      }

      WayspotAnchorStatusUpdated?.Invoke(args);
    }
  }
}
