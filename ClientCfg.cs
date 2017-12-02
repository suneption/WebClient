using System.Net;

public class ClientCfg 
{
    public string IpAddressRaw { get; set; }
    public IPAddress IpAddress { get { return IPAddress.Parse(IpAddressRaw); } }
    public int Port { get; set; }
    public IPEndPoint EndPoint { get { return new IPEndPoint(IpAddress, Port); } }
}