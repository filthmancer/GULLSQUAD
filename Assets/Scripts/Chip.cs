using UnityEngine;
using UnityEngine.UI;

namespace Gull
{
    public class Chip : Gull.Object
    {
        public Sprite[] chippies;
        public Image image;

        void Start()
        {
            image.sprite = chippies[Random.Range(0, chippies.Length)];
        }
        public override void InteractWith<T>(T obj)
        {
            if (obj is Gull.Seagull) LevelManager.instance.EatChip(this, obj as Gull.Unit);
        }
    }
}