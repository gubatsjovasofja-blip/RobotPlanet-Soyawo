using System;
using System.Collections.Generic;
using RobotPlanet.Core;

namespace RobotPlanet.WpfApp
{
    public abstract class RobotGroup
    {
        public string Name { get; }

        public event EventHandler<RobotEventArgs>? LogAdded;
        public event EventHandler? Changed;

        protected RobotGroup(string name)
        {
            Name = name;
        }

        protected void ForwardLog(object? sender, RobotEventArgs e)
        {
            LogAdded?.Invoke(this, e);
        }

        protected void NotifyChanged(object? sender, EventArgs e)
        {
            Changed?.Invoke(this, e);
        }

        public abstract IEnumerable<Robot> GetAll();
        public abstract void Add(string name);
        public abstract void Remove(Robot robot);
        public abstract void ExecuteCrazyAction(Robot robot);
    }

    public sealed class RobotGroup<T> : RobotGroup where T : Robot
    {
        private readonly Repository<T> _repository = new();
        private readonly Func<string, T> _factory;

        public RobotGroup(string name, Func<string, T> factory)
            : base(name)
        {
            _factory = factory;

            _repository.LogAdded += ForwardLog;
            _repository.Changed += NotifyChanged;
        }

        public override IEnumerable<Robot> GetAll()
        {
            return _repository.GetAll();
        }

        public override void Add(string name)
        {
            T robot = _factory(name);
            _repository.Add(robot);
        }

        public override void Remove(Robot robot)
        {
            if (robot is T typedRobot)
                _repository.Remove(typedRobot);
        }

        public override void ExecuteCrazyAction(Robot robot)
        {
            if (robot is T typedRobot)
                _repository.ExecuteCrazyAction(typedRobot);
        }
    }
}