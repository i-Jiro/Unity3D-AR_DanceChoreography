#if SHARED_AR_V2

using System;
using System.Collections.Generic;
using System.IO;

using Niantic.ARDK.Networking;// TODO(kmori): remove later
using Niantic.ARDK.Networking.MultipeerNetworkingEventArgs; // TODO(kmori): remove later
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Utilities.BinarySerialization;

namespace Niantic.Experimental.ARDK.SharedAR
{
  /// <summary>
  /// 
  /// </summary>
  public struct NetworkingStats
  {
    // TODO(kmori): define this struct
    public int Rtt { get; set; }
    public float PacketLoss { get; set; }
  }

  /// <summary>
  /// 
  /// </summary>
  public enum NetworkingRequestResult
  {
    // TODO(kmori): Add more stats
    Success = 0,
    Error = 1
  };

  /// <summary>
  /// 
  /// </summary>
  public enum ConnectionState
  {
    Disconnected = 0,
    Connecting = 1,
    Connected = 2,
  };

  /// <summary>
  /// 
  /// </summary>
  public enum ConnectionEvents
  {
    Connected = 0,
    Disconnected = 1,
    ConnectionError = 2 // TODO(kmori): more detailed errors
  };

  /// <summary>
  /// 
  /// </summary>
  public enum ConnectionType
  {
    UseDefault = 0,
    Reliable = 1,
    Unreliable = 2
  };

  /// <summary>
  /// 
  /// </summary>
  public struct ConnectionStateArgs : IArdkEventArgs
  {
    public ConnectionState connectionState { get; private set; }
    public ConnectionStateArgs(ConnectionState state)
    {
      connectionState = state;
    }
  }

  public struct ConnectionEventArgs : IArdkEventArgs
  {
    public ConnectionEvents connectionEvent { get; private set; }
    public ConnectionEventArgs(ConnectionEvents connEvent)
    { connectionEvent = connEvent;}
  }

  /// <summary>
  /// 
  /// </summary>
  public struct PeerIDArgs : IArdkEventArgs
  {
    public IPeerID peerID { get; private set; }
    public PeerIDArgs(IPeerID peerid)
    {
      peerID = peerid;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public struct DataReceivedArgs : IArdkEventArgs
  {
    public IPeer Peer { get; private set; } // TODO(kmori): To be removed
    public IPeerID PeerID { get; private set; }
    public uint Tag { get; private set; }
    //public TransportType TransportType { get; private set; } // TODO(kmori): needed?
    public int DataLength { get { return _dataArgs.DataLength; } }
    private PeerDataReceivedArgs _dataArgs;

    public DataReceivedArgs(PeerDataReceivedArgs dataArgs)
    {
      Peer = dataArgs.Peer;
      PeerID = PeerIDv0.GetPeerID(Peer);
      Tag = dataArgs.Tag;
      _dataArgs = dataArgs;
    }

    public MemoryStream CreateDataReader() { return _dataArgs.CreateDataReader(); }
    public byte[] CopyData() { return _dataArgs.CopyData(); }
    public void GetData<T>(ref T data)
    {
      var ser = new BinaryDeserializer(_dataArgs.CreateDataReader());
      data = (T)ser.Deserialize();
    }
  }

  // The low level networking interface
  public interface INetworking :
    IDisposable
  {

    /// <summary>
    // 
    /// </summary>
    /// <param name="dest">default connection type</param>
    /// <returns></returns>
    void SetDefaultConnectionType(ConnectionType connectionType);

    /// <summary>
    // 
    /// </summary>
    /// <param name="dest">Destination of the message. destination could be peer ID, “server” peer ID, list of peer IDs, or empty for broadcast </param>
    /// <param name="tag"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    void SendData(List<IPeerID> dest, uint tag, byte[] data, ConnectionType connectionType);
    void SendData<T>(List<IPeerID> dest, uint tag, T data, ConnectionType connectionType=ConnectionType.UseDefault);

    /// <summary>
    /// Get the latest connection event
    /// </summary>
    /// <returns></returns>
    ConnectionState ConnectionState { get; }

    /// <summary>
    /// if self is a “server”
    /// </summary>
    /// <returns>true if this networking is server role, false if this networking is client role</returns>
    // TDOO(kmori): Remove once colocalization refactored not using this.
    bool IsServer { get; }

    /// <summary>
    /// Return the self Peer ID
    /// </summary>
    /// <returns>self Peer ID</returns>
    IPeerID SelfPeerID { get; }

    /// <summary>
    /// Return the server peer ID
    /// </summary>
    /// <returns>server Peer ID. Invalid peer ID if no server role in the current room</returns>
    // TDOO(kmori): Remove once colocalization refactored not using this.
    IPeerID ServerPeerId { get; }

    /// <summary>
    /// Get all PeerIDs actively connected to the room
    /// </summary>
    /// <returns>List of all Peer IDs actively connected to the room</returns>
    List<IPeerID> PeerIDs { get; }

    /// <summary>
    /// Make specified peer disconnected
    /// Can do only by server connection.
    /// TODO(kmori): Should be async?
    /// </summary>
    /// <param name="peerID">PeerID of the peer to be kicked out</param>
    /// <returns>result of the request</returns>
    NetworkingRequestResult KickOutPeer(IPeerID peerID);

    /// <summary>
    /// Get networking stats
    /// RTT, bps, packet loss, etc…
    /// </summary>
    /// <returns>current network stats struct</returns>
    NetworkingStats NetworkingStats { get; }

    /// <summary>
    /// Join the networking as a server
    /// </summary>
    void JoinAsServer(byte[] roomId);

    /// <summary>
    /// Join the networking as a peer
    /// </summary>
    void JoinAsPeer(byte[] roomId);

    /// <summary>
    /// disconnect from network and datastore
    /// </summary>
    void Leave();

    /// <summary>
    /// Get room config of the currrently connected room
    /// RoomConfig include name, ID, geo info, etc.
    /// </summary>
    /// <returns>the room current of the currrently connected room</returns>
    RoomConfig RoomConfig { get; }

    // connected, failed, disconnected, Deinitialized
    event ArdkEventHandler<ConnectionEventArgs> ConnectionEvent;

    event ArdkEventHandler<PeerIDArgs> PeerAdded;

    /// Event fired when a peer is removed, either from intentional action, timeout, or error.
    event ArdkEventHandler<PeerIDArgs> PeerRemoved;

    /// <summary>
    /// </summary>
    event ArdkEventHandler<DataReceivedArgs> DataReceived;

  }
} // namespace Niantic.ARDK.SharedAR

#endif // SHARED_AR_V2