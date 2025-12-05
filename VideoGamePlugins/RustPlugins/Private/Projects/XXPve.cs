using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Libraries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Plugins
{
    [Info("XPve", "InfinityNet/Xray", "1.0.0")]
    [Description("PRIVATE PLUGIN")]
    public class XXPve : RustPlugin
    {

        private bool RealID(ulong ID) => BasePlayer.FindByID(ID) != null ? true : false;
        //On Loot Boxes Furnace Etc
        void OnLootEntity(BasePlayer player, BaseEntity entity)
        {
            if(entity != null && player != null)
            {
                if(entity is BasePlayer)
                {
                    if (RealID(entity.OwnerID) && entity.OwnerID != player.userID)
                    {
                        SendReply(player, "Cannot Steal Other Players Stuff");
                        NextFrame(player.EndLooting);
                        return;
                    }
                }
            }
        }

        //On Loot Sleeper Etc
        object CanLootEntity(BasePlayer player, DroppedItemContainer container)
        {
            if (player != null && container != null)
            {
                if (RealID(container.OwnerID) && container.OwnerID != player.userID)
                {
                    SendReply(player, "Cannot Steal Other Players Stuff");
                    NextFrame(player.EndLooting);
                    return false;
                }
            }
            return null;
        }

        object OnEntityTakeDamage(BaseCombatEntity entity, HitInfo info)
        {
            //Attacker
            var attacker = info.InitiatorPlayer;

            if (attacker != null && entity != null)
            {
                //On Hurt Players
                if (entity is BasePlayer)
                {
                    var victim = BasePlayer.allPlayerList.Where(x => x.userID == entity.ToPlayer().userID);
                    if (victim != null && (attacker != victim))
                    {
                        SendReply(attacker, "Cannot Hurt Other Players");
                        return false;
                    }
                }
                
                //On Hurt Buildings
                if (entity is BuildingBlock || entity is Door)
                {
                    if(RealID(entity.OwnerID) && entity.OwnerID != attacker.userID)
                    {
                        SendReply(attacker, "Cannot Damage Other Players Buildings");
                        return false;
                    }
                }
            }
            return null; /*Execute Unknown*/
        }
    }
}