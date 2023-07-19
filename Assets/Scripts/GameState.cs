using DG.Tweening;
using UnityEngine;

namespace Topebox.Tankwars
{
    public class GameState : MonoBehaviour
    {
        public GameConfig Config;
        private Constants.CellType[,] logicMap;
        private Cell[,] displayMap;
        public Cell cellPrefab;

        Vector2 Player1Position;
        Vector2 Player2Position;
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

        bool IsGameOver = false;
        bool IsMoving = false;

        private void Start()
        {
            logicMap = new Constants.CellType[Config.MapWidth, Config.MapHeight];
            displayMap = new Cell[Config.MapWidth, Config.MapHeight];
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
            if (IsMoving || IsGameOver)
            {
                Debug.Log("IsMoving:" + IsMoving + " IsGameOver:" + IsGameOver);
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
            var direction = currentTank.GetNextMove(this, logicMap, player1Tank.CurrentCell);
            var nextCell = GetNextCell(currentTank.CurrentCell, direction);

            UpdateMoveForTank(currentTank, nextCell);
            CurrentPlayer = (CurrentPlayer == 1) ? 2 : 1;
        }

        public Constants.GameResult CheckGameOver()
        {
            var hasMoveP1 = HasValidMove(player1Tank.CurrentCell);
            var hasMoveP2 = HasValidMove(player2Tank.CurrentCell);

            if (hasMoveP1 && !hasMoveP2)
                return Constants.GameResult.PLAYER1_WIN;
            if (!hasMoveP1 && hasMoveP2)
                return Constants.GameResult.PLAYER2_WIN;
            if (!hasMoveP1 && !hasMoveP2)
                return Constants.GameResult.DRAW; //draw

            return Constants.GameResult.PLAYING; //not over
        }

        private bool HasValidMove(Vector2 currentCell)
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

        private void UpdateMoveForTank(Tank currentTank, Vector2 nextCell)
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
                bool isPlayer1Position = false;
                bool isPlayer2Position = false;
                bool mayStuck = false;

                do
                {
                    x = Random.Range(0, Config.MapWidth);
                    y = Random.Range(0, Config.MapHeight);
                    isPlayer1Position = (x == Player1Position.x && y == Player1Position.y);
                    isPlayer2Position = (x == Player2Position.x && y == Player2Position.y);
                    mayStuck = (x == 0 && y == 1) || (x == 1 && y == 0) || (x==9 && y ==8) || (x==8 &&y==9);
                }
                while (isPlayer1Position || isPlayer2Position || mayStuck);

                int x2 = Config.MapWidth - x -1;
                int y2 = Config.MapHeight - y -1;

                logicMap[x, y] = Constants.CellType.WALL;
                logicMap[x2, y2] = Constants.CellType.WALL;
            }

            for (int x = 0; x < Config.MapWidth; x++)
            {
                for (int y = 0; y < Config.MapHeight; y++)
                {
                    var pos = GetPosition(new Vector2(x, y));
                    var cell = Instantiate(cellPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity,
                        transform);
                    displayMap[x, y] = cell;
                }
            }
        }

        public int GetCapturedCellsCount(Constants.CellType[,] logicmap, Constants.TankType CurrentTank)
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
        }
    }
}