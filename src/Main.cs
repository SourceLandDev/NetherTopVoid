using LiteLoader.Hook;
using LiteLoader.NET;
using MC;
using System.Reflection;

namespace NetherTopVoid;
[PluginMain(pluginName)]
public class NetherTopVoid : IPluginInitializer
{
    internal const string pluginName = "NetherTopVoid";
    public string Introduction => "下界顶部虚空";
    public Dictionary<string, string> MetaData => new();
    public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
    internal static Dictionary<string, int> TickCount = new();
    public void OnInitialize() => Thook.RegisterHook<PlayerTickHook, PlayerTickHookCallback>();
}

internal delegate void PlayerTickHookCallback(pointer<Player> player);
[HookSymbol("?normalTick@Player@@UEAAXXZ")]
internal class PlayerTickHook : THookBase<PlayerTickHookCallback>
{
    public override PlayerTickHookCallback Hook => (playerPointer) =>
    {
        Original(playerPointer);
        Player player = playerPointer.Dereference();
        if (
            (int)player.GameMode is not 1 and not 6 &&
            player.DimensionId is 1 &&
            player.BlockPos.Y > 128
        )
        {
            NetherTopVoid.TickCount[player.Xuid] = NetherTopVoid.TickCount.TryGetValue(player.Xuid, out int value) ? value + 1 : 1;
            Console.WriteLine(NetherTopVoid.TickCount[player.Xuid]);
            if (NetherTopVoid.TickCount[player.Xuid] < 11)
            {
                return;
            }
            player.HurtEntity(4, ActorDamageCause.Void);
        }
        NetherTopVoid.TickCount.Remove(player.Xuid);
    };
}
