using System.IO;
using System.Collections.Generic;
using Chorizite.ACProtocol.Enums;
using Chorizite.ACProtocol.Messages;
using Chorizite.ACProtocol.Types;
using Chorizite.ACProtocol.Extensions;
using Chorizite.Common.Enums;
using System.Numerics;

namespace Chorizite.ACProtocol.Messages.S2C.Events {
    /// <summary>
    /// Equip an item.
    /// </summary>
    public class Item_WearItem : ACGameEvent {
        /// <summary>
        /// the object Id of the item being equipped
        /// </summary>
        public uint ObjectId;

        /// <summary>
        /// the slot(s) the item uses
        /// </summary>
        public EquipMask Slot;

        /// <summary>
        /// Reads instance data from a binary reader
        /// </summary>
        public override void Read(BinaryReader reader) {
            base.Read(reader);
            ObjectId = reader.ReadUInt32();
            Slot = (EquipMask)reader.ReadUInt32();
        }

        /// <summary>
        /// Writes instance data to a binary writer
        /// </summary>
        public override void Write(BinaryWriter writer) {
            base.Write(writer);
            writer.Write(ObjectId);
            writer.Write((uint)Slot);
        }

    }

}
