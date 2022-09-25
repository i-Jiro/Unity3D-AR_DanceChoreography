// Copyright 2022 Niantic, Inc. All Rights Reserved.
#pragma warning disable 0067
#if SHARED_AR_V2

using System.Collections.Generic;
using Niantic.ARDK.Utilities;

namespace Niantic.Experimental.ARDK.SharedAR
{
  public sealed class _NativeNetworking :
    INetworking
  {
    private bool _isServer;
    private IPeerID _selfPeerID;
    private List<IPeerID> _peerIDs;
    public event ArdkEventHandler<ConnectionEventArgs> ConnectionEvent;
    public event ArdkEventHandler<PeerIDArgs> PeerAdded;
    public event ArdkEventHandler<PeerIDArgs> PeerRemoved;
    public event ArdkEventHandler<DataReceivedArgs> DataReceived;

    public void SetDefaultConnectionType(ConnectionType connectionType)
    {
      throw new System.NotImplementedException();
    }

    public void SendData(List<IPeerID> dest, uint tag, byte[] data, ConnectionType connectionType)
    {
      throw new System.NotImplementedException();
    }

    public void SendData<T>(List<IPeerID> dest, uint tag, T data,
      ConnectionType connectionType = ConnectionType.UseDefault)
    {
      throw new System.NotImplementedException();
    }

    public ConnectionState ConnectionState { get; }

    bool INetworking.IsServer => _isServer;

    IPeerID INetworking.SelfPeerID => _selfPeerID;

    public IPeerID ServerPeerId { get; }

    List<IPeerID> INetworking.PeerIDs => _peerIDs;

    public NetworkingRequestResult KickOutPeer(IPeerID peerID)
    {
      throw new System.NotImplementedException();
    }

    public NetworkingStats NetworkingStats { get; }

    public void JoinAsServer(byte[] roomId)
    {
      throw new System.NotImplementedException();
    }

    public void JoinAsPeer(byte[] roomId)
    {
      throw new System.NotImplementedException();
    }

    public void Leave()
    {
      throw new System.NotImplementedException();
    }

    public RoomConfig RoomConfig { get; }

    public void Dispose()
    {
    }
  }
}

#endif
#pragma warning restore 0067