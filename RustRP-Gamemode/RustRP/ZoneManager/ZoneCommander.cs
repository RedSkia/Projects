using System;
using UnityEngine;

namespace CoreRP.ZoneManager
{
    internal sealed class ZoneCommander : Contracts.IChatCommand
    {
        public string Command => "zone";
        public void OnCommand(BasePlayer player, string command, string[] args)
        {
            Script.Instance.ScriptCheck();
            var argSelector = args.ElementAt(0).ToLower();
            var argValue = args.ElementAt(1).ToLower();

            switch (argSelector)
            {
                case "s":
                case "save": {
                    ZoneCreatorBehaviour tool;
                    if(!player.TryGetComponent<ZoneCreatorBehaviour>(out tool)) { player.SendMsg("ZoneTool", "missing vector points use /zone tool");  return; }
                    ZoneRect zoneRect = new ZoneRect(tool.zonePoints, tool.zoneHeight, tool.zoneRotation); 
                    ZoneObject zone = new ZoneObject(tool.zonePrefix, zoneRect, tool.zoneFlags);
                    tool.Init(zone);
                    string result = ZoneData.Instance.AddZone(argValue, zone) ? $"sucessfully created \"{argValue}\"" : $"failed to create \"{argValue}\"";
                    player.SendMsg("ZoneTool", $"{result}");
                    ZoneData.Instance.SaveData();
                } break;

                case "d":
                case "del": {
                    string result = ZoneData.Instance.DeleteZone(argValue) ? $"sucessfully deleted \"{argValue}\"" : $"failed to delete \"{argValue}\"";
                    player.SendMsg("ZoneTool", $"{result}");
                    ZoneData.Instance.SaveData();
                } break;

                case "f":
                case "flag": {
                    ZoneCreatorBehaviour tool;
                    if(!player.TryGetComponent<ZoneCreatorBehaviour>(out tool)) { player.SendMsg("ZoneTool", "to edit zone flags you must enable /zone tool");  return; }
                    if(String.IsNullOrEmpty(argValue))
                    {
                        player.SendMsg("ZoneTool", $"Active flags: {tool.zoneFlags}");
                        return;
                    }
                    ZoneFlagsAllowed inputFlag = FlagManager<ZoneFlagsAllowed>.To(argValue[0]);
                    bool hasInputFlag = FlagManager<ZoneFlagsAllowed>.Has(tool.zoneFlags, inputFlag);
                    if (argValue[0] == '*')
                    {
                        bool hasAny = FlagManager<ZoneFlagsAllowed>.HasAny(tool.zoneFlags);
                        if (hasAny)
                        {
                            FlagManager<ZoneFlagsAllowed>.Clear(ref tool.zoneFlags);
                            player.SendMsg("ZoneTool", $"All flags removed");
                        }
                        else
                        {
                            FlagManager<ZoneFlagsAllowed>.Add(ref tool.zoneFlags,  FlagManager<ZoneFlagsAllowed>.From(FlagManager<ZoneFlagsAllowed>.Total()));
                            player.SendMsg("ZoneTool", $"All flags added");
                        }
                        return;
                    }
                    if (inputFlag == ZoneFlagsAllowed.None) 
                    {
                        player.SendMsg("ZoneTool", $"Flag not found");
                        return;
                    }
                    if(!hasInputFlag)
                    {
                        FlagManager<ZoneFlagsAllowed>.Add(ref tool.zoneFlags, inputFlag);
                        player.SendMsg("ZoneTool", $"Flag added: {inputFlag}");
                    }
                    else
                    {
                        FlagManager<ZoneFlagsAllowed>.Remove(ref tool.zoneFlags, inputFlag);
                        player.SendMsg("ZoneTool", $"Flag removed: {inputFlag}");
                    }
                } break;

                case "t":
                case "tool": { 
                    ZoneCreator.ToggleTool(player, argValue);
                } break;
                default: { 
                    player.SendMsg("ZoneTool", $"/zone <save|del|tool> \"zoneName\"\n/zone <flag> \"flag\"\n/zone <flag> *");
                } break;
            }
        }
    }
}