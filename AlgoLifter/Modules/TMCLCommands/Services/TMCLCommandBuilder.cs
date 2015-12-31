using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgoLifter.Infrastructure;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Microsoft.Practices.Unity;

namespace AlgoLifter.Modules.TMCLCommands.Services
{
    public class TMCLCommandBuilder : Infrastructure.ICommandBuilder
    {
        private IEventAggregator ea;
        private IUnityContainer container;

        public TMCLCommandBuilder()
        {
            ea = ServiceLocator.Current.GetInstance<IEventAggregator>();
            container = ServiceLocator.Current.GetInstance<IUnityContainer>();
        }

        byte[] getDatagramForId(int id)
        {
            id = id%256;
            var message = new byte[9];

            message[0] = (byte) id;
            return message;
        }

        private static void setValue(int parameter, byte[] message)
        {
            var value = BitConverter.GetBytes(parameter);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(value);
            value.CopyTo(message, 4);
        }

        void appendChecksum(byte[] message)
        {
            var sum = 0;
            for (var i = 0; i < message.Length-1; i++)
            {
                sum += message[i];
            }
            sum = sum%256;
            message[message.Length - 1] = (byte)sum;
        }

        public byte[] StopMotion(int id)
        {
            var message = getDatagramForId(id);
            message[1] = 3;
            appendChecksum(message);
            return message;
        }

        public byte[] RotateLeft(int id, int speed)
        {
            var message = getDatagramForId(id);
            message[1] = 2;
            setValue(speed, message);
            appendChecksum(message);
            return message;
        }

        public byte[] RotateRight(int id, int speed)
        {
            var message = getDatagramForId(id);
            message[1] = 1;
            setValue(speed, message);
            appendChecksum(message);
            return message;
        }

        public byte[] MoveToPosition(int id, int position)
        {
            var message = getDatagramForId(id);
            message[1] = 4;
            message[2] = 0;
            setValue(position, message);
            appendChecksum(message);
            return message;
        }

        public byte[] MoveToRelativePosition(int id, int position)
        {
            var message = getDatagramForId(id);
            message[1] = 4;
            message[2] = 1;
            setValue(position, message);
            appendChecksum(message);
            return message;
        }

        public byte[] SetSpeed(int id, int speed)
        {
            var message = getDatagramForId(id);
            message[1] = 5;
            message[2] = 4;
            setValue(speed, message);
            appendChecksum(message);
            return message;
        }

        public byte[] SetAcceleration(int id, int acceleration)
        {
            var message = getDatagramForId(id);
            message[1] = 5;
            message[2] = 5;
            setValue(acceleration, message);
            appendChecksum(message);
            return message;
        }

        public byte[] SetSpeedDivider(int id, int divider)
        {
            var message = getDatagramForId(id);
            message[1] = 5;
            message[2] = 154;
            setValue(divider, message);
            appendChecksum(message);
            return message;
        }

        public byte[] SetRampDivider(int id, int divider)
        {
            var message = getDatagramForId(id);
            message[1] = 5;
            message[2] = 153;
            setValue(divider, message);
            appendChecksum(message);
            return message;
        }

        public byte[] SetActualPosition(int id, int position)
        {
            var message = getDatagramForId(id);
            message[1] = 5;
            message[2] = 1;
            setValue(position, message);
            appendChecksum(message);
            return message;
        }

        public byte[] SetMicrostepResolution(int id, int resolution)
        {
            var message = getDatagramForId(id);
            message[1] = 5;
            message[2] = 140;
            setValue(resolution, message);
            appendChecksum(message);
            return message;

        }

        public byte[] SetMaxCurrent(int id, int current)
        {
            var message = getDatagramForId(id);
            message[1] = 5;
            message[2] = 1;
            setValue(current, message);
            appendChecksum(message);
            return message;
        }

        public byte[] GetSpeed(int id)
        {
            var message = getDatagramForId(id);
            message[1] = 6;
            message[2] = 4;
            appendChecksum(message);
            return message;
        }

        public byte[] GetAcceleration(int id)
        {
            var message = getDatagramForId(id);
            message[1] = 6;
            message[2] = 5;
            appendChecksum(message);
            return message;
        }

        public byte[] GetSpeedDivider(int id)
        {
            var message = getDatagramForId(id);
            message[1] = 6;
            message[2] = 154;
            appendChecksum(message);
            return message;
        }

        public byte[] GetRampDivider(int id)
        {
            var message = getDatagramForId(id);
            message[1] = 6;
            message[2] = 153;
            appendChecksum(message);
            return message;
        }

        public byte[] GetActualPosition(int id)
        {
            var message = getDatagramForId(id);
            message[1] = 6;
            message[2] = 1;
            appendChecksum(message);
            return message;
        }

        public byte[] GetMicrostepResolution(int id)
        {
            var message = getDatagramForId(id);
            message[1] = 6;
            message[2] = 140;
            appendChecksum(message);
            return message;
        }

        public byte[] GetFirmwareID(int id)
        {
            var message = getDatagramForId(id);
            message[1] = 136;
            message[2] = 0;
            appendChecksum(message);
            return message;
        }

        public int ReadValue(byte[] message)
        {
            var sub = message.ToList().GetRange(4, 4).ToArray();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(sub);
            return BitConverter.ToInt32(sub, 0);
        }

        public int ReadActualPosition(byte[] message)
        {
            var sub = message.ToList().GetRange(4, 4).ToArray();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(sub);
            return BitConverter.ToInt32(sub, 0);
        }

        public string ReadFirmwareID(byte[] message)
        {
            return Encoding.ASCII.GetString(message, 1, 8);
            //return BitConverter.ToString((char[])message, 1);
        }

        public TMCLReturnStatus GetReturnStatus(byte[] reply)
        {
            return (TMCLReturnStatus) reply[2];
        }
    }
}
