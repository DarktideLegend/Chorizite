using System.IO;
using System.Collections.Generic;
using MagicHat.ACProtocol.Enums;
using MagicHat.ACProtocol.Messages;
using MagicHat.ACProtocol.Types;
using MagicHat.ACProtocol.Extensions;
using System.Numerics;

namespace MagicHat.ACProtocol.Messages.S2C.Events {
    /// <summary>
    /// Update multiple enchantments from your character.
    /// </summary>
    public class Magic_UpdateMultipleEnchantments : ACGameEvent {
        /// <summary>
        /// List of enchantments getting updated
        /// </summary>
        public List<Enchantment> Enchantments = new();

        /// <summary>
        /// Reads instance data from a binary reader
        /// </summary>
        public override void Read(BinaryReader reader) {
            base.Read(reader);
            Enchantments = reader.ReadPackableList<Enchantment>();
        }

        /// <summary>
        /// Writes instance data to a binary writer
        /// </summary>
        public override void Write(BinaryWriter writer) {
            base.Write(writer);
            writer.WritePackableList(Enchantments);
        }

    }

}