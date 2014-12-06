using System;

namespace AnchovyMultiPing
{
    public interface IMultiPings
    {
        IMultiPings RequestTime();
        string GetPingInformation();
        String GetIp();
    }
}
