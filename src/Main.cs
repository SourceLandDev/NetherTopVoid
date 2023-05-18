using LiteLoader.Hook;
using LiteLoader.NET;
using MC;
using System.Reflection;

namespace NetherHigherVoid;
[PluginMain(pluginName)]
public class NetherHigherVoid : IPluginInitializer
{
    internal const string pluginName = "NetherHigherVoid";
    public string Introduction => "下界高处虚空";
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
            NetherHigherVoid.TickCount[player.Xuid] = NetherHigherVoid.TickCount.TryGetValue(player.Xuid, out int value) ? value + 1 : 1;
            Console.WriteLine(NetherHigherVoid.TickCount[player.Xuid]);
            if (NetherHigherVoid.TickCount[player.Xuid] < 11)
            {
                return;
            }
            player.HurtEntity(4, ActorDamageCause.Void);
        }
        NetherHigherVoid.TickCount.Remove(player.Xuid);
    };
}
