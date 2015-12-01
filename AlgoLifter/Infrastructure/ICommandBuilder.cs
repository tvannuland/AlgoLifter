﻿using System;
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
        byte[] SetSpeedDivider(int id, int divider);
        byte[] SetRampDivider(int id, int divider);
        byte[] SetActualPosition(int id, int position);
        byte[] SetMicrostepResolution(int id, int resolution);
        byte[] SetMaxCurrent(int id, int current);
        byte[] GetActualPosition(int id);
        byte[] GetFirmwareID(int id);

        int ReadActualPosition(byte[] message);
        string ReadFirmwareID(byte[] message);
        TMCLReturnStatus GetReturnStatus(byte[] reply);
    }
}
