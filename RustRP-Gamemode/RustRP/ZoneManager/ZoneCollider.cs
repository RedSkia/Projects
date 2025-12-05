using UnityEngine;

namespace CoreRP.ZoneManager
{
    internal sealed class ZoneColliderBehaviour : MonoBehaviour
    {
        private BoxCollider boxCollider;
        private ZoneObject zoneObject;

        internal void Init(ZoneObject Zone)
        {
            Script.Instance.ScriptCheck();
            if (Zone == null || Zone.Rect.Equals(default(ZoneRect)) || zoneObject != null || boxCollider != null) { return; }
            this.enabled = true;
            this.gameObject.layer = (int)Rust.Layer.Trigger;
            this.zoneObject = Zone;
            this.transform.rotation = Quaternion.Euler(0, Zone.Rect.Rotation, 0);
            this.transform.position = Zone.Rect.Center;
            this.boxCollider = gameObject.AddComponent<BoxCollider>();
            this.boxCollider.size = Zone.Rect.Size;
            this.boxCollider.isTrigger = true;
            FileLogger.LogMessage($"Init ZoneCollider: \"{Zone.Prefix}\"", diskLog: false);
        }

        internal void Dispose()
        {
            FileLogger.LogMessage($"Kill ZoneCollider: \"{zoneObject.Prefix}\"", diskLog: false);
            if (boxCollider != null) { Destroy(boxCollider); }
            Destroy(this);
        }

        private void OnTriggerEnter(Collider collider)
        {
            Script.Instance.ScriptCheck();
            if(zoneObject == null) { Dispose(); return; }
            BaseEntity baseEntity = collider?.gameObject?.ToBaseEntity();
            BasePlayer player = baseEntity != null && (baseEntity is BasePlayer) ? (baseEntity as BasePlayer) : null;
            BaseVehicle mountable = baseEntity != null && (baseEntity is BaseVehicle) ? (baseEntity as BaseVehicle) : null;
            if (player.IsPlayer(true)) 
            {
                zoneObject.Players.Add(player.userID);
                FileLogger.LogMessage($"Player Enter Zone: \"{zoneObject.Prefix}\"", diskLog: false);
            }
 
            #region Mountable Vehicles
            if(FlagManager<ZoneFlagsAllowed>.Has(zoneObject.Flags, ZoneFlagsAllowed.Vehicles)) { return; }

            bool isVehicle = baseEntity.IsType(
              typeof(ModularCar),
              typeof(MiniCopter),
              typeof(ScrapTransportHelicopter),
              typeof(MotorRowboat),
              typeof(RHIB),
              typeof(BaseSubmarine)
            );
            bool isAnimal = baseEntity.IsType(
              typeof(RidableHorse)
            );

            if(mountable != null)
            {
                if(isVehicle && mountable.rigidBody != null)
                {
                    /*Remove Velocity*/
                    mountable.rigidBody.velocity = Vector3.zero;
                    mountable.rigidBody.angularVelocity = Vector3.zero;
                }
                if(isAnimal && mountable.HasComponent<BaseRidableAnimal>())
                {
                    /*Remove Velocity*/
                    mountable.GetComponent<BaseRidableAnimal>().currentSpeed = 0;
                }
                //mountable.transform.position += (-mountable.transform.forward * mountable.bounds.size.z); /*Teleport/Push Back*/

                Vector3 newPosition = mountable.transform.position - mountable.transform.forward * mountable.bounds.size.z;
                newPosition.y = mountable.transform.position.y; // Preserve the original Y position
                mountable.transform.position = newPosition; // Teleport to the new position
            }
            #endregion Mountable Vehicles
        }
        private void OnTriggerExit(Collider collider)
        {
            Script.Instance.ScriptCheck();
            if (zoneObject == null) { Dispose(); return; }
            BaseEntity baseEntity = collider?.gameObject?.ToBaseEntity();
            BasePlayer player = baseEntity != null && (baseEntity is BasePlayer) ? (baseEntity as BasePlayer) : null;
            if (player.IsPlayer(true)) 
            {
                zoneObject.Players.Remove(player.userID);
                FileLogger.LogMessage($"Player Exit Zone: \"{zoneObject.Players}\"", diskLog: false);
            }
        }
    }
}