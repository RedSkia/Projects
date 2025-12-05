using System;

namespace CoreRP.ZoneManager
{
    [Flags]
    internal enum ZoneFlagsAllowed : ulong
    {
        None = 0,
        Building = 1 << 0,
        Damage = 1 << 1,
        Interact = 1 << 2,
        Vehicles = 1 << 3,
    }
}