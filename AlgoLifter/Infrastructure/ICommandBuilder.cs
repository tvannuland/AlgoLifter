using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLifter.Infrastructure
{
    public interface ICommandBuilder
    {
        byte[] StopMotion(int id);
        byte[] RotateLeft(int id, int speed);
        byte[] RotateRight(int id, int speed);
        byte[] MoveToPosition(int id, int position);
        byte[] MoveToRelativePosition(int id, int position);

        byte[] SetSpeed(int id, int speed);
        byte[] SetAcceleration(int id, int acceleration);
        byte[] SetBaudRate(int id, int baudrate);
        byte[] SetSpeedDivider(int id, int divider);
        byte[] SetRampDivider(int id, int divider);
        byte[] SetActualPosition(int id, int position);
        byte[] SetMicrostepResolution(int id, int resolution);
        byte[] SetMaxCurrent(int id, int current);

        byte[] GetSpeed(int id);
        byte[] GetAcceleration(int id);
        byte[] GetBaudRate(int id);
        byte[] GetSpeedDivider(int id);
        byte[] GetRampDivider(int id);
        byte[] GetActualPosition(int id);
        byte[] GetMicrostepResolution(int id);
        byte[] GetFirmwareID(int id);

        int ReadValue(byte[] message);
        string ReadFirmwareID(byte[] message);
        string GetReturnStatus(byte[] reply);
    }
}
