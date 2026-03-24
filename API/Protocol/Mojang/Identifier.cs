using System.Text.RegularExpressions;

using API.Protocol.Packets;

namespace API.Protocol.Mojang;

public class Identifier : IWriteToPackets
{
    public string Namespace { get; private set; }
    public string Value { get; private set; }

    public Identifier(string ns, string value)
    {
        ValidateNamespace(ns);
        ValidateValue(value);
        
        this.Namespace = ns;
        this.Value = value;
    }

    public Identifier(string value)
    {
        ValidateValue(value);
        
        this.Namespace = "minecraft";
        this.Value = value;
    }

    public static Identifier Parse(string identifier)
    {
        string ns = "prisma";
        string v = "null";
        
        string[] parts = identifier.Split(":");

        if (parts.Length == 1)
        {
            ns = "minecraft";
            v = parts[0];

            return new Identifier(ns, v);
        }

        return new Identifier(parts[0], parts[1]);
    }

    private void ValidateNamespace(string ns)
    {
        if (!Regex.IsMatch(ns, "^[a-z0-9._-]+$"))
        {
            throw new ArgumentException("Namespace contains invalid characters!");
        }
    }

    private void ValidateValue(string va)
    {
        if (!Regex.IsMatch(va, "^[a-z0-9._/-]+$"))
        {
            throw new ArgumentException("Value contains invalid characters!");
        }
    }
    
    public void WriteToPacket(Packet packet)
    { 
        packet.Write($"{this.Namespace}:{this.Value}");
    }

    public override string ToString()
    {
        ValidateNamespace(this.Namespace);
        ValidateValue(this.Value);

        return $"{this.Namespace}:{this.Value}";
    }

}