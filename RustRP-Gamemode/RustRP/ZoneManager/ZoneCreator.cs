using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace CoreRP.ZoneManager
{
    internal static class ZoneCreator
    {
        internal static void ToggleTool(BasePlayer player, string zoneName = "")
        {
            Script.Instance.ScriptCheck();
            if (!player.IsPlayer(true)) { return; }
            var zone = ZoneData.Instance.GetZone(zoneName);

            ZoneCreatorBehaviour tool;
            player.TryGetComponent<ZoneCreatorBehaviour>(out tool);

            if (tool != null && (zone == null && String.IsNullOrEmpty(zoneName)))
            {
                tool.Dispose();
                player.SendMsg("ZoneTool", "Disabled");
                return;
            }
            if ((tool != null && zone == null && !String.IsNullOrEmpty(zoneName)) || (tool == null && zone == null && !String.IsNullOrEmpty(zoneName))) 
            {
                player.SendMsg("ZoneTool", $"Unable to load zone: \"{zoneName}\" not found");
                return;
            }
            if (tool == null && zone == null)
            {
                player.gameObject.AddComponent<ZoneCreatorBehaviour>();
                player.SendMsg("ZoneTool", "Enabled");
                return;
            }
            if (tool == null && zone != null)
            {
                player.gameObject.AddComponent<ZoneCreatorBehaviour>().Init(zone);
                zone.InitCollider();
                player.SendMsg("ZoneTool", $"Enabled and loaded zone: \"{zoneName}\"");
                return;
            }
            if (tool != null && zone != null)
            {
                tool.Init(zone);
                zone.InitCollider();
                player.SendMsg("ZoneTool", $"Enabled and loaded zone: \"{zoneName}\"");
                return;
            }
        }
        internal static void DisableTool()
        {
            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                try
                {
                    ZoneCreatorBehaviour tool = null;
                    player?.TryGetComponent<ZoneCreatorBehaviour>(out tool);
                    tool?.Dispose();
                } catch { continue; }
            }
        }
    }

    internal sealed class ZoneCreatorBehaviour : MonoBehaviour
    {
        private const int updateDelay = 100;
        private const int clickDelay = 100;
        private const float rotationInterval = 11.25f;
        private BasePlayer player;
        private bool isButtonPressed;
        private bool isRotateMode;
        private DateTime lastUpdate = DateTime.Now.AddMilliseconds(updateDelay);
        private DateTime lastClick = DateTime.Now.AddMilliseconds(clickDelay);
        private ZoneObject zoneObject;

        internal string zonePrefix { get; set; } = "zonePrefix";
        internal float zoneHeight { get; set; } = 10;
        internal float zoneRotation { get; set; } = 0; /*Y Axis*/
        internal Vector3[] zonePoints { get; set; } = new Vector3[4];
        internal ZoneFlagsAllowed zoneFlags = ZoneFlagsAllowed.None;

        private void Awake()
        {
            Script.Instance.ScriptCheck();
            gameObject.TryGetComponent<BasePlayer>(out this.player);
            if(!player.IsPlayer(true)) { Dispose(); }
        }
        internal void Init(ZoneObject Zone)
        {
            Script.Instance.ScriptCheck();
            if (Zone == null || Zone.Rect.Equals(default(ZoneRect))) { Dispose(); return; }
            this.zoneObject = Zone;
            this.zonePrefix = Zone.Prefix;
            this.zoneHeight = Zone.Rect.Height;
            this.zoneRotation = Zone.Rect.Rotation;
            this.zoneFlags = Zone.Flags;
            this.zonePoints = new Vector3[]
            {
                Zone.Rect.Points[0],
                Zone.Rect.Points[1],
                Zone.Rect.Points[2],
                Zone.Rect.Points[3],
            };
        }
        private void Update()
        {
            Script.Instance.ScriptCheck();
            if (!player.IsPlayer(true)) { Dispose(); return; }

            RaycastHit hit;
            var raycast = Physics.Raycast(player.eyes.HeadRay(), out hit, 20);
            var point = raycast ? hit.point : Vector3.zero;

            bool mouse1 = !isButtonPressed && player.serverInput.WasJustPressed(BUTTON.FIRE_PRIMARY);
            bool mouse2 = !isButtonPressed && player.serverInput.WasJustPressed(BUTTON.FIRE_SECONDARY);
            bool keyE = !isButtonPressed && player.serverInput.WasJustPressed(BUTTON.USE);
            bool keyR = !isButtonPressed && player.serverInput.WasJustPressed(BUTTON.RELOAD);
            bool keyShift = !isButtonPressed && player.serverInput.WasJustPressed(BUTTON.SPRINT);

            var pointsPlaced = zonePoints.Where(x => x != Vector3.zero).Count();

            if (DateTime.Now > lastClick)
            {
                /*Place point*/
                if (mouse1 && pointsPlaced != zonePoints.Length)
                {
                    lastClick = DateTime.Now.AddMilliseconds(clickDelay); isButtonPressed = true;
                    int i = (pointsPlaced < zonePoints.Length - 1 ? pointsPlaced : zonePoints.Length - 1);
                    zonePoints[i] = point;
                }

                /*Remove last point*/
                if (mouse2)
                {
                    lastClick = DateTime.Now.AddMilliseconds(clickDelay); isButtonPressed = true;
                    int i = (pointsPlaced - 1 > 0 ? pointsPlaced - 1 : 0);
                    player.SendConsoleCommand("ddraw.sphere", 1.0f, UnityEngine.Color.red, zonePoints[i], 0.25f);
                    zonePoints[i] = Vector3.zero;
                }

                /*Swap Mode*/
                if (keyShift)
                {
                    lastClick = DateTime.Now.AddMilliseconds(clickDelay); isButtonPressed = true;
                    isRotateMode = !isRotateMode; /*Opposite*/
                    string msg = isRotateMode ?  "Rotate Mode" : "Height Mode";
                    player.ShowGametip(msg, GametipType.Info);
                }

                /*Extend height*/
                if (keyE)
                {
                    zoneObject?.Dispose();
                    lastClick = DateTime.Now.AddMilliseconds(clickDelay); isButtonPressed = true;
                    switch (isRotateMode)
                    {
                        case true: { zoneRotation = Math.Abs(zoneRotation - rotationInterval) >= 360 ? 0 : zoneRotation -= rotationInterval; } break;
                        case false: { zoneHeight += 1; } break;
                    }
                }

                /*Reduce height*/
                if (keyR)
                {
                    zoneObject?.Dispose();
                    lastClick = DateTime.Now.AddMilliseconds(clickDelay); isButtonPressed = true;
                    switch (isRotateMode)
                    {
                        case true: { zoneRotation = (zoneRotation + rotationInterval) >= 360 ? 0 : zoneRotation += rotationInterval; } break;
                        case false: { zoneHeight = zoneHeight > 1 ? zoneHeight - 1 : 1; } break;
                    }
                }
            }
            if (!mouse1 && !mouse2 && !keyE && !keyR) { isButtonPressed = false; } /*Reset Key Press*/

            if (DateTime.Now < lastUpdate) { return; }
            lastUpdate = DateTime.Now.AddMilliseconds(updateDelay);
            float drawTime = (float)updateDelay / 1000.0f;
            if (pointsPlaced == zonePoints.Length)
            {
                var rect = new ZoneRect(zonePoints.ToArray(), zoneHeight, zoneRotation);
                ZoneHelper.DrawZone(player, zonePrefix, rect, updateDelay);
            }
            else
            {
                zoneObject?.Dispose();
                player.SendConsoleCommand("ddraw.sphere", drawTime, UnityEngine.Color.red, point, 0.25f);
                for (int i = 0; i < zonePoints.Length; i++)
                {
                    var rectPoint = zonePoints[i];
                    player.SendConsoleCommand("ddraw.sphere", drawTime, UnityEngine.Color.green, rectPoint, 0.25f);
                    player.SendConsoleCommand("ddraw.text", drawTime, UnityEngine.Color.white, rectPoint, $"<size={32}>{i+1}</size>");
                }
            }
        }
        public void Dispose() => Destroy(this);
    }
}