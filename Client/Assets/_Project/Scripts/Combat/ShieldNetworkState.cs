using System;
using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Combat
{
    public struct ShieldNetworkState : INetworkSerializable, IEquatable<ShieldNetworkState>
    {
        public float Front;
        public float Rear;
        public float Port;
        public float Starboard;

        public float this[ShieldFacing facing]
        {
            get
            {
                switch (facing)
                {
                    case ShieldFacing.Front: return Front;
                    case ShieldFacing.Rear: return Rear;
                    case ShieldFacing.Port: return Port;
                    case ShieldFacing.Starboard: return Starboard;
                    default: return 0f;
                }
            }
            set
            {
                switch (facing)
                {
                    case ShieldFacing.Front: Front = value; break;
                    case ShieldFacing.Rear: Rear = value; break;
                    case ShieldFacing.Port: Port = value; break;
                    case ShieldFacing.Starboard: Starboard = value; break;
                }
            }
        }

        public static ShieldNetworkState Full(float maxPerFacing) =>
            new ShieldNetworkState
            {
                Front = maxPerFacing,
                Rear = maxPerFacing,
                Port = maxPerFacing,
                Starboard = maxPerFacing
            };

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Front);
            serializer.SerializeValue(ref Rear);
            serializer.SerializeValue(ref Port);
            serializer.SerializeValue(ref Starboard);
        }

        public bool Equals(ShieldNetworkState other) =>
            Mathf.Approximately(Front, other.Front)
            && Mathf.Approximately(Rear, other.Rear)
            && Mathf.Approximately(Port, other.Port)
            && Mathf.Approximately(Starboard, other.Starboard);
    }
}
