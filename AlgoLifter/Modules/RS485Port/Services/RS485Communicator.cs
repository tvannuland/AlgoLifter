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
            return comPortIsOpen;
        }

        public byte[] recievedData()
        {
            if (!comPortIsOpen)
                return null;

            byte[] buffer = new byte[serialPort.BytesToRead];

            for (int i = 0; i < serialPort.BytesToRead; i++)
            {
                buffer[i] = (byte) serialPort.ReadChar();
            }
            return buffer;
        }

        public void sendData(byte[] data)
        { //TODO: Refactor to read Status and result of sent command. Handling of answered Requests should be made possible!
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
            if (senderPort.BytesToRead > 8)
                ea.GetEvent<Infrastructure.SerialDataReceivedEvent>().Publish(true);
        }
    }
}
