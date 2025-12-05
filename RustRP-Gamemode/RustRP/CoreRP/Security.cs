using System;
using System.Linq;

namespace CoreRP
{
    internal interface ISecurity
    {
        bool IsAuthenticated { get; }
    }
    internal static class Security
    {
        internal sealed class Player : ISecurity
        {
            private bool authenticated { get; } = false;

            internal Player(BasePlayer player, Settings.Groups.Group group) : this(player, group?.minuimAuthLevel ?? 0, group?.allowedIds, group?.requiredPermissions, group?.requiredGroups, group?.allowedIPs) { }
            internal Player(BasePlayer player, uint minuimAuthLevel = 0, ulong[] allowedIds = null, string[] requiredPermissions = null, string[] requiredGroups = null, string[] allowedIPs = null)
            {
                if (!player.IsPlayer(true)) { return; }
                var oxidePerms = Oxide.Core.Interface.Oxide.GetLibrary<Oxide.Core.Libraries.Permission>();
      
                if (
                    player?.Connection?.authLevel < minuimAuthLevel || 
                    (allowedIds != null && !allowedIds.Contains(player?.userID ?? 0)) || 
                    (requiredPermissions != null && !requiredPermissions.All(permission => oxidePerms.UserHasPermission(player?.UserIDString ?? "", permission))) ||
                    (requiredGroups != null && !requiredGroups.All(group => oxidePerms.UserHasGroup(player?.UserIDString ?? "", group))) ||
                    (allowedIPs != null && !allowedIPs.Contains(player?.Connection?.IPAddressWithoutPort() ?? ""))
                ) { return; }

                this.authenticated = true;
            }

            public static implicit operator bool(Player player) => player?.authenticated ?? false;
            public bool IsAuthenticated => this.authenticated;
        }
    }
}