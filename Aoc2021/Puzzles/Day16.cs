using System;
using System.Collections.Generic;
using System.Linq;
using WinstonPuckett.PipeExtensions;

namespace Aoc2021.Puzzles;

internal class Day16 : Puzzle
{
    public override object PartOne()
    {
        var packet = GetInput()
            .Pipe(ConvertToBitString)
            .Pipe(x => ParsePacket(x, out _));

        var allSubPackets = packet.SubPackets.Concat(packet.SubPackets.SelectManyRecursive(x => x.SubPackets)).ToList();

        return packet.Version + allSubPackets.Sum(x => x.Version);
    }

    public override object PartTwo() =>
        GetInput()
            .Pipe(ConvertToBitString)
            .Pipe(x => ParsePacket(x, out _))
            .GetValue();

    private Packet ParsePacket(string bitString, out int packetSize)
    {
        var packetVersion = Convert.ToInt32(bitString[..3], 2);
        var type = Convert.ToInt32(bitString.Substring(3, 3), 2);
        var packet = new Packet(packetVersion, type);
        
        if (type == 4)
        {
            packet.Value = ParseLiteralValue(bitString, out packetSize);
        }
        else
        {
            ParseOperatorPacket(bitString, packet, out packetSize);
        }

        return packet;
    }

    private static long ParseLiteralValue(string bitString, out int packetSize)
    {
        var currentIndex = 6;
        var resultBitString = string.Empty;

        while (bitString[currentIndex] == '1')
        {
            resultBitString += bitString.Substring(currentIndex + 1, 4);
            currentIndex += 5;
        }

        resultBitString += bitString.Substring(currentIndex + 1, 4);
        packetSize = currentIndex + 5;

        return Convert.ToInt64(resultBitString, 2);
    }

    private void ParseOperatorPacket(string bitString, Packet packet, out int packetSize)
    {
        var lengthType = int.Parse(bitString[6].ToString());
        var (remainingBits, remainingSubPackets, headerSize) = GetOperatorPacketInfo(bitString, lengthType);
        packetSize = headerSize;
        bitString = string.Join("", bitString.Skip(headerSize));

        while (remainingBits is > 0 || remainingSubPackets is > 0)
        {
            var subPacket = ParsePacket(bitString, out var subPacketSize);

            packet.SubPackets.Add(subPacket);
            packetSize += subPacketSize;
            bitString = string.Join("", bitString.Skip(subPacketSize));

            if (remainingSubPackets.HasValue) remainingSubPackets--;
            if (remainingBits.HasValue) remainingBits -= subPacketSize;
        }
    }

    private static (int? numberOfBits, int? numberOfSubPackets, int headerSize) GetOperatorPacketInfo(string bitString, int lengthType)
    {
        if (lengthType == 0)
            return (Convert.ToInt32(bitString.Substring(7, 15), 2), null, 22);

        return (null, Convert.ToInt32(bitString.Substring(7, 11), 2), 18);
    }

    private static string ConvertToBitString(string hexString)
    {
        return hexString.Aggregate(string.Empty,
            (current, c) => current + Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'));
    }

    private string GetInput()
    {
        return Utilities.GetInput(GetType());
    }

    private class Packet
    {
        private readonly int _type;
        public readonly int Version;
        public long Value;
        public readonly IList<Packet> SubPackets;

        public Packet(int version, int type)
        {
            Version = version;
            _type = type;
            SubPackets = new List<Packet>();
        }

        public long GetValue()
        {
            return _type switch
            {
                4 => Value,
                0 => SubPackets.Sum(x => x.GetValue()),
                1 => SubPackets.Aggregate(1L, (current, sp) => current * sp.GetValue()),
                2 => SubPackets.Min(x => x.GetValue()),
                3 => SubPackets.Max(x => x.GetValue()),
                5 => SubPackets[0].GetValue() > SubPackets[1].GetValue() ? 1 : 0,
                6 => SubPackets[0].GetValue() < SubPackets[1].GetValue() ? 1 : 0,
                7 => SubPackets[0].GetValue() == SubPackets[1].GetValue() ? 1 : 0,
                _ => 0
            };
        }
    }
}