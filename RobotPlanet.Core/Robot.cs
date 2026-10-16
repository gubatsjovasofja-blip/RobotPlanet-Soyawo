using System;
using System.Collections.Generic;

namespace RobotPlanet.Core
{
    public abstract class Robot
    {
        public string Name { get; }
        public int Energy { get; protected set; }
        public int ReactionCount { get; private set; }

        private readonly List<string> _history = new();

        public IReadOnlyList<string> History => _history.AsReadOnly();

        public event EventHandler<RobotEventArgs>? LogAdded;

        protected Robot(string name, int energy)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Robot name cannot be empty.");

            if (energy < 0 || energy > 100)
                throw new ArgumentException("Energy must be between 0 and 100.");

            Name = name;
            Energy = energy;
        }

        public virtual string Work()
        {
            if (Energy < 10)
                return $"{Name} has too little energy.";

            Energy -= 10;

            return $"{Name} performs normal robot work. Energy: {Energy}%";
        }

        public abstract string CrazyAction();

        private void RecordReaction(string message)
        {
            _history.Add(message);
            LogAdded?.Invoke(this, new RobotEventArgs(this, message));
        }

        public void OnRobotAdded(object? sender, RobotEventArgs e)
        {
            if (ReferenceEquals(this, e.Actor))
                return;

            RecordReaction($"{Name} welcomes {e.Actor.Name}!");
        }

        public void OnRobotRemoved(object? sender, RobotEventArgs e)
        {
            if (ReferenceEquals(this, e.Actor))
                return;

            RecordReaction($"{Name} says goodbye to {e.Actor.Name}.");
        }

        public virtual void OnCrazyAction(object? sender, RobotEventArgs e)
        {
            if (ReferenceEquals(this, e.Actor))
                return;

            ReactionCount++;

            RecordReaction(
                $"{Name} detects a strange signal from {e.Actor.Name} " +
                $"and runs a diagnostic. Reactions: {ReactionCount}.");
        }

        public override string ToString()
        {
            return $"{GetType().Name}: {Name}";
        }
    }
}