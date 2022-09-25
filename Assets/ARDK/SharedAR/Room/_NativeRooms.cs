// Copyright 2022 Niantic, Inc. All Rights Reserved.

#if SHARED_AR_V2

using System.Collections.Generic;

namespace Niantic.Experimental.ARDK.SharedAR
{
  public sealed class _NativeRooms :
    IRooms
  {
    public RoomID Create(RoomConfig config)
    {
      throw new System.NotImplementedException();
    }

    public string GeneratePin(RoomID roomID)
    {
      throw new System.NotImplementedException();
    }

    public RoomID ResolvePin(string pin)
    {
      throw new System.NotImplementedException();
    }

    public void RevokePin(RoomID roomID)
    {
      throw new System.NotImplementedException();
    }

    public List<RoomID> Find(string criteria)
    {
      throw new System.NotImplementedException();
    }

    public RoomConfig GetRoomMetadata(RoomID roomID)
    {
      throw new System.NotImplementedException();
    }

    public (INetworking, IDatastore) JoinRoom(RoomID roomID)
    {
      throw new System.NotImplementedException();
    }

    public RoomRequestResult Delete(RoomID roomID)
    {
      throw new System.NotImplementedException();
    }

    public void Dispose()
    {
    }
  }
}

#endif