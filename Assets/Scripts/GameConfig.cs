using System;
using UnityEngine;

namespace Topebox.Tankwars
{
    [Serializable]
    public class GameConfig
    {
        public int MapWidth = 10;
        public int MapHeight = 10;
        public int WallCount = 10;
        public Constants.TankType Player1Type = Constants.TankType.RED;
        public Constants.TankType Player2Type = Constants.TankType.BLUE;
    }
}