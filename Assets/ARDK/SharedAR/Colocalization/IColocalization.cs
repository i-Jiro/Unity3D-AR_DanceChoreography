// Copyright 2022 Niantic, Inc. All Rights Reserved.
#if SHARED_AR_V2

using System;
using System.Collections.ObjectModel;

using Niantic.ARDK.AR.Networking;
using Niantic.ARDK.AR.Networking.ARNetworkingEventArgs;
using Niantic.ARDK.Utilities;
using Matrix4x4 = UnityEngine.Matrix4x4;

namespace Niantic.Experimental.ARDK.SharedAR.LLAPI {
  public enum ColocalizationState
  {
    Unknown = 0,
    Initialized,
    Colocalizing,
    Colocalized,
    LimitedTracking,
    Failed
  }

  public enum ColocalizationFailureReason
  {
    Unknown = 0,
    NetworkingError,
    VPSLocationFailed,
    VPSTimeout,
    VPSSpaceFailure
  }

  // WIP Names
  public interface IColocalization :
    IDisposable
  {
    // Start Colocalization
    void Start();

    // Stop colocalization 
    void Pause();

    ReadOnlyDictionary<IPeerID,ColocalizationState> ColocalizationStates { get; }
    ReadOnlyDictionary<IPeerID,Matrix4x4> LatestPeerPoses { get; }

    ColocalizationFailureReason FailureReason { get; }
    Matrix4x4 AlignedSpaceOrigin { get; }

    /// <summary>
    /// Fired upon any peers' (including self) localization state updating
    /// </summary>
    event ArdkEventHandler<ColocalizationStateUpdatedArgs> ColocalizationStateUpdated;

    event ArdkEventHandler<PeerPoseReceivedArgs> PeerPoseReceived;

    ColocalizationState ConvertToSharedSpace(Matrix4x4 poseInUnitySpace, out Matrix4x4 poseInSharedSpace);
    ColocalizationState ConvertToUnitySpace(Matrix4x4 poseInSharedSpace, out Matrix4x4 posInUnitySpace);
  }
}
#endif // SHARED_AR_V2
