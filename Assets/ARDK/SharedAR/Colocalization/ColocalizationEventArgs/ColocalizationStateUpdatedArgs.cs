// Copyright 2022 Niantic, Inc. All Rights Reserved.

using Niantic.ARDK.Networking;
using Niantic.ARDK.Utilities;

#if SHARED_AR_V2

namespace Niantic.Experimental.ARDK.SharedAR.LLAPI
{
  public struct ColocalizationStateUpdatedArgs:
    IArdkEventArgs
  {
    public ColocalizationStateUpdatedArgs(IPeer peer, ColocalizationState state):
      this()
    {
      Peer = peer;
      State = state;
    }
    
    public IPeer Peer { get; private set; }
    public ColocalizationState State { get; private set; }
  }
}
#endif