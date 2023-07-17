using System.Collections;
using System.Collections.Generic;
using Topebox.Tankwars;
using UnityEngine;

namespace Topebox.Tankwars
{
    public class Cell : MonoBehaviour
    {
        public Constants.CellType CurrentType = Constants.CellType.EMPTY;
        public SpriteRenderer SpriteRenderer;
        public Sprite EmptySprite;
        public Sprite WallSprite;
        public Sprite RedSprite;
        public Sprite BlueSprite;

        public void SetType(Constants.CellType cellType)
        {
            CurrentType = cellType;
            switch (CurrentType)
            {
                case Constants.CellType.EMPTY:
                    SpriteRenderer.sprite = EmptySprite;
                    SpriteRenderer.color = Color.white;
                    break;
                case Constants.CellType.WALL:
                    SpriteRenderer.sprite = WallSprite;
                    SpriteRenderer.color = Color.gray;
                    break;
                case Constants.CellType.RED:
                    SpriteRenderer.sprite = RedSprite;
                    SpriteRenderer.color = Color.red;
                    break;
                case Constants.CellType.BLUE:
                    SpriteRenderer.sprite = BlueSprite;
                    SpriteRenderer.color = Color.blue;
                    break;
            }
        }
    }
}