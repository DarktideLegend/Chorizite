﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using MagicHat.ACProtocol.Packets;

namespace MagicHat.ACProtocol.Lib.PCap {
	public class PCapPacket {
		public PCapPacketHeader Header { get; }
		public DateTime Timestamp { get; set; }
		public bool IsC2S => Header.SourcePort == 12345;

		public PCapPacket(DateTime utcNow, BinaryReader reader, uint capturedLength) {
			Header = new PCapPacketHeader(reader);
		}
	}
}