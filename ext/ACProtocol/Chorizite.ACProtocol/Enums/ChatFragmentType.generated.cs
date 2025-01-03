//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
//                                                            //
//                          WARNING                           //
//                                                            //
//           DO NOT MAKE LOCAL CHANGES TO THIS FILE           //
//               EDIT THE .tt TEMPLATE INSTEAD                //
//                                                            //
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//


using System;

namespace Chorizite.ACProtocol.Enums {
    /// <summary>
    /// The ChatFragmentType categorizes chat window messages to control color and filtering.
    /// </summary>
    public enum ChatFragmentType : uint {
        Default = 0x00,

        Speech = 0x02,

        Tell = 0x03,

        OutgoingTell = 0x04,

        System = 0x05,

        Combat = 0x06,

        Magic = 0x07,

        Channels = 0x08,

        OutgoingChannel = 0x09,

        Social = 0x0A,

        OutgoingSocial = 0x0B,

        Emote = 0x0C,

        Advancement = 0x0D,

        Abuse = 0x0E,

        Help = 0x0F,

        Appraisal = 0x10,

        Spellcasting = 0x11,

        Allegiance = 0x12,

        Fellowship = 0x13,

        WorldBroadcast = 0x14,

        CombatEnemy = 0x15,

        CombatSelf = 0x16,

        Recall = 0x17,

        Craft = 0x18,

        Salvaging = 0x19,

        AdminTell = 0x1F,

    };
}
