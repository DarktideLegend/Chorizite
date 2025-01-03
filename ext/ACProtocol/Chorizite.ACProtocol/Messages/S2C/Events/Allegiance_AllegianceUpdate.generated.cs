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
    /// Returns info related to your monarch, patron and vassals.
    /// </summary>
    public class Allegiance_AllegianceUpdate : ACGameEvent {
        public uint Rank;

        public AllegianceProfile Profile;

        /// <summary>
        /// Reads instance data from a binary reader
        /// </summary>
        public override void Read(BinaryReader reader) {
            base.Read(reader);
            Rank = reader.ReadUInt32();
            Profile = new AllegianceProfile();
            Profile.Read(reader);
        }

        /// <summary>
        /// Writes instance data to a binary writer
        /// </summary>
        public override void Write(BinaryWriter writer) {
            base.Write(writer);
            writer.Write(Rank);
            Profile.Write(writer);
        }

    }

}
