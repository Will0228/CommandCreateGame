using System;
using R3;
using Shared.MagicOnion;

namespace Infra.MagicOnion
{
    /// <summary>
    /// サーバーから送られた通知を受け取るクライアント側の実装
    /// </summary>
    public sealed class GameNotificationHubReceiver : IGameNotificationHubReceiver, IDisposable
    {
        private readonly Subject<GameNotificationDto> _onReceivedSubject = new();

        /// <summary>
        /// サーバーからデータを受信したときに発火する
        /// </summary>
        public Observable<GameNotificationDto> OnReceivedAsObservable => _onReceivedSubject;

        public void OnReceiveNotification(GameNotificationDto notification)
        {
            _onReceivedSubject.OnNext(notification);
        }

        public void Dispose()
        {
            _onReceivedSubject.Dispose();
        }
    }
}
