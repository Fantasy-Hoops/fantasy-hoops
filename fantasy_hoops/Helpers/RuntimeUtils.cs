using System;

namespace fantasy_hoops.Helpers
{
    public static class RuntimeUtils
    {
        public static DateTime NEXT_GAME = DateTime.UtcNow;
        public static DateTime NEXT_GAME_CLIENT = DateTime.UtcNow;
        public static DateTime NEXT_LAST_GAME = DateTime.UtcNow;
        public static DateTime PREVIOUS_GAME = DateTime.UtcNow;
        public static DateTime PREVIOUS_LAST_GAME = DateTime.UtcNow;
        public static DateTime PLAYER_POOL_DATE = DateTime.UtcNow;
    }
}