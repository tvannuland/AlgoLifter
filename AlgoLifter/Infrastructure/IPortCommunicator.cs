using System.Collections.Generic;

namespace AlgoLifter.Infrastructure
{
    public interface IPortCommunicator
    {
        void setComPort(string name);
        bool isOpen();
        byte[] recievedData();
        void sendData(byte[] data);
        List<string> getComPorts();
        void closeComPort();
    }
}