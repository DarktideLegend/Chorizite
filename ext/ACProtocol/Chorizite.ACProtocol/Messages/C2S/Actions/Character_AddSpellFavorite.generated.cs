using System.IO;
using System.Collections.Generic;
using Chorizite.ACProtocol.Enums;
using Chorizite.ACProtocol.Messages;
using Chorizite.ACProtocol.Types;
using Chorizite.ACProtocol.Extensions;
using Chorizite.Common.Enums;
using System.Numerics;

namespace Chorizite.ACProtocol.Messages.C2S.Actions {
    /// <summary>
    /// Add a spell to a spell bar.
    /// </summary>
    public class Character_AddSpellFavorite : ACGameAction {
        /// <summary>
        /// The spell&#39;s Id
        /// </summary>
        public LayeredSpellId SpellId;

        /// <summary>
        /// Position on the spell bar where the spell is to be added
        /// </summary>
        public uint Index;

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
            Index = reader.ReadUInt32();
            SpellBar = reader.ReadUInt32();
        }

        /// <summary>
        /// Writes instance data to a binary writer
        /// </summary>
        public override void Write(BinaryWriter writer) {
            base.Write(writer);
            SpellId.Write(writer);
            writer.Write(Index);
            writer.Write(SpellBar);
        }

    }

}
