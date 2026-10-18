
namespace RobotPlanet.Core
{
    public class FireBot : Robot
    {
        public FireBot(string name) : base(name, 100)
        {
        }

        // Робот тушит пожар
        public override string CrazyAction()
        {
            if (Energy < 20)
                return $"{Name} has too little energy.";

            Energy -= 20;

            return $"{Name} shoots a huge water cannon! Energy: {Energy}%";
        }
    }
}
