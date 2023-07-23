using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Topebox.Tankwars
{
    [Serializable]
    public class Tank : MonoBehaviour
    {
        public Constants.TankType CurrentTank = Constants.TankType.RED;
        public Constants.TankType EnemyTank = Constants.TankType.BLUE;
        public SpriteRenderer SpriteRenderer;
        public Sprite RedSprite;
        public Sprite BlueSprite;
        public int PlayerId;
        public bool fillMode = false;
        public Vector2 CurrentCell;

        public void SetType(Constants.TankType tankType)
        {
            CurrentTank = tankType;
            switch (CurrentTank)
            {
                case Constants.TankType.RED:
                    SpriteRenderer.sprite = RedSprite;
                    SpriteRenderer.color = Color.red;
                    EnemyTank = Constants.TankType.BLUE;
                    break;

                case Constants.TankType.BLUE:
                    SpriteRenderer.sprite = BlueSprite;
                    SpriteRenderer.color = Color.blue;
                    EnemyTank = Constants.TankType.RED;
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

        public Constants.Direction GetNextMove(GameState game, Constants.CellType[,] logicMap, int[,] scoreMap, Vector2 otherPosition)
        {
            var myPosition = CurrentCell;
            //var enemyPosition = otherPosition;
            //bool isMaximizing = game.CurrentPlayer == 1;
            var availableMove = GetAvailableMoves(game, myPosition);

            
            if (availableMove.Count == 0) // if there are no available moves, return DOWN
                return Constants.Direction.DOWN;

            if (availableMove.Count == 1) // if there is only one available move, return it
                return availableMove[0];
            int maxScore = 0;
            var bestMove = availableMove[0];
            var canMove = new List<Constants.Direction>();
            
            foreach (var move in availableMove)
            {   
                /*if( !fillMode)
                {*/
                    var pos = game.GetNextCell(myPosition, move);
                    game.CheckDeadEnd(pos);
                    if (scoreMap[(int)pos.x, (int)pos.y] > maxScore)
                    {
                        maxScore = scoreMap[(int)pos.x, (int)pos.y];
                        bestMove = move;
                    }
                    else if (scoreMap[(int)pos.x, (int)pos.y] == maxScore)
                    {
                        canMove.Add(move);
                    }
                    
                //}
                
            }

            if (canMove.Count > 0)
            {
                return canMove[Random.Range(0, canMove.Count)];
            }
            else
            return bestMove;
            
        }

        private List<Constants.Direction> GetAvailableMoves(GameState game, Vector2 position)
        {
            var availableMove = new List<Constants.Direction>();

            var upCell = game.GetNextCell(position, Constants.Direction.UP);
            if (game.IsValidCell(upCell))
            {
                availableMove.Add(Constants.Direction.UP);
            }

            var downCell = game.GetNextCell(position, Constants.Direction.DOWN);
            if (game.IsValidCell(downCell))
            {
                availableMove.Add(Constants.Direction.DOWN);
            }

            var leftCell = game.GetNextCell(position, Constants.Direction.LEFT);
            if (game.IsValidCell(leftCell))
            {
                availableMove.Add(Constants.Direction.LEFT);
            }

            var rightCell = game.GetNextCell(position, Constants.Direction.RIGHT);
            if (game.IsValidCell(rightCell))
            {
                availableMove.Add(Constants.Direction.RIGHT);
            }

            return availableMove;
        }


    }
}