using System.Collections.Concurrent;
using LiteLoader.Hook;
using LiteLoader.NET;
using LiteLoader.Schedule;
using MC;

namespace NetherTopVoid;
[PluginMain(pluginName)]
public class NetherTopVoid : IPluginInitializer
{
    internal const string pluginName = "NetherTopVoid";
    public string Introduction => "下界顶部虚空";
    public Dictionary<string, string> MetaData => new();
    internal static ConcurrentDictionary<string, int> TickCount = new();
    public void OnInitialize() => Thook.RegisterHook<PlayerTickHook, PlayerTickHookCallback>();
}

internal delegate void PlayerTickHookCallback(pointer<ServerPlayer> player);
[HookSymbol("?normalTick@ServerPlayer@@UEAAXXZ")]
internal class PlayerTickHook : THookBase<PlayerTickHookCallback>
{
    public override PlayerTickHookCallback Hook => (playerPointer) =>
    {
        Original(playerPointer);
        Player player = playerPointer.Dereference();
        Player.GameType gameMode = player.GameMode;
        int dimensionId = player.DimensionId;
        BlockPos blockPos = player.BlockPos;
        string xuid = player.Xuid;
        Task.Run(() =>
        {
            if (
                gameMode is not Player.GameType.Creative and not Player.GameType.Spectator &&
                dimensionId is 1 &&
                blockPos.Y > 127
            )
            {
                NetherTopVoid.TickCount[xuid] = NetherTopVoid.TickCount.TryGetValue(xuid, out int value) ? value + 1 : 1;
                if (NetherTopVoid.TickCount[xuid] < 11)
                {
                    return;
                }
                ScheduleAPI.NextTick(() =>
                {
                    foreach (Player player in Level.GetAllPlayers())
                    {
                        if (player.Xuid != xuid)
                        {
                            continue;
                        }
                        player.HurtEntity(4, ActorDamageCause.Void);
                        break;
                    }
                });
            }
            NetherTopVoid.TickCount.TryRemove(xuid, out _);
        });
    };
}
