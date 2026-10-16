using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotPlanet.Core
{
    public class Repository<T> : IRepository<T> where T : Robot
    {
        private readonly List<T> _items = new();

        public event EventHandler<RobotEventArgs>? RobotAdded;
        public event EventHandler<RobotEventArgs>? RobotRemoved;
        public event EventHandler<RobotEventArgs>? CrazyActionPerformed;

        public event EventHandler<RobotEventArgs>? LogAdded;
        public event EventHandler? Changed;

        public void Add(T item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (_items.Contains(item))
                return;

            _items.Add(item);

            RobotAdded += item.OnRobotAdded;
            RobotRemoved += item.OnRobotRemoved;
            CrazyActionPerformed += item.OnCrazyAction;
            item.LogAdded += OnRobotLogAdded;

            var args = new RobotEventArgs(
                item, $"{item.Name} joined Robot Planet.");

            LogAdded?.Invoke(this, args);
            RobotAdded?.Invoke(this, args);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool Remove(T item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (!_items.Remove(item))
                return false;

            RobotAdded -= item.OnRobotAdded;
            RobotRemoved -= item.OnRobotRemoved;
            CrazyActionPerformed -= item.OnCrazyAction;
            item.LogAdded -= OnRobotLogAdded;

            var args = new RobotEventArgs(
                item, $"{item.Name} left Robot Planet.");

            LogAdded?.Invoke(this, args);
            RobotRemoved?.Invoke(this, args);
            Changed?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public void ExecuteCrazyAction(T item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (!_items.Contains(item))
                throw new InvalidOperationException(
                    "Robot does not belong to this repository.");

            string message = item.CrazyAction();
            var args = new RobotEventArgs(item, message);

            LogAdded?.Invoke(this, args);
            CrazyActionPerformed?.Invoke(this, args);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerable<T> GetAll()
        {
            return _items.AsReadOnly();
        }

        public T? Find(Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return _items.FirstOrDefault(predicate);
        }

        private void OnRobotLogAdded(object? sender, RobotEventArgs e)
        {
            LogAdded?.Invoke(this, e);
        }
    }
}