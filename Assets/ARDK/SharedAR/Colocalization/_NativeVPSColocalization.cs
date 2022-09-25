// Copyright 2022 Niantic, Inc. All Rights Reserved.

#pragma warning disable 0067
#if SHARED_AR_V2

using System.Collections.ObjectModel;

using Niantic.ARDK.AR.Networking;
using Niantic.ARDK.AR.Networking.ARNetworkingEventArgs;
using Niantic.ARDK.Utilities;
using Matrix4x4 = UnityEngine.Matrix4x4;

namespace Niantic.Experimental.ARDK.SharedAR.LLAPI
{
  internal sealed class _NativeVPSColocalization :
    IColocalization
  {
    public void Start()
    {
      throw new System.NotImplementedException();
    }

    public void Pause()
    {
      throw new System.NotImplementedException();
    }

    public ReadOnlyDictionary<IPeerID, ColocalizationState> ColocalizationStates { get; }
    public ReadOnlyDictionary<IPeerID, Matrix4x4> LatestPeerPoses { get; }
    public ColocalizationFailureReason FailureReason { get; }
    public Matrix4x4 AlignedSpaceOrigin { get; }

    public event ArdkEventHandler<ColocalizationStateUpdatedArgs> ColocalizationStateUpdated;
    public event ArdkEventHandler<PeerPoseReceivedArgs> PeerPoseReceived;

    public ColocalizationState ConvertToSharedSpace(Matrix4x4 poseInUnitySpace, out Matrix4x4 poseInSharedSpace)
    {
      throw new System.NotImplementedException();
    }

    public ColocalizationState ConvertToUnitySpace(Matrix4x4 poseInSharedSpace, out Matrix4x4 posInUnitySpace)
    {
      throw new System.NotImplementedException();
    }

    public void Dispose()
    {
    }
  }
}

#endif
#pragma warning restore 0067