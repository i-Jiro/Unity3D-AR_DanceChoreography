#if SHARED_AR_V2

using System;
using System.Collections;
using System.Collections.Generic;

namespace Niantic.Experimental.ARDK.SharedAR
{
  public class RoomID: IEquatable<RoomID>
  {

    public static readonly RoomID InvalidRoomID;
    static RoomID()
    {
      InvalidRoomID = new RoomID("");
    }

    private readonly string _string;
    public RoomID(string id) {
        this._string = id;
    }
    public static implicit operator string(RoomID id) {
        return id._string;
    }
    public static implicit operator RoomID(string id) {
        return new RoomID(id);
    }
    public override string ToString()
    {
      return _string;
    }

    public override int GetHashCode()
    {
      return _string.GetHashCode();
    }

    // Implementing IEqualable
    public bool Equals(RoomID info)
    {
      return info != null && _string==info.ToString();
    }

    public override bool Equals(object obj)
    {
      return Equals(obj as RoomID);
    }
  }

} // namespace Niantic.ARDK.SharedAR

#endif // SHARED_AR_V2