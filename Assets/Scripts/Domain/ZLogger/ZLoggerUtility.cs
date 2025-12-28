using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;
using ZLogger.Unity;

namespace Common.ZLogger
{
    public sealed class ZLoggerUtility
    {
        private static ILoggerFactory _loggerFactory;
        private static ILogger<ZLoggerUtility> _logger;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_loggerFactory != null)
            {
                return;
            }

            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddZLoggerUnityDebug();
            });
            
            _logger = _loggerFactory.CreateLogger<ZLoggerUtility>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitApplicationQuittingHandler()
        {
            UnityEngine.Device.Application.quitting += OnApplicationQuit;
        }

        private static void OnApplicationQuit()
        {
            if (_loggerFactory != null)
            {
                _loggerFactory.Dispose();
                _loggerFactory = null;
                ZLoggerUtility.LogDebug("ZLogger Factory Disposed.");
            }
        }

        public static void LogDebug(string message)
        {
            _logger.ZLogDebug($"{message}");
        }

        public static void LogWarning(string message)
        {
            _logger.ZLogWarning($"{message}");
        }

        public static void LogError(string message)
        {
            _logger.ZLogError($"{message}");
        }
    }
}