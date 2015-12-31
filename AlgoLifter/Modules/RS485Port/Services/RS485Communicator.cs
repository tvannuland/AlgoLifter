using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlgoLifter.Modules.RS485Port.Services
{
    public class RS485Communicator : Infrastructure.IPortCommunicator
    {
        static IEventAggregator ea;
        bool comPortIsOpen = false;
        string comPortName;
        SerialPort serialPort;
        private static bool datagramAvailable = false;

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
            if (serialPort.BytesToRead < 9)
                throw new ArgumentOutOfRangeException("BytesToRead Datagram Size not 9");

            byte[] buffer = new byte[serialPort.BytesToRead];

            serialPort.Read(buffer, 0, 9);
            //for (int i = 0; i < serialPort.BytesToRead; i++)
            //{
            //    buffer[i] = (byte) serialPort.ReadChar();
            //}

            datagramAvailable = false;
            return buffer;
        }

        public byte[] sendData(byte[] data)
        {
            if (serialPort == null)
                return null;
            try
            {
                serialPort.Write(data, 0, data.Length);
                int i = 0;
                while (!datagramAvailable)
                {
                    Thread.Sleep(10);
                    if (i++ > 15)
                        return null;
                }

                return recievedData();
            }
            catch
            {
                serialPort.Close();
                comPortIsOpen = false;
            }
            return null;
        }

        public void setComPort(string name)
        {
            try {
                serialPort.WriteTimeout = 300;
                serialPort.ReadTimeout = 300;
                serialPort.PortName = name;
                serialPort.BaudRate = 9600;
                serialPort.ReceivedBytesThreshold = 9;
                if (serialPort.IsOpen)
                    serialPort.Close();
                serialPort.Open();
                comPortIsOpen = true;
                serialPort.DataReceived += onDataRecieved;
            }
            catch {
                serialPort.Close();
                comPortIsOpen = false;
            }
        }

        static void onDataRecieved(object sender, SerialDataReceivedEventArgs eargs)
        {
            SerialPort senderPort = (SerialPort) sender;
            if (senderPort == null) throw new ArgumentNullException("senderPort");
            if (senderPort.BytesToRead > 8)
            {
                datagramAvailable = true;
                ea.GetEvent<Infrastructure.SerialDataReceivedEvent>().Publish(true);
            }
        }
    }
}
