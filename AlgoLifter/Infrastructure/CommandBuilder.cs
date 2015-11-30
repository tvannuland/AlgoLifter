using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLifter.Infrastructure
{
    public interface CommandBuilder
    {
        byte[] StopMotion();
        byte[] RotateLeft(int speed);
        byte[] RotateRight(int speed);
        byte[] MoveToPosition(int Position);
        byte[] GetFirmwareID(int id);
        byte[] SetMaxCurrent(int id, int current);
    }
}
