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
    /// Set or update a Character Skill state
    /// </summary>
    public class Qualities_PrivateUpdateSkillAC : ACS2CMessage {
        /// <inheritdoc />
        public override uint OpCode => 0x02E1;

        /// <inheritdoc />
        public override S2CMessageType MessageType => S2CMessageType.Qualities_PrivateUpdateSkillAC;

        /// <summary>
        /// sequence number
        /// </summary>
        public byte Sequence;

        /// <summary>
        /// skill Id
        /// </summary>
        public SkillId Key;

        /// <summary>
        /// skill state
        /// </summary>
        public SkillAdvancementClass Value;

        /// <summary>
        /// Reads instance data from a binary reader
        /// </summary>
        public override void Read(BinaryReader reader) {
            base.Read(reader);
            Sequence = reader.ReadByte();
            Key = (SkillId)reader.ReadInt32();
            Value = (SkillAdvancementClass)reader.ReadUInt32();
        }

        /// <summary>
        /// Writes instance data to a binary writer
        /// </summary>
        public override void Write(BinaryWriter writer) {
            base.Write(writer);
            writer.Write(Sequence);
            writer.Write((int)Key);
            writer.Write((uint)Value);
        }

    }

}
