namespace fantasy_hoops.Helpers
{
    public class TeamUtils
    {
        private TeamUtils() {}
		
        public static TeamUtils Instance => Nested.instance;

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly TeamUtils instance = new TeamUtils();
        }
        
        public static string GetTeamColor(int id)
        {
            switch (id)
            {
                // RAPTORS
                case 1610612761:
                    return "#CD1141";
                // CELTICS
                case 1610612738:
                    return "#008248";
                // 76ERS
                case 1610612755:
                    return "#002B5C";
                // KNICKS
                case 1610612752:
                    return "#F58426";
                // NETS
                case 1610612751:
                    return "#000000";
                // CAVALIERS
                case 1610612739:
                    return "#690031";
                // PACERS
                case 1610612754:
                    return "#002D62";
                // BUCKS
                case 1610612749:
                    return "#00471B";
                // PISTONS
                case 1610612765:
                    return "#E01E38";
                // BULLS
                case 1610612741:
                    return "#CE1141";
                // WIZARDS
                case 1610612764:
                    return "#E31837";
                // HEAT
                case 1610612748:
                    return "#98002E";
                // HORNETS
                case 1610612766:
                    return "#1D1160";
                // MAGIC
                case 1610612753:
                    return "#0B77BD";
                // HAWKS
                case 1610612737:
                    return "#E03A3E";
                // TRAIL BLAZERS
                case 1610612757:
                    return "#E13A3E";
                // TIMBERWOLVES
                case 1610612750:
                    return "#002B5C";
                // THUNDER
                case 1610612760:
                    return "#EF3B24";
                // NUGGETS
                case 1610612743:
                    return "#00285E";
                // JAZZ
                case 1610612762:
                    return "#0C2340";
                // WARRIORS
                case 1610612744:
                    return "#243E90";
                // CLIPPERS
                case 1610612746:
                    return "#ED174C";
                // LAKERS
                case 1610612747:
                    return "#552583";
                // KINGS
                case 1610612758:
                    return "#5A2D81";
                // SUNS
                case 1610612756:
                    return "#E56020";
                // ROCKETS
                case 1610612745:
                    return "#C21F32";
                // PELICANS
                case 1610612740:
                    return "#85714D";
                // SPURS
                case 1610612759:
                    return "#8A8D8F";
                // MAVERICKS
                case 1610612742:
                    return "#007DC5";
                // GRIZZLIES
                case 1610612763:
                    return "#12173F";
                default:
                    return "#C4CED4";
            }
        }
    }
}