namespace Topebox.Tankwars
{
    public static class Constants
    {
        public enum Direction
        {
            UP,
            LEFT,
            DOWN,
            RIGHT
        }

        public enum CellType
        {
            EMPTY,
            WALL,
            RED,
            BLUE


        }

        public enum GameResult
        {
            PLAYING ,
            DRAW ,
            PLAYER1_WIN,
            PLAYER2_WIN,
        }

        public enum TankType
        {
            RED,
            BLUE
        }
    }
}