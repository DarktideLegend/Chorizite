using System.IO;
using System.Collections.Generic;
using Chorizite.ACProtocol.Enums;
using Chorizite.ACProtocol.Messages;
using Chorizite.ACProtocol.Types;
using Chorizite.ACProtocol.Extensions;
using System.Numerics;

namespace Chorizite.ACProtocol.Messages.S2C {
    /// <summary>
    /// Set or update a Character Attribute value
    /// </summary>
    public class Qualities_PrivateUpdateAttribute : ACS2CMessage {
        /// <inheritdoc />
        public override uint OpCode => 0x02E3;

        /// <inheritdoc />
        public override S2CMessageType MessageType => S2CMessageType.Qualities_PrivateUpdateAttribute;

        /// <summary>
        /// sequence number
        /// </summary>
        public byte Sequence;

        /// <summary>
        /// attribute Id
        /// </summary>
        public AttributeId Key;

        /// <summary>
        /// attribute information
        /// </summary>
        public AttributeInfo Value;

        /// <summary>
        /// Reads instance data from a binary reader
        /// </summary>
        public override void Read(BinaryReader reader) {
            base.Read(reader);
            Sequence = reader.ReadByte();
            Key = (AttributeId)reader.ReadUInt32();
            Value = new AttributeInfo();
            Value.Read(reader);
        }

        /// <summary>
        /// Writes instance data to a binary writer
        /// </summary>
        public override void Write(BinaryWriter writer) {
            base.Write(writer);
            writer.Write(Sequence);
            writer.Write((uint)Key);
            Value.Write(writer);
        }

    }

}