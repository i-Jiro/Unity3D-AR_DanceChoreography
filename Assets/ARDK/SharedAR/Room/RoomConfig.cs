#if SHARED_AR_V2

using System;
using System.Collections;
using System.Collections.Generic;

namespace Niantic.Experimental.ARDK.SharedAR
{
  public class RoomConfig// : Dictionary<string, string>
  {
    public string RoomID {get; set;}
    public string Pin {get; set;}
    public string Name {get; set;}
    public string ConnectionID {get; set;}
    public string DatastoreID {get; set;}
    public float Lat {get; set;}
    public float Lng {get; set;}
    // TODO(kmori): more config to be added

    public RoomConfig(string id)
    {
      RoomID = id;
      Pin = id;
      Name = id;
      ConnectionID = id;
      DatastoreID = id;
    }

    public RoomConfig(string id, string name)
    {
      RoomID = id;
      Pin = id;
      Name = name;
      ConnectionID = id;
      DatastoreID = id;
    }

    public RoomConfig(string id, string pin, string name)
    {
      RoomID = id;
      Pin = pin;
      Name = name;
      ConnectionID = id;
      DatastoreID = id;
    }

    public RoomConfig(string id, string name, float lat, float lng)
    {
      RoomID = id;
      Name = name;
      ConnectionID = id;
      DatastoreID = id;
      Lat = lat;
      Lng = lng;
    }
  }
} // namespace Niantic.ARDK.SharedAR

#endif // SHARED_AR_V2