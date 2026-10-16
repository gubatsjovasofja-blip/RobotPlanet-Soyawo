using System;

namespace RobotPlanet.Core
{
    public class RobotEventArgs : EventArgs
    {
        public Robot Actor { get; }
        public string Message { get; }

        public RobotEventArgs(Robot actor, string message)
        {
            Actor = actor;
            Message = message;
        }
    }
}