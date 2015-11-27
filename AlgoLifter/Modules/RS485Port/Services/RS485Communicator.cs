using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLifter.Modules.RS485Port.Services
{
    public class RS485Communicator : Infrastructure.IPortCommunicator
    {
        public RS485Communicator()
        {

        }

        public void closeComPort()
        {
            throw new NotImplementedException();
        }

        public List<string> getComPorts()
        {
            return SerialPort.GetPortNames().ToList();
        }

        public bool isOpen()
        {
            throw new NotImplementedException();
        }

        public byte[] recievedData()
        {
            throw new NotImplementedException();
        }

        public void sendData(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void setComPort(string name)
        {
            throw new NotImplementedException();
        }
    }
}
