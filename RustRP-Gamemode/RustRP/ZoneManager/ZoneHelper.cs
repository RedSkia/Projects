using System;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace CoreRP.ZoneManager
{
    internal static class ZoneHelper
    {
        internal static Vector3 GetPoint(Vector3 point, Vector3 pivot, Quaternion rotation) => rotation * (point - pivot) + pivot;
        internal static Vector3 MapPoint(Vector3 point) => new Vector3(point.x, TerrainMeta.HeightMap.GetHeight(point), point.z);
        internal static Vector3 CenteredPoint(params Vector3[] points) => points.Aggregate((a, c) => a + c) / points.Length;
        internal static Vector3 MinPoint(params Vector3[] points) => points.Aggregate((a, c) => Vector3.Min(a, c));
        internal static Vector3 MaxPoint(params Vector3[] points) => points.Aggregate((a, c) => Vector3.Max(a, c));
        internal static Vector3[] GenerateRandomPoints(ZoneRect zone, int amount)
        {
            Vector3 extents = new Vector3(zone.Size.x * 0.5f, 0f, zone.Size.z * 0.5f);
            Vector3[] points = new Vector3[amount];
            for (int i = 0; i < amount; i++)
            {
                float randomX = UnityEngine.Random.Range(zone.Center.x - extents.x, zone.Center.x + extents.x);
                float randomZ = UnityEngine.Random.Range(zone.Center.z - extents.z, zone.Center.z + extents.z);
                points[i] = new Vector3(randomX, zone.Bounds[0].y, randomZ);
            }
            return points;
        }
        internal static ZoneObject GetActiveZone(BasePlayer player)
        {
            Script.Instance.ScriptCheck();
            return ZoneData.Instance.GetZones()?.LastOrDefault(zone => zone.Value != null && zone.Value.Players.Contains(player?.userID ?? 0)).Value;
        }
        internal static BasePlayer[] GetPlayersInZone(string zoneName)
        {
            Script.Instance.ScriptCheck();
            return ZoneData.Instance.GetZones()?.FirstOrDefault(zone => zone.Value != null && zone.Key == zoneName.ToLower()).Value.Players.Select(playerId => BasePlayer.Find(playerId.ToString())).Where(player => player != null).ToArray();
        }
        internal static void DrawZone(BasePlayer player, string zonePrefix, ZoneRect zone, int drawTimeMS = 1000, string drawColorHex = "#000000")
        {
            Script.Instance.ScriptCheck();
            if (!player.IsPlayer(true) || zone.Equals(default(ZoneRect))) { return; }
            System.Drawing.Color colorRGB = ColorTranslator.FromHtml(drawColorHex.StartsWith("#") ? drawColorHex : $"#{drawColorHex}");
            UnityEngine.Color color = new UnityEngine.Color(colorRGB.R / byte.MaxValue, colorRGB.G / byte.MaxValue, colorRGB.B / byte.MaxValue);

            float drawTime = (float)(drawTimeMS / 1000.0f);

            zonePrefix = !String.IsNullOrEmpty(zonePrefix) ? zonePrefix : "Unnamed";
            player.SendConsoleCommand("ddraw.text", drawTime, UnityEngine.Color.white, zone.Center, $"<size={32}>{zonePrefix}</size>");
            foreach (var point in zone.Bounds)
            {
                player.SendConsoleCommand("ddraw.sphere", drawTime, color, GetPoint(point, zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), 0.25f);
            }

            #region Box Lines Shape
            /*Size Faces X*/
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[0], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[5], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[1], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[4], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[1], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[6], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[2], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[5], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[2], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[7], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[3], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[6], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[0], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[7], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[3], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[4], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));


            /*Top-Bottom Faces X*/
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[0], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[2], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[1], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[3], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[4], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[6], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[5], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[7], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));


            /*Horizontal Lines*/
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[0], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[1], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[1], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[2], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[2], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[3], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[3], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[0], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[4], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[5], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[5], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[6], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[6], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[7], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[7], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[4], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));


            /*Vertical Lines*/
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[0], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[4], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[1], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[5], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[2], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[6], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            player.SendConsoleCommand("ddraw.line", drawTime, color, GetPoint(zone.Bounds[3], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)), GetPoint(zone.Bounds[7], zone.Center, Quaternion.Euler(0, zone.Rotation, 0)));
            #endregion Box Lines Shape
        }
    }
}