using Microsoft.Practices.ServiceLocation;
using Prism.Events;
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
        static IEventAggregator ea;
        bool comPortIsOpen = false;
        string comPortName;
        SerialPort serialPort;

        public RS485Communicator()
        {
            ea = ServiceLocator.Current.GetInstance<IEventAggregator>();
            serialPort = new SerialPort();
        }

        public void closeComPort()
        {
            serialPort.Close();
            comPortIsOpen = false;
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
            try
            {
                serialPort.Write(data, 0, data.Length);
            }
            finally
            {
                serialPort.Close();
                comPortIsOpen = false;
            }
        }

        public void setComPort(string name)
        {
            try {
                serialPort.WriteTimeout = 100;
                serialPort.ReadTimeout = 100;
                serialPort.PortName = name;
                serialPort.BaudRate = 9600;
                if (serialPort.IsOpen)
                    serialPort.Close();
                serialPort.Open();
                comPortIsOpen = true;
                serialPort.DataReceived += onDataRecieved;
            }
            finally {
                serialPort.Close();
                comPortIsOpen = false;
            }
        }

        static void onDataRecieved(object sender, SerialDataReceivedEventArgs eargs)
        {
            SerialPort senderPort = (SerialPort) sender;
            if (senderPort == null) throw new ArgumentNullException("senderPort");
            if (senderPort.BytesToRead > 7)
                ea.GetEvent<Infrastructure.SerialDataReceivedEvent>().Publish(true);
        }
    }
}
