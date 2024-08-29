using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PacketDotNet;

public abstract class IPOption
{
    public abstract IPOptionType IPOptionType { get;  }
    public abstract byte[] Bytes { get; }
    public abstract int Length { get; }
    
}

public sealed class Rfc1770IPOption : IPOption
{
   
    public sealed override IPOptionType IPOptionType { get; }
    
    public override byte[] Bytes { get; }
    public override int Length => Bytes.Length;

    public IPAddress[] Destinations { get;  }

    public Rfc1770IPOption(IPAddress[] destinations)
    {
        if (destinations.Length > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(destinations), "The maximum number of destinations is 9");
        }
        Destinations = destinations;
        IPOptionType = IPOptionType.Rfc1770;
        Bytes = GetBytes();
    }

    private  byte[] GetBytes()
    {
        var bytes = new List<byte>();
        //Rfc1770 Option type is 149
        bytes.Add(149); 
        var length = (byte)(2 + Destinations.Length * 4);
        bytes.Add(length);
        foreach (var destination in Destinations)
        {
            bytes.AddRange(destination.GetAddressBytes());
        }
        var reminder = bytes.Count % 4;
        if (reminder != 0)
        {
            bytes.AddRange(new byte[4 - reminder]);
        }

        return bytes.ToArray();
    }

    public static IPOption Create(byte[] bytes)
    {
        if (bytes[0] != 149)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), "Invalid Rfc1770 option type");
        }

        var length = bytes[1];
        if (length < 2 || length > 38 )
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), "Invalid Rfc1770 option length");
        }

        var destinations = new List<IPAddress>();
        for (var i = 2; i < length; i += 4)
        {
            var destination = new byte[4];
            Array.Copy(bytes, i, destination, 0, 4);
            destinations.Add(new IPAddress(destination));
        }

        return new Rfc1770IPOption(destinations.ToArray());
    }   
}

public sealed class IPOptionType
{
    public static readonly IPOptionType EndOfOptionsList = new IPOptionType(0, "End of Options List");
    public static readonly IPOptionType NoOperation = new IPOptionType(1, "No Operation");
    public static readonly IPOptionType Security = new IPOptionType(130, "Security");
    public static readonly IPOptionType LooseSourceRouting = new IPOptionType(131, "Loose Source Routing");
    public static readonly IPOptionType StrictSourceRouting = new IPOptionType(137, "Strict Source Routing");
    public static readonly IPOptionType RecordRoute = new IPOptionType(7, "Record Route");
    public static readonly IPOptionType StreamId = new IPOptionType(136, "Stream ID");
    public static readonly IPOptionType TimeStamp = new IPOptionType(68, "Time Stamp");
    public static readonly IPOptionType QuickStart = new IPOptionType(25, "Quick-Start");
    public static readonly IPOptionType Unknown = new IPOptionType(0, "Unknown");
    public static readonly IPOptionType Rfc1770 = new IPOptionType(149, "RFC 1770");

    private static readonly Dictionary<byte, IPOptionType> _types = new Dictionary<byte, IPOptionType>();

    static IPOptionType()
    {
        _types.Add(EndOfOptionsList.Number, EndOfOptionsList);
        _types.Add(NoOperation.Number, NoOperation);
        _types.Add(Security.Number, Security);
        _types.Add(LooseSourceRouting.Number, LooseSourceRouting);
        _types.Add(StrictSourceRouting.Number, StrictSourceRouting);
        _types.Add(RecordRoute.Number, RecordRoute);
        _types.Add(StreamId.Number, StreamId);
        _types.Add(TimeStamp.Number, TimeStamp);
        _types.Add(QuickStart.Number, QuickStart);
        _types.Add(Rfc1770.Number,Rfc1770);
    }

    public static IPOptionType GetType(byte type)
    {
        if (_types.ContainsKey(type))
        {
            return _types[type];
        }

        return Unknown;
    }

    public byte Number { get; private set; }
    public string Name { get; private set; }

    private IPOptionType(byte number, string name)
    {
        Number = number;
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}
