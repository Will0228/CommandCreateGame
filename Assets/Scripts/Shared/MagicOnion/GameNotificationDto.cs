using System;
using MessagePack;

namespace Shared.MagicOnion
{
    /// <summary>
    /// サーバーからクライアントへ送るサンプルデータ
    /// </summary>
    [MessagePackObject]
    public sealed class GameNotificationDto
    {
        [Key(0)]
        public string Message { get; set; } = string.Empty;

        [Key(1)]
        public int Value { get; set; }

        [Key(2)]
        public long SentAtUnixSeconds { get; set; }

        public GameNotificationDto()
        {
        }

        public GameNotificationDto(string message, int value, long sentAtUnixSeconds)
        {
            Message = message;
            Value = value;
            SentAtUnixSeconds = sentAtUnixSeconds;
        }

        public static GameNotificationDto CreateWelcome()
        {
            return new GameNotificationDto(
                "サーバーに接続しました",
                0,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

        public static GameNotificationDto CreateSample(int value)
        {
            return new GameNotificationDto(
                "サーバーから送られたサンプルデータ",
                value,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }
    }
}
