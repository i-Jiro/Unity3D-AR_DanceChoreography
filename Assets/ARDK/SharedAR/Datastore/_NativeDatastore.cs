// Copyright 2022 Niantic, Inc. All Rights Reserved.
#pragma warning disable 0067
#if SHARED_AR_V2

using System.Collections.Generic;
using Niantic.ARDK.Utilities;

namespace Niantic.Experimental.ARDK.SharedAR
{
  public sealed class _NativeDatastore :
    IDatastore
  {
    public Result SetData(string key, byte[] value)
    {
      throw new System.NotImplementedException();
    }

    public Result SetData<T>(string key, T value)
    {
      throw new System.NotImplementedException();
    }

    public Result GetData(string key, ref byte[] value)
    {
      throw new System.NotImplementedException();
    }

    public Result GetData<T>(string key, ref T value)
    {
      throw new System.NotImplementedException();
    }

    public Result DeleteData(string key)
    {
      throw new System.NotImplementedException();
    }

    public List<string> GetKeys()
    {
      throw new System.NotImplementedException();
    }

    public Result ClaimOwnership(string key)
    {
      throw new System.NotImplementedException();
    }

    public Result SetLifeCycle(string key, LifeCycleOptions options)
    {
      throw new System.NotImplementedException();
    }

    public event ArdkEventHandler<KeyValuePairArgs> KeyValueAdded;
    public event ArdkEventHandler<KeyValuePairArgs> KeyValueUpdated;
    public event ArdkEventHandler<KeyValuePairArgs> KeyValueDeleted;

    public void Dispose()
    {
    }
  }
}

#endif
#pragma warning restore 0067