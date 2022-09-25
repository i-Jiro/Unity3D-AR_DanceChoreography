#if SHARED_AR_V2

using System;
using System.Collections;
using System.Collections.Generic;

using Niantic.ARDK.Networking;

namespace Niantic.Experimental.ARDK.SharedAR
{

  public class PeerIDv0 : IPeerID
  {
    // static members
    private static Dictionary<string, IPeer> _peers;

    static PeerIDv0()
    {
      _peers = new Dictionary<string, IPeer>();
    }

    public static IPeerID GetPeerID(IPeer peer)
    {
      return GetPeerID(peer.ToString());
    }

    public static IPeerID GetPeerID(string stringID)
    {
      if (_peers.ContainsKey(stringID))
      {
        return new PeerIDv0(_peers[stringID]);
      }
      else
      {
        return null;
      }
    }

    internal static IPeer GetPeer(IPeerID peerID)
    {
      if (_peers.ContainsKey(peerID.ToString()))
      {
        return _peers[peerID.ToString()];
      }
      else
      {
        return null;
      }
    }

    //
    private IPeer _ipeer;

    internal PeerIDv0(IPeer ipeer)
    {
      _ipeer = ipeer;
      if (!_peers.ContainsKey(ipeer.ToString()))
      {
        _peers.Add(ipeer.ToString(), ipeer);
      }
    }

    // Implementing IPeerID
    public override string ToString()
    {
      return _ipeer.ToString();
    }

    public Guid ToGUID()
    {
      return _ipeer.Identifier;
    }

    public override int GetHashCode()
    {
      return _ipeer.GetHashCode();
    }

    // Implementing IEqualable
    public bool Equals(IPeerID info)
    {
      return info != null && _ipeer.Identifier.Equals(info.ToGUID());
    }

    public override bool Equals(object obj)
    {
      return Equals(obj as IPeerID);
    }

  };

} // namespace Niantic.ARDK.SharedAR

#endif // SHARED_AR_V2