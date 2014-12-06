namespace AnchovyMultiPing
{
    public abstract class AbstractMultiPing : IMultiPings
    {
        public abstract IMultiPings RequestTime();
        public abstract string GetPingInformation();
        public abstract string GetIp();
    }
}
