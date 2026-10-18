using System;
using System.Collections.ObjectModel;
using System.Linq;
using RobotPlanet.Core;

namespace RobotPlanet.WpfApp
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<RobotGroup> Groups { get; } = new()
        {
            new RobotGroup<CleanerBot>(
                "CleanerBot", name => new CleanerBot(name, 100)),
            new RobotGroup<ExplorerBot>(
                "ExplorerBot", name => new ExplorerBot(name, 100)),
            new RobotGroup<RepairBot>(
                "RepairBot", name => new RepairBot(name, 100)),
            new RobotGroup<GuardBot>(
                "GuardBot", name => new GuardBot(name, 100)),

            new RobotGroup<FireBot>(
                "FireBot", name => new FireBot(name)),

            new RobotGroup<MusicBot>(
                "MusicBot", name => new MusicBot(name)),

            new RobotGroup<GardenBot>(
                "GardenBot", name => new GardenBot(name))
        };

        public ObservableCollection<Robot> Robots { get; } = new();
        public ObservableCollection<string> Log { get; } = new();

        public RelayCommand AddCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public RelayCommand CrazyActionCommand { get; }
        public RelayCommand WorkCommand { get; }
        public RelayCommand ChargeCommand { get; }
        public RelayCommand RepairCommand { get; }

        private RobotGroup? _selectedGroup;
        private Robot? _selectedRobot;
        private string _newRobotName = "";
        private string _searchText = "";

        public RobotGroup? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (!SetProperty(ref _selectedGroup, value))
                    return;

                SelectedRobot = null;
                Refresh();
            }
        }

        public Robot? SelectedRobot
        {
            get => _selectedRobot;
            set
            {
                if (!SetProperty(ref _selectedRobot, value))
                    return;

                UpdateDetails();
                UpdateCommands();
            }
        }

        public string NewRobotName
        {
            get => _newRobotName;
            set
            {
                if (SetProperty(ref _newRobotName, value))
                    AddCommand.RaiseCanExecuteChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    Refresh();
            }
        }

        public string SelectedRobotInfo =>
            SelectedRobot?.ToString() ?? "No robot selected";

        public int SelectedEnergy => SelectedRobot?.Energy ?? 0;

        public int SelectedReactionCount =>
            SelectedRobot?.ReactionCount ?? 0;

        public string Summary
        {
            get
            {
                var rows = Groups
                    .SelectMany(group => group.GetAll())
                    .GroupBy(robot => robot.GetType().Name)
                    .OrderBy(group => group.Key)
                    .Select(group => $"{group.Key}: {group.Count()}");

                return string.Join(Environment.NewLine, rows);
            }
        }

        public MainViewModel()
        {
            AddCommand = new RelayCommand(
                AddRobot,
                () => SelectedGroup != null &&
                      !string.IsNullOrWhiteSpace(NewRobotName));

            RemoveCommand = new RelayCommand(
                RemoveRobot, HasSelectedRobot);

            CrazyActionCommand = new RelayCommand(
                RunCrazyAction, HasSelectedRobot);

            WorkCommand = new RelayCommand(
                Work, HasSelectedRobot);

            ChargeCommand = new RelayCommand(
                Charge,
                () => HasSelectedRobot() &&
                      SelectedRobot is IChargeable);

            RepairCommand = new RelayCommand(
                Repair,
                () => HasSelectedRobot() &&
                      SelectedRobot is IRepair);

            foreach (var group in Groups)
            {
                group.LogAdded += OnLogAdded;
                group.Changed += OnGroupChanged;
            }
            Groups.OfType<RobotGroup<FireBot>>()
                .First().Add("Fire 1");

            Groups.OfType<RobotGroup<MusicBot>>()
                .First().Add("Music 1");

            Groups.OfType<RobotGroup<GardenBot>>()
                .First().Add("Garden 1");

            SelectedGroup = Groups.FirstOrDefault();
        }

        private bool HasSelectedRobot()
        {
            return SelectedRobot != null &&
                   SelectedGroup != null &&
                   SelectedGroup.GetAll().Contains(SelectedRobot);
        }

        private void AddRobot()
        {
            SelectedGroup!.Add(NewRobotName.Trim());
            NewRobotName = "";
        }

        private void RemoveRobot()
        {
            SelectedGroup!.Remove(SelectedRobot!);
        }

        private void RunCrazyAction()
        {
            SelectedGroup!.ExecuteCrazyAction(SelectedRobot!);
        }

        private void Work()
        {
            Log.Add(SelectedRobot!.Work());
            Refresh();
        }

        private void Charge()
        {
            if (SelectedRobot is IChargeable chargeable)
            {
                Log.Add(chargeable.Charge(20));
                Refresh();
            }
        }

        private void Repair()
        {
            if (SelectedRobot is IRepair repairable)
            {
                Log.Add(repairable.Repair());
                Refresh();
            }
        }

        private void OnLogAdded(object? sender, RobotEventArgs e)
        {
            Log.Add(e.Message);
        }

        private void OnGroupChanged(object? sender, EventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            var previousSelection = SelectedRobot;
            string query = (SearchText ?? "").Trim();

            var visibleRobots = (SelectedGroup?.GetAll()
                    ?? Enumerable.Empty<Robot>())
                .Where(robot => robot.Name.Contains(
                    query, StringComparison.OrdinalIgnoreCase))
                .OrderBy(robot => robot.Name)
                .ToList();

            Robots.Clear();

            foreach (var robot in visibleRobots)
                Robots.Add(robot);

            SelectedRobot = previousSelection != null &&
                            visibleRobots.Contains(previousSelection)
                ? previousSelection
                : null;

            UpdateDetails();
            OnPropertyChanged(nameof(Summary));
            UpdateCommands();
        }

        private void UpdateDetails()
        {
            OnPropertyChanged(nameof(SelectedRobotInfo));
            OnPropertyChanged(nameof(SelectedEnergy));
            OnPropertyChanged(nameof(SelectedReactionCount));
        }

        private void UpdateCommands()
        {
            AddCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
            CrazyActionCommand.RaiseCanExecuteChanged();
            WorkCommand.RaiseCanExecuteChanged();
            ChargeCommand.RaiseCanExecuteChanged();
            RepairCommand.RaiseCanExecuteChanged();
        }
    }
}