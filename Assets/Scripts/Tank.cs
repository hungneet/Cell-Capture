using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Topebox.Tankwars
{
    [Serializable]
    public class Tank : MonoBehaviour
    {
        public Constants.TankType CurrentTank = Constants.TankType.RED;
        public SpriteRenderer SpriteRenderer;
        public Sprite RedSprite;
        public Sprite BlueSprite;
        public int PlayerId;
        public Vector2 CurrentCell;

        public void SetType(Constants.TankType tankType)
        {
            CurrentTank = tankType;
            switch (CurrentTank)
            {
                case Constants.TankType.RED:
                    SpriteRenderer.sprite = RedSprite;
                    SpriteRenderer.color = Color.red;
                    break;
                case Constants.TankType.BLUE:
                    SpriteRenderer.sprite = BlueSprite;
                    SpriteRenderer.color = Color.blue;
                    break;
            }
        }

        public void SetId(int playerId)
        {
            PlayerId = playerId;
        }

        public void SetCurrentCell(Vector2 pos)
        {
            CurrentCell = pos;
        }

        public Constants.Direction GetNextMove(GameState game, Constants.CellType[,] logicMap, Vector2 otherPosition)
        {
            var myPosition = CurrentCell;
            var enemyPosition = otherPosition;

            var availableMove = new List<Constants.Direction>();
            var upCell = game.GetNextCell(myPosition, Constants.Direction.UP);
            if (game.IsValidCell(upCell))
            {
                availableMove.Add(Constants.Direction.UP);
            }

            var downCell = game.GetNextCell(myPosition, Constants.Direction.DOWN);
            if (game.IsValidCell(downCell))
            {
                availableMove.Add(Constants.Direction.DOWN);
            }

            var leftCell = game.GetNextCell(myPosition, Constants.Direction.LEFT);
            if (game.IsValidCell(leftCell))
            {
                availableMove.Add(Constants.Direction.LEFT);
            }

            var rightCell = game.GetNextCell(myPosition, Constants.Direction.RIGHT);
            if (game.IsValidCell(rightCell))
            {
                availableMove.Add(Constants.Direction.RIGHT);
            }

            if (availableMove.Count == 0) //if have no available move, return DOWN
                return Constants.Direction.DOWN;
            
            if (availableMove.Count == 1) //if have 1 available move, return it
                return availableMove[0];

            //TODO: Your logic here
            return availableMove[Random.Range(0, availableMove.Count)]; //temp return random move
        }
    }
}