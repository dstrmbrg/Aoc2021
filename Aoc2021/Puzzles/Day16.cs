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
            .Pipe(ToBitString)
            .Pipe(ToBitStream)
            .Pipe(x => ParsePacket(x, out _));

        var allSubPackets = packet.SubPackets.Concat(packet.SubPackets.SelectManyRecursive(x => x.SubPackets)).ToList();

        return packet.Version + allSubPackets.Sum(x => x.Version);
    }

    public override object PartTwo() =>
        GetInput()
            .Pipe(ToBitString)
            .Pipe(ToBitStream)
            .Pipe(x => ParsePacket(x, out _))
            .GetValue();

    private Packet ParsePacket(BitStream bitStream, out int packetSize)
    {
        var packet = new Packet(bitStream.Next(3), bitStream.Next(3));

        if (packet.Type == Operator.LiteralValue)
            packet.Value = ParseLiteralValue(bitStream, out packetSize);
        else
            ParseSubPackets(bitStream, packet, out packetSize);

        return packet;
    }

    private static long ParseLiteralValue(BitStream bitStream, out int packetSize)
    {
        packetSize = 6;
        var result = 0L;

        while (true)
        {
            packetSize += 5;
            var value = bitStream.Next(5);
            result = (result << 4) | (uint)(value & 0xF);
            
            if (value >> 4 == 0)
                break;
        }
        
        return result;
    }

    private void ParseSubPackets(BitStream bitStream, Packet packet, out int packetSize)
    {
        var lengthType = bitStream.Next(1);
        var (remainingBits, remainingSubPackets, headerSize) = GetOperatorPacketInfo(bitStream, lengthType);
        packetSize = headerSize;

        while (remainingBits is > 0 || remainingSubPackets is > 0)
        {
            var subPacket = ParsePacket(bitStream, out var subPacketSize);
            packet.SubPackets.Add(subPacket);
            packetSize += subPacketSize;

            if (remainingSubPackets.HasValue) remainingSubPackets--;
            if (remainingBits.HasValue) remainingBits -= subPacketSize;
        }
    }

    private static (int? numberOfBits, int? numberOfSubPackets, int headerSize) GetOperatorPacketInfo(BitStream bitStream, int lengthType)
    {
        return lengthType == 0 ? (bitStream.Next(15), null, 22) : (null, bitStream.Next(11), 18);
    }

    private static string ToBitString(string hexString)
    {
        return hexString.Aggregate(string.Empty,
            (current, c) => current + Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'));
    }

    private static BitStream ToBitStream(string bitString) => new(bitString);
    private string GetInput() => Utilities.GetInput(GetType());

    private class Packet
    {
        public readonly Operator Type;
        public readonly int Version;
        public long Value;
        public readonly IList<Packet> SubPackets;

        public Packet(int version, int type)
        {
            Version = version;
            Type = (Operator)type;
            SubPackets = new List<Packet>();
        }

        public long GetValue()
        {
            return Type switch
            {
                Operator.LiteralValue => Value,
                Operator.Sum => SubPackets.Sum(x => x.GetValue()),
                Operator.Product => SubPackets.Aggregate(1L, (current, sp) => current * sp.GetValue()),
                Operator.Min => SubPackets.Min(x => x.GetValue()),
                Operator.Max => SubPackets.Max(x => x.GetValue()),
                Operator.GreaterThan => SubPackets[0].GetValue() > SubPackets[1].GetValue() ? 1 : 0,
                Operator.LessThan => SubPackets[0].GetValue() < SubPackets[1].GetValue() ? 1 : 0,
                Operator.Equal => SubPackets[0].GetValue() == SubPackets[1].GetValue() ? 1 : 0,
                _ => 0
            };
        }
    }

    private class BitStream
    {
        private int _current;
        private readonly string _bitString;

        public BitStream(string bitString)
        {
            _bitString = bitString;
        }

        public int Next(int bits) => Convert.ToInt32(_bitString[_current..(_current += bits)], 2);
    }

    private enum Operator
    {
        Sum,
        Product,
        Min,
        Max,
        LiteralValue,
        GreaterThan,
        LessThan,
        Equal
    }
}