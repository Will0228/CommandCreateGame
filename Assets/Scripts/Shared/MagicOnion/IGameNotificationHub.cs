using System.Threading.Tasks;
using MagicOnion;

namespace Shared.MagicOnion
{
    /// <summary>
    /// サーバーとクライアントで共有する StreamingHub インターフェース
    /// </summary>
    public interface IGameNotificationHub : IStreamingHub<IGameNotificationHub, IGameNotificationHubReceiver>
    {
        /// <summary>
        /// クライアントから呼び出し。サーバーがサンプルデータをクライアントへ送る
        /// </summary>
        Task RequestSampleDataAsync();
    }

    /// <summary>
    /// サーバーからクライアントへデータを受け取るためのインターフェース（クライアント側で実装）
    /// </summary>
    public interface IGameNotificationHubReceiver
    {
        void OnReceiveNotification(GameNotificationDto notification);
    }
}
