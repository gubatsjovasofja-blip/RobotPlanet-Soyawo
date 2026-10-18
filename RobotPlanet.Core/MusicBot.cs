
namespace RobotPlanet.Core
{
    public class MusicBot : Robot
    {
        public MusicBot(string name) : base(name, 100)
        {
        }

        // Робот устраивает концерт
        public override string CrazyAction()
        {
            if (Energy < 15)
                return $"{Name} has too little energy.";

            Energy -= 15;

            return $"{Name} starts a crazy robot concert! Energy: {Energy}%";
        }
    }
}
