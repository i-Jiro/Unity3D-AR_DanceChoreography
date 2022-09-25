#if SHARED_AR_V2
using System;
using System.Collections.Generic;
using Niantic.ARDK.Utilities;

namespace Niantic.Experimental.ARDK.SharedAR
{

  /// <summary>
  /// 
  /// </summary>
  public enum Result
  {
    Success,
    Fail, // too generic
    NotAuthorized

    // TODO(kmori): add more detailed status/error as needed
  };

  public enum LifeCycleOptions
  {
    DuringActive,
    Persisted,
    UntilDisconnected
  };

  /// <summary>
  /// 
  /// </summary>
  public struct KeyValuePairArgs : IArdkEventArgs
  {
    public string Key { get; set; }
    public KeyValuePairArgs(string key)
    {
      Key = key;
    }
  }

  public interface IDatastore :
    IDisposable
  {
    // CRUD operations

    /// <summary>
    /// Set data into storage
    /// </summary>
    /// <param name="key">Key of the data</param>
    /// <param name="value">Value to set</param>
    /// <returns>Result of the operation</returns>
    // TODO(kmori): should be async? should have other value type?
    Result SetData(string key, byte[] value);
    Result SetData<T>(string key, T value);

    /// <summary>
    /// Set data into storage
    /// </summary>
    /// <param name="key">Key of the data</param>
    /// <param name="value">Value to set</param>
    /// <returns>Result of the operation</returns>
    // TODO(kmori): should be async? should have other value type?
    Result GetData(string key, ref byte[] value);
    Result GetData<T>(string key, ref T value);

    /// <summary>
    /// Delete the key-value pair from the storage
    /// </summary>
    /// <param name="key">Key of the data</param>
    /// <returns>Result of the operation</returns>
    // TODO(kmori): should be async?
    Result DeleteData(string key);

    /// <summary>
    /// Get list of keys under specified tag
    /// </summary>
    /// <returns>List of keys</returns>
    // TODO(kmori): should be async?
    List<string> GetKeys();

    //

    /// <summary>
    /// Claim ownership of data specified by the key
    /// </summary>
    /// <returns>Result of the operation</returns>
    Result ClaimOwnership(string key);

    /// <summary>
    /// Set lifecycle of the data specified by the key
    /// </summary>
    /// <returns>Result of the operation</returns>
    Result SetLifeCycle(string key, LifeCycleOptions options);
    // TODO(kmori): Need a getter?

    // Events

    //
    event ArdkEventHandler<KeyValuePairArgs> KeyValueAdded;

    //
    event ArdkEventHandler<KeyValuePairArgs> KeyValueUpdated;

    //
    event ArdkEventHandler<KeyValuePairArgs> KeyValueDeleted;

    // TODO(kmori): Add transaction interfaces
    // TBD batch/transactional/atomic job. Receive result asynchronously
    //IDatastore::startTransaction()
    //IDatastore::endTransaction(): async_response_obj
  }
} // namespace Niantic.ARDK.SharedAR

#endif // SHARED_AR_V2