
namespace RobotPlanet.Core
{
    public class GardenBot : Robot
    {
        public GardenBot(string name) : base(name, 100)
        {
        }

        // Робот выращивает цветы
        public override string CrazyAction()
        {
            if (Energy < 25)
                return $"{Name} has too little energy.";

            Energy -= 25;

            return $"{Name} grows a giant glowing flower! Energy: {Energy}%";
        }
    }
}
