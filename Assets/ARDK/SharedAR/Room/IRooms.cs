#if SHARED_AR_V2

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Niantic.Experimental.ARDK.SharedAR
{
  public enum RoomRequestResult
  {
    Success = 0,
    Error = 1
  };

  public interface IRooms : 
    IDisposable
  {

    /// <summary>
    /// Create a room on the Rooms backend.
    /// </summary>
    /// <param name="config">Settings of the room as key value pairs</param>
    /// <returns>RoomID of the newly created room</returns>
    RoomID Create(RoomConfig config);

    /// <summary>
    /// Generate a new PIN for the specified room, which can be used to join room.
    /// valid for a few minutes. PIN may be 4-6 digit number, alphanumeric, etc.
    /// </summary>
    /// <param name="roomID">Settings of the room as key value pairs</param>
    /// <returns>RoomID of the newly created room</returns>
    string GeneratePin(RoomID roomID);

    /// <summary>
    /// Get Room ID from the PIN . This is used by the peer joining a room with PIN.
    /// </summary>
    /// <param name="pin">PIN</param>
    /// <returns>RoomID of the given PIN, or InvalidRoomID no room has given PIN</returns>
    RoomID ResolvePin(string pin);

    /// <summary>
    /// Revoke the PIN for the room. This can be used to “close the door” to the room
    /// after enough players joined or prevent bad actor rejoining with PIN.
    /// </summary>
    /// <param name="roomID">Room ID to revoke PIN</param>
    void RevokePin(RoomID roomID);

    /// <summary>
    /// Find rooms by given query.
    /// criteria may be by PIN, room name, geo search, etc with filter by activeness. Returns list of RoomInstances.
    /// </summary>
    /// <param name="roomID">Room ID to revoke PIN</param>    public static List<RoomID> Find(string criteria)
    /// <returns>RoomID of the given PIN, or InvalidRoomID no room has given PIN</returns>
    // TODO(kmori): Probably use something else than string to be less ambigiuous in querying
    List<RoomID> Find(string criteria);

    /// <summary>
    /// Get RoomConfig of the specific room
    /// </summary>
    /// <param name="roomID">Room ID</param>
    /// <returns>Room metadata as RoomConfig type</returns>
    RoomConfig GetRoomMetadata(RoomID roomID);

    /// <summary>
    /// Get RoomConfig of the specific room
    /// </summary>
    /// <param name="roomID">Room ID</param>
    /// <returns>Room metadata as RoomConfig type</returns>
    (INetworking, IDatastore) JoinRoom(RoomID roomID);

    /// <summary>
    /// Get RoomConfig of the specific room
    /// </summary>
    /// <param name="roomID">Room ID</param>
    /// <returns>Room metadata as RoomConfig type</returns>
    // TBD access control who can delete in future?
    RoomRequestResult Delete(RoomID roomID);

  }
} // namespace Niantic.ARDK.SharedAR

#endif // SHARED_AR_V2