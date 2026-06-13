using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.MagicOnion;

namespace Infra.MagicOnion
{
    /// <summary>
    /// MagicOnion の StreamingHub に接続し、サーバーからのデータを受け取るクライアント
    /// </summary>
    public sealed class GameNotificationHubClient : IDisposable
    {
        private GrpcChannel _channel;
        private IGameNotificationHub _hub;

        /// <summary>
        /// 既定: ローカル MagicOnion サーバー（HTTP/2、TLS なし）
        /// Unity では YetAnotherHttpHandler の導入が必要な場合があります。
        /// https://cysharp.github.io/MagicOnion/installation/unity
        /// </summary>
        public const string DefaultServerAddress = "http://localhost:5000";

        public bool IsConnected => _hub != null;

        /// <summary>
        /// サーバーへ接続する
        /// </summary>
        public async UniTask ConnectAsync(
            GameNotificationHubReceiver receiver,
            string serverAddress = DefaultServerAddress,
            CancellationToken cancellationToken = default)
        {
            if (receiver == null)
            {
                throw new ArgumentNullException(nameof(receiver));
            }

            await DisconnectAsync();

            _channel = GrpcChannel.ForAddress(serverAddress);
            _hub = await StreamingHubClient.ConnectAsync<IGameNotificationHub, IGameNotificationHubReceiver>(
                _channel,
                receiver,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// サーバーにサンプルデータの送信を依頼する（サーバーが Receiver 経由で返す）
        /// </summary>
        public async UniTask RequestSampleDataAsync(CancellationToken cancellationToken = default)
        {
            EnsureConnected();
            await _hub.RequestSampleDataAsync();
        }

        public async UniTask DisconnectAsync()
        {
            if (_hub != null)
            {
                await _hub.DisposeAsync();
                _hub = null;
            }

            if (_channel != null)
            {
                _channel.Dispose();
                _channel = null;
            }
        }

        public void Dispose()
        {
            DisconnectAsync().Forget();
        }

        private void EnsureConnected()
        {
            if (_hub == null)
            {
                throw new InvalidOperationException("Hub に未接続です。先に ConnectAsync を呼び出してください。");
            }
        }
    }
}
