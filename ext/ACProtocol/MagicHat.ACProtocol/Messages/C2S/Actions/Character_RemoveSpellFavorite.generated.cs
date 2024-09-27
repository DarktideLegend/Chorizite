using System.IO;
using System.Collections.Generic;
using MagicHat.ACProtocol.Enums;
using MagicHat.ACProtocol.Messages;
using MagicHat.ACProtocol.Types;
using MagicHat.ACProtocol.Extensions;
using System.Numerics;

namespace MagicHat.ACProtocol.Messages.C2S.Actions {
    /// <summary>
    /// Remove a spell from a spell bar.
    /// </summary>
    public class Character_RemoveSpellFavorite : ACGameAction {
        /// <summary>
        /// The spell&#39;s Id
        /// </summary>
        public LayeredSpellId SpellId;

        /// <summary>
        /// The spell bar number
        /// </summary>
        public uint SpellBar;

        /// <summary>
        /// Reads instance data from a binary reader
        /// </summary>
        public override void Read(BinaryReader reader) {
            base.Read(reader);
            SpellId = new LayeredSpellId();
            SpellId.Read(reader);
            SpellBar = reader.ReadUInt32();
        }

        /// <summary>
        /// Writes instance data to a binary writer
        /// </summary>
        public override void Write(BinaryWriter writer) {
            base.Write(writer);
            SpellId.Write(writer);
            writer.Write(SpellBar);
        }

    }

}