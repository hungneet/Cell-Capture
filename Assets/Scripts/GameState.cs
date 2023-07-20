using DG.Tweening;
using UnityEngine;

namespace Topebox.Tankwars
{
    public class GameState : MonoBehaviour
    {
        public GameConfig Config;
        public Constants.CellType[,] logicMap;
        public Cell[,] displayMap;
        public int[,] scoreMap;
        public int[,] scoreMapFillMode;
        public bool isFillMode = false;
        public Cell cellPrefab;

        [HideInInspector]
        public Vector2 Player1Position;

        [HideInInspector]
        public Vector2 Player2Position;

        public Tank tankPrefab;
        public Tank player1Tank;
        public Tank player2Tank;
        public Transform TankParent;

        [SerializeField, Range(1, 2)]
        public int CurrentPlayer = 1;

        [SerializeField]
        public bool is2Player = false;

        [SerializeField]
        public bool is2PlayerAI = false;

        [HideInInspector]
        public bool IsGameOver = false;

        [HideInInspector]
        public bool IsMoving = false;

        public int gameDepth = 0;

        private void Start()
        {
            logicMap = new Constants.CellType[Config.MapWidth, Config.MapHeight];
            displayMap = new Cell[Config.MapWidth, Config.MapHeight];
            scoreMap = new int[Config.MapWidth, Config.MapHeight];
            GenerateMap();
            UpdateMap();
            Player1Position = new Vector2(0, 0);
            Player2Position = new Vector2(Config.MapWidth - 1, Config.MapHeight - 1);
            player1Tank = CreateTank(Player1Position, Config.Player1Type, 1);
            OccupyPosition(Player1Position, player1Tank.CurrentTank);
            player2Tank = CreateTank(Player2Position, Config.Player2Type, 2);
            OccupyPosition(Player2Position, player2Tank.CurrentTank);
        }

        private void Update()
        {
            if (IsMoving)
            {
                Debug.Log("IsMoving:" + IsMoving + " Game Depth:" + gameDepth);
                return;
            }

            var winPlayer = CheckGameOver();
            if (winPlayer != Constants.GameResult.PLAYING)
            {
                Debug.Log($" IsGameOver {IsGameOver} Result {winPlayer}");

                return;
            }

            /*int captured = GetCapturedCellsCount(logicMap, player1Tank.CurrentTank);
            Debug.LogError($"captured {captured}");*/

            // Check for player input and update the move accordingly
            if (player1Tank.PlayerId == CurrentPlayer)
            {
                if (is2PlayerAI)
                {
                    HandleAIMove(player1Tank);
                }
                else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                {
                    HandlePlayerInput(player1Tank);
                }
            }
            else if (player2Tank.PlayerId == CurrentPlayer)
            {
                if (!is2Player)
                {
                    HandleAIMove(player2Tank);
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                        Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        HandlePlayerInput(player2Tank);
                    }
                }
            }
        }

        private void HandlePlayerInput(Tank currentTank)
        {
            var direction = GetInputDirection();
            var nextCell = GetNextCell(currentTank.CurrentCell, direction);

            if (IsValidCell(nextCell))
            {
                UpdateMoveForTank(currentTank, nextCell);
                CurrentPlayer = (CurrentPlayer == 1) ? 2 : 1;
                gameDepth++;
            }
        }

        private Constants.Direction GetInputDirection()
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                return Constants.Direction.RIGHT;
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                return Constants.Direction.LEFT;
            }
            else if (Input.GetAxis("Vertical") > 0)
            {
                return Constants.Direction.UP;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                return Constants.Direction.DOWN;
            }

            return 0;
        }

        private void HandleAIMove(Tank currentTank)
        {
            var enemeyPosition = (currentTank.PlayerId == 1) ? player2Tank.CurrentCell : player1Tank.CurrentCell;
            var direction = currentTank.GetNextMove(this, logicMap, scoreMap, enemeyPosition, isFillMode);
            var nextCell = GetNextCell(currentTank.CurrentCell, direction);

            UpdateMoveForTank(currentTank, nextCell);
            CurrentPlayer = (CurrentPlayer == 1) ? 2 : 1;
            gameDepth++;
        }

        public Constants.GameResult CheckGameOver()
        {
            var hasMoveP1 = HasValidMove(player1Tank.CurrentCell);
            var hasMoveP2 = HasValidMove(player2Tank.CurrentCell);
            IsGameOver = true;

            if (hasMoveP1 && !hasMoveP2)
                return Constants.GameResult.PLAYER1_WIN;
            if (!hasMoveP1 && hasMoveP2)
                return Constants.GameResult.PLAYER2_WIN;
            if (!hasMoveP1 && !hasMoveP2)
                return Constants.GameResult.DRAW; //draw
            IsGameOver = false;

            return Constants.GameResult.PLAYING; //not over
        }

        public bool HasValidMove(Vector2 currentCell)
        {
            var upCell = GetNextCell(currentCell, Constants.Direction.UP);
            if (IsValidCell(upCell))
            {
                return true;
            }

            var downCell = GetNextCell(currentCell, Constants.Direction.DOWN);
            if (IsValidCell(downCell))
            {
                return true;
            }

            var leftCell = GetNextCell(currentCell, Constants.Direction.LEFT);
            if (IsValidCell(leftCell))
            {
                return true;
            }

            var rightCell = GetNextCell(currentCell, Constants.Direction.RIGHT);
            if (IsValidCell(rightCell))
            {
                return true;
            }

            return false;
        }

        public void UpdateMoveForTank(Tank currentTank, Vector2 nextCell)
        {
            var direction = GetMoveDirection(currentTank.CurrentCell, nextCell);
            currentTank.SetCurrentCell(nextCell);

            var position = GetPosition(nextCell);
            IsMoving = true;
            currentTank.transform.DORotate(GetRotateByDirection(direction), 0.5f);
            currentTank.transform.DOMove(new Vector3(position.x, position.y, 0), 1f).OnComplete(() =>
            {
                IsMoving = false;
                OccupyPosition(nextCell, currentTank.CurrentTank);
            });
        }

        private Constants.Direction GetMoveDirection(Vector2 currentCell, Vector2 nextCell)
        {
            if (nextCell.x > currentCell.x)
            {
                return Constants.Direction.RIGHT;
            }
            else if (nextCell.x < currentCell.x)
            {
                return Constants.Direction.LEFT;
            }
            else if (nextCell.y > currentCell.y)
            {
                return Constants.Direction.UP;
            }
            else if (nextCell.y < currentCell.y)
            {
                return Constants.Direction.DOWN;
            }

            return 0;
        }

        private Vector3 GetRotateByDirection(Constants.Direction direction)
        {
            switch (direction)
            {
                case Constants.Direction.UP:
                    return new Vector3(0, 0, 0);

                case Constants.Direction.LEFT:
                    return new Vector3(0, 0, 90);

                case Constants.Direction.DOWN:
                    return new Vector3(0, 0, 180);

                case Constants.Direction.RIGHT:
                    return new Vector3(0, 0, 270);
            }

            return Vector3.zero;
        }

        public bool IsValidCell(Vector2 nextCell)
        {
            if (nextCell.x < 0 || nextCell.x >= Config.MapWidth)
                return false;
            if (nextCell.y < 0 || nextCell.y >= Config.MapHeight)
                return false;

            return logicMap[(int)nextCell.x, (int)nextCell.y] == Constants.CellType.EMPTY;
        }

        private void OccupyPosition(Vector2 cell, Constants.TankType tankType)
        {
            if (logicMap[(int)cell.x, (int)cell.y] == Constants.CellType.EMPTY)
            {
                scoreMap[(int)cell.x, (int)cell.y] = -1;
                switch (tankType)
                {
                    case Constants.TankType.RED:
                        logicMap[(int)cell.x, (int)cell.y] = Constants.CellType.RED;

                        break;

                    case Constants.TankType.BLUE:
                        logicMap[(int)cell.x, (int)cell.y] = Constants.CellType.BLUE;
                        break;
                }

                UpdateMap();
            }
        }

        public Vector2 GetNextCell(Vector2 currentPosition, Constants.Direction direction)
        {
            switch (direction)
            {
                case Constants.Direction.UP:
                    return new Vector2(currentPosition.x, currentPosition.y - 1);

                case Constants.Direction.LEFT:
                    return new Vector2(currentPosition.x - 1, currentPosition.y);

                case Constants.Direction.DOWN:
                    return new Vector2(currentPosition.x, currentPosition.y + 1);

                case Constants.Direction.RIGHT:
                    return new Vector2(currentPosition.x + 1, currentPosition.y);
            }

            return currentPosition;
        }

        private Tank CreateTank(Vector2 cell, Constants.TankType tankType, int playerId)
        {
            var position = GetPosition(cell);
            var tank = Instantiate<Tank>(tankPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity,
                TankParent);
            tank.SetType(tankType);
            tank.SetId(playerId);
            tank.SetCurrentCell(cell);
            return tank;
        }

        public Vector2 GetPosition(Vector2 cellPos)
        {
            return GetPosition(cellPos.x, cellPos.y);
        }

        public Vector2 GetPosition(float cellX, float cellY)
        {
            return new Vector2(cellX, -cellY);
        }

        private void UpdateMap()
        {
            for (int x = 0; x < Config.MapWidth; x++)
            {
                for (int y = 0; y < Config.MapHeight; y++)
                {
                    displayMap[x, y].SetType(logicMap[x, y]);
                }
            }
        }

        private void GenerateMap()
        {
            // Generate random map
            for (int i = 0; i < Config.WallCount / 2; i++)
            {
                int x, y;
                bool isPlayer1Position;
                bool isPlayer2Position;
                bool mayStuck;

                //GENERATE WALL
                do
                {
                    x = Random.Range(0, Config.MapWidth);
                    y = Random.Range(0, Config.MapHeight);
                    isPlayer1Position = (x == Player1Position.x && y == Player1Position.y);
                    isPlayer2Position = (x == Player2Position.x && y == Player2Position.y);
                    mayStuck = (x == 0 && y == 1) || (x == 1 && y == 0) ||
                        (x == Config.MapWidth - 1 && y == Config.MapHeight - 2) || (x == Config.MapWidth - 2 && y == Config.MapHeight - 1);
                }
                while (isPlayer1Position || isPlayer2Position || mayStuck);

                int x2 = Config.MapWidth - x - 1;
                int y2 = Config.MapHeight - y - 1;

                logicMap[x, y] = Constants.CellType.WALL;
                logicMap[x2, y2] = Constants.CellType.WALL;
                scoreMap[x, y] = -1;
                scoreMap[x2, y2] = -1;
            }
            int medium = Config.MapHeight / 2;
            for (int y = 0; y < Config.MapHeight; y++)
            {
                for (int x = 0; x < Config.MapWidth; x++)
                {
                    var pos = GetPosition(new Vector2(x, y));
                    var cell = Instantiate(cellPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity,
                        transform);
                    displayMap[x, y] = cell;

                    //Assign score to scoreMap
                    if (scoreMap[x, y] != -1)
                    {
                        if (y < medium - 1)
                            scoreMap[x, y] = y + 1;
                        else if (y <= medium)
                        {
                            if (x < medium)
                                scoreMap[x, y] = medium + x;
                            else if (x >= medium)
                                scoreMap[x, y] = scoreMap[Config.MapWidth - x - 1, y];
                        }
                        else
                            scoreMap[x, y] = Config.MapHeight - y;
                    }
                }
            }
        }

        /*public int GetCapturedCellsCount(Constants.CellType[,] logicmap, Constants.TankType CurrentTank)
        {
            int capturedCellsCount = 0;
            for (int i = 0; i < logicmap.GetLength(0); i++)
            {
                for (int j = 0; j < logicmap.GetLength(1); j++)
                {
                    if ((int)logicmap[i, j] == (int)CurrentTank+2)
                    {
                        capturedCellsCount++;
                    }
                }
            }
            return capturedCellsCount;
        }*/

        public void TurnOnFillMode()
        {
        }
    }
}