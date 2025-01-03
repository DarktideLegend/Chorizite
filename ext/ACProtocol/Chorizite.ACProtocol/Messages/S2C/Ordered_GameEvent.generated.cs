using System.IO;
using System.Collections.Generic;
using Chorizite.ACProtocol.Enums;
using Chorizite.ACProtocol.Messages;
using Chorizite.ACProtocol.Types;
using Chorizite.ACProtocol.Extensions;
using Chorizite.Common.Enums;
using System.Numerics;

namespace Chorizite.ACProtocol.Messages.S2C {
    /// <summary>
    /// Ordered game event
    /// </summary>
    public class Ordered_GameEvent : ACS2CMessage {
        /// <inheritdoc />
        public override uint OpCode => 0xF7B0;

        /// <inheritdoc />
        public override S2CMessageType MessageType => S2CMessageType.Ordered_GameEvent;

        public uint OrderedObjectId;

        public uint OrderedSequence;

        public GameEventType EventType;

        /// <summary>
        /// Reads instance data from a binary reader
        /// </summary>
        public override void Read(BinaryReader reader) {
            base.Read(reader);
            OrderedObjectId = reader.ReadUInt32();
            OrderedSequence = reader.ReadUInt32();
            EventType = (GameEventType)reader.ReadUInt32();
        }

        /// <summary>
        /// Writes instance data to a binary writer
        /// </summary>
        public override void Write(BinaryWriter writer) {
            base.Write(writer);
            writer.Write(OrderedObjectId);
            writer.Write(OrderedSequence);
            writer.Write((uint)EventType);
        }

    }

}
