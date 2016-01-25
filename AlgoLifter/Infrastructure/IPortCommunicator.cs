using System.Collections.Generic;

namespace AlgoLifter.Infrastructure
{
    public interface IPortCommunicator
    {
        void setComPort(string name, int rate);
        void setComPort(string name);
        bool isOpen();
        byte[] recievedData();
        byte[] sendData(byte[] data);
        List<string> getComPorts();
        void closeComPort();
    }
}