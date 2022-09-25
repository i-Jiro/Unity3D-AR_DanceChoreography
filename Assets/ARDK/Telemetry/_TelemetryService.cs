// Copyright 2022 Niantic, Inc. All Rights Reserved.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using AOT;

using Google.Protobuf;

using Niantic.ARDK.AR;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.Protobuf;
using Niantic.ARDK.Configuration;
using Niantic.ARDK.Utilities.Logging;

namespace Niantic.ARDK.Telemetry
{
  internal class _TelemetryService
  {
    private static CancellationTokenSource _cancellationTokenSource;

    // current ar session based info
    private static Guid _sessionId;
    private static IARSession _currentSession;
    private static string _persistentDataPath;

    // fields that require config and nar system to be initialised
    private static _ITelemetryPublisher _telemetryPublisher;

    private static _ITelemetryPublisher _TelemetryPublisher
    {
      get
      {
        if (_telemetryPublisher == null)
        {
          _telemetryPublisher = LazyInitializePublisher();
        }

        return _telemetryPublisher;
      }
    }

    private static string _developerApiKey;

    private static string _DeveloperApiKey
    {
      get {
        if (string.IsNullOrWhiteSpace(_developerApiKey))
        {
          _developerApiKey = ArdkGlobalConfig._Internal.GetApiKey();
        }

        return _developerApiKey;
      }
    }

    private static ARCommonMetadata _commonMetadata;
    private static ARCommonMetadata _CommonMetadata
    {
      get
      {
        if (_commonMetadata == null)
          _commonMetadata = LazyInitializeCommonMetadata();
        
        return _commonMetadata;
      }
    }
    
    private static _AnalyticsTelemetryPublisher _lazyAnalyticsTelemetryPublisherInstance ;
    private static _AnalyticsTelemetryPublisher _AnalyticsTelemetryPublisherInstance
    {
      get
      {
        if (_lazyAnalyticsTelemetryPublisherInstance == null)
          _lazyAnalyticsTelemetryPublisherInstance = LazyIntializeAnalyticsPublisher();

        return _lazyAnalyticsTelemetryPublisherInstance;
      }
    }


    // fields that can be initialized whenever
    private static readonly int _maxCountPerSecondForSafeIngress;
    private static readonly MessageParser<ARDKTelemetryOmniProto> _protoMessageParser;
    private static readonly ConcurrentQueue<ARDKTelemetryOmniProto> _messagesToBeSent;

    // constants
    private const int _MaxEventCountBeforeDropping = 100;
    private const int _TimeInSecondsForQueueFlush = 3;

    // fields required for safe startup
    private static object _lock;
    private static bool _isIntialised = false; 
    public static readonly _TelemetryService Instance;
    
    static _TelemetryService()
    {
      _lock = new object();
      Instance = new _TelemetryService();
      
      _maxCountPerSecondForSafeIngress = _MaxEventCountBeforeDropping / _TimeInSecondsForQueueFlush;
      _protoMessageParser = new MessageParser<ARDKTelemetryOmniProto>(() => new ARDKTelemetryOmniProto());
      _messagesToBeSent = new ConcurrentQueue<ARDKTelemetryOmniProto>();
    }
    
    public void Start(string persistentDataPath)
    {
      ARLog._Debug("Starting the telemetry service");

      lock (_lock)
      {
        if(!_isIntialised)
          Initialize(persistentDataPath);

        _isIntialised = true;
      }
      
      // in case it is called multiple times. Lets cancel the previous run.
      if (_cancellationTokenSource != null && 
          _cancellationTokenSource.IsCancellationRequested)
      {
        _cancellationTokenSource.Cancel();
      }

      _cancellationTokenSource = new CancellationTokenSource();
#pragma warning disable CS4014 
      // CS 4014: we are not awaiting the fire and forget task. boohoo.
      Task.Run(async () =>
      {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
          await PublishEventsEverySecondAsync(_cancellationTokenSource.Token);
        }
      });
#pragma warning restore CS4014
      
      ARLog._Debug("Started the telemetry service successfully");
    }
    
    public void Stop()
    {
      ARLog._Debug("Stopping the telemetry service");

      lock (_lock)
      {
        if (!_isIntialised)
        {
          ARLog._Debug("Stopped the telemetry service successfully");
          return;
        }
      }
      
      if (_cancellationTokenSource != null && 
          !_cancellationTokenSource.IsCancellationRequested)
      {
        _cancellationTokenSource.Cancel();
      }
      
      lock (_lock)
      {
        _isIntialised = false;
      }

      ARLog._Debug("Stopped the telemetry service successfully");
    }

#region C-sharp event logging
    
    public static void RecordEvent(InitializationEvent initializationEvent)
    {
      try
      {
        ARDKTelemetryOmniProto initOmniProto = new ARDKTelemetryOmniProto()
        {
          InitializationEvent = initializationEvent,
          TimestampMs = GetCurrentUtcTimestamp(),
          DeveloperKey = _DeveloperApiKey,
          CommonMetadata = _CommonMetadata.Clone(),
          ArSessionId = _sessionId.ToString(),
        };

        _messagesToBeSent.Enqueue(initOmniProto);
      }
      catch (Exception ex)
      {
        ARLog._Warn($"Recording telemetry event failed with {ex}");
      }
    }
    
    public static void RecordEvent(ARSessionEvent sessionEvent)
    {
      try
      {
        ARDKTelemetryOmniProto initOmniProto = new ARDKTelemetryOmniProto()
        {
          ArSessionEvent = sessionEvent,
          TimestampMs = GetCurrentUtcTimestamp(),
          DeveloperKey = _DeveloperApiKey,
          CommonMetadata = _CommonMetadata.Clone(),
          ArSessionId = _sessionId.ToString(),
        };
      
        _messagesToBeSent.Enqueue(initOmniProto);
      }
      catch (Exception ex)
      {
        ARLog._Warn($"Recording telemetry event failed with {ex}");
      }
    }
    
    public static void RecordEvent(EnabledContextualAwarenessEvent enabledContextualAwarenessEvent)
    {
      try
      {
        ARDKTelemetryOmniProto initOmniProto = new ARDKTelemetryOmniProto()
        {
          EnableContextualAwarenessEvent = enabledContextualAwarenessEvent,
          TimestampMs = GetCurrentUtcTimestamp(),
          DeveloperKey = _DeveloperApiKey,
          CommonMetadata = _CommonMetadata.Clone(),
          ArSessionId = _sessionId.ToString(),
        };
      
        _messagesToBeSent.Enqueue(initOmniProto);
      }
      catch (Exception ex)
      {
        ARLog._Warn($"Recording telemetry event failed with {ex}");
      }
    }
    
    public static void RecordEvent(MultiplayerColocalizationEvent multiplayerColocalizationEvent)
    {
      try
      {
        ARDKTelemetryOmniProto initOmniProto = new ARDKTelemetryOmniProto()
        {
          MultiplayerColocalizationEvent = multiplayerColocalizationEvent,
          TimestampMs = GetCurrentUtcTimestamp(),
          DeveloperKey = _DeveloperApiKey,
          CommonMetadata = _CommonMetadata.Clone(),
          ArSessionId = _sessionId.ToString(),
        };
      
        _messagesToBeSent.Enqueue(initOmniProto);
      }
      catch (Exception ex)
      {
        ARLog._Warn($"Recording telemetry event failed with {ex}");
      }
    }
    
    public static void RecordEvent(MultiplayerConnectionEvent multiplayerConnectionEvent)
    {
      try
      {
        ARDKTelemetryOmniProto initOmniProto = new ARDKTelemetryOmniProto()
        {
          MultiplayerConnectionEvent = multiplayerConnectionEvent,
          TimestampMs = GetCurrentUtcTimestamp(),
          DeveloperKey = _DeveloperApiKey,
          CommonMetadata = _CommonMetadata.Clone(),
          ArSessionId = _sessionId.ToString(),
        };
      
        _messagesToBeSent.Enqueue(initOmniProto);
      }
      catch (Exception ex)
      {
        ARLog._Warn($"Recording telemetry event failed with {ex}");
      }
    }
    
    public static void RecordEvent(MultiplayerColocalizationInitializationEvent multiplayerColocalizationInitializationEvent)
    {
      try
      {
        ARDKTelemetryOmniProto initOmniProto = new ARDKTelemetryOmniProto()
        {
          MultiplayerColocalizationInitializationEvent = multiplayerColocalizationInitializationEvent,
          TimestampMs = GetCurrentUtcTimestamp(),
          DeveloperKey = _DeveloperApiKey,
          CommonMetadata = _CommonMetadata.Clone(),
          ArSessionId = _sessionId.ToString(),
        };
      
        _messagesToBeSent.Enqueue(initOmniProto);
      }
      catch (Exception ex)
      {
        ARLog._Warn($"Recording telemetry event failed with {ex}");
      }
    }
    
    public static void RecordEvent(LightshipServiceEvent lightshipServiceEvent)
    {
      try
      {
        ARDKTelemetryOmniProto initOmniProto = new ARDKTelemetryOmniProto()
        {
          LightshipServiceEvent = lightshipServiceEvent,
          TimestampMs = GetCurrentUtcTimestamp(),
          DeveloperKey = _DeveloperApiKey,
          CommonMetadata = _CommonMetadata.Clone(),
          ArSessionId = _sessionId.ToString(),
        };
      
        _messagesToBeSent.Enqueue(initOmniProto);
      }
      catch (Exception ex)
      {
        ARLog._Warn($"Recording telemetry event failed with {ex}");
      }
    }
    
#endregion

    private static async Task PublishEventsEverySecondAsync(CancellationToken cancellationToken)
    {
      ARLog._Debug("Starting fire and forget task to publish events every second");
      while (!cancellationToken.IsCancellationRequested)
      {
        PublishEventsEverySecond();
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
      }
      
      ARLog._Debug("Stopping fire and forget task for event publishing");
    }

    /*
    * The telemetry client has some limitations. 
    * The max possible buffer size is 100. Any more messages and you have dropping that ensues.
    * The buffer gets cleared out every 3 seconds. 
    * So if we limit the ingress to be 100/3 i.e. 33 events/second, we can stay right on the limit
    * and stagger the telemetry events
    */
    private static void PublishEventsEverySecond()
    {
      PublishTelemetryEvents(_messagesToBeSent, _maxCountPerSecondForSafeIngress);
    }

    private static void PublishTelemetryEvents(ConcurrentQueue<ARDKTelemetryOmniProto> events, int count)
    {
      // We do not know if there will be events to send in the queue at all times. So we 
      // run the for loop from 0 to either the count/Max possible events in the queue. Whatever is lower.
      int queueLengthRightNow = events.Count;
      for (int i = 0; i < Math.Min(count, queueLengthRightNow); i++)
      {
        var success = events.TryDequeue(out var eventToSend);
        if (success)
        {
          _TelemetryPublisher.RecordEvent(eventToSend);
        }
      }
    }
    
    internal delegate void _ARDKTelemetry_Callback([MarshalAs(UnmanagedType.LPArray, SizeParamIndex= 1)]byte[] serialisedProto, UInt32 length);

    [MonoPInvokeCallback(typeof(_ARDKTelemetry_Callback))]
    internal static void _OnNativeRecordTelemetry(byte[] serialisedPayload, UInt32 payloadLength)
    {
      try
      {
        var omniProtoObject = _protoMessageParser.ParseFrom(serialisedPayload); 
        if (omniProtoObject.TimestampMs == default)
        {
          omniProtoObject.TimestampMs = GetCurrentUtcTimestamp();
        }
        
        omniProtoObject.CommonMetadata = _CommonMetadata.Clone();
        omniProtoObject.ArSessionId = _sessionId.ToString();
        omniProtoObject.DeveloperKey = _DeveloperApiKey;
        
        _messagesToBeSent.Enqueue(omniProtoObject);
      }
      catch (Exception e) 
      {
        // failing silently and not bothering the users
        ARLog._WarnFormat("Sending telemetry failed: {0}.", objs: e);
      }
    }

    private static void OnConfigLoginChanged()
    {
      _telemetryPublisher = GetPublisherBasedOnAgeLevel();
    }

    private static _ITelemetryPublisher GetPublisherBasedOnAgeLevel()
    {
      if (ArdkGlobalConfig._Internal.GetAgeLevel() == ARClientEnvelope.Types.AgeLevel.Minor)
      {
        ARLog._Debug("The user has been determined to be a minor. Disabling telemetry.");
        return new _DummyTelemetryPublisher();
      }
    
      ARLog._Debug("Using the ardk telemetry service.");
      return _AnalyticsTelemetryPublisherInstance;
    }

    private void Initialize(string persistentDataPath)
    {
      ARLog._Debug("Initializing telemetry service.");

      _persistentDataPath = persistentDataPath;
      ArdkGlobalConfig._LoginChanged += OnConfigLoginChanged;
      ARSessionFactory.SessionInitialized += AssignCurrentSessionInfo;
      
      ARLog._Debug("Successfully initialized telemetry service.");
    }

    private static _ITelemetryPublisher LazyInitializePublisher()
    {
      return GetPublisherBasedOnAgeLevel();
    }
    
    private static long GetCurrentUtcTimestamp()
    {
      return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
    
    private static void AssignCurrentSessionInfo(AnyARSessionInitializedArgs args)
    {
      _currentSession = args.Session;
      _sessionId = args.Session.StageIdentifier;
      _currentSession.Deinitialized += UnAssignCurrentSessionInfo;
    }
    
    private static ARCommonMetadata LazyInitializeCommonMetadata()
    {
      var internallyVisibleConfig = ArdkGlobalConfig._Internal;
      
      var commonMetadata = new ARCommonMetadata
      {
        Manufacturer = string.IsNullOrWhiteSpace(internallyVisibleConfig.GetManufacturer()) ? 
          string.Empty : internallyVisibleConfig.GetManufacturer(),
        ApplicationId = string.IsNullOrWhiteSpace(internallyVisibleConfig.GetApplicationId()) ? 
          string.Empty : internallyVisibleConfig.GetApplicationId(),
        ClientId = string.IsNullOrWhiteSpace(internallyVisibleConfig.GetClientId()) ? 
          string.Empty : internallyVisibleConfig.GetClientId(),
        UserId = string.IsNullOrWhiteSpace(internallyVisibleConfig.GetUserId()) ? 
          string.Empty : internallyVisibleConfig.GetUserId(),
        ArdkVersion = string.IsNullOrWhiteSpace(internallyVisibleConfig.GetArdkVersion()) ? 
          string.Empty : internallyVisibleConfig.GetArdkVersion(),
        DeviceModel = string.IsNullOrWhiteSpace(internallyVisibleConfig.GetDeviceModel()) ? 
          string.Empty : internallyVisibleConfig.GetDeviceModel(),
        ArdkAppInstanceId = string.IsNullOrWhiteSpace(internallyVisibleConfig.GetArdkAppInstanceId()) ? 
          string.Empty : internallyVisibleConfig.GetArdkAppInstanceId(),
        Platform = string.IsNullOrWhiteSpace(internallyVisibleConfig.GetPlatform()) ? 
          string.Empty : internallyVisibleConfig.GetPlatform()
      };

      return commonMetadata;
    }
    
    private static _AnalyticsTelemetryPublisher LazyIntializeAnalyticsPublisher()
    {
      var telemetryKey = string.IsNullOrWhiteSpace(ArdkGlobalConfig._GetTelemetryKey()) ? string.Empty : ArdkGlobalConfig._GetTelemetryKey();
      return new _AnalyticsTelemetryPublisher
      ( 
        _persistentDataPath + Path.DirectorySeparatorChar + "temp", 
        telemetryKey, 
        false
      );
    }

    private static void UnAssignCurrentSessionInfo(ARSessionDeinitializedArgs args)
    {
      _currentSession = null;
    }
    
    ~_TelemetryService()
    {
      ARSessionFactory.SessionInitialized -= AssignCurrentSessionInfo;
      _cancellationTokenSource.Cancel();
    }
  }
}
