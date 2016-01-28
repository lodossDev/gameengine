using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public static class EntityHelper
    {
        public static float GetDiff(float e1, float e2)
        {
            return Math.Abs(e1 - e2);
        }

        public static bool InRangeX(Entity e1, Entity e2, float range)
        {
            Rectangle entityBox = e1.GetBoxes(BoundingBox.BoxType.BODY)[0].GetBox();
            Rectangle targetBox = e2.GetBoxes(BoundingBox.BoxType.BODY)[0].GetBox();
            int wx = (entityBox.Width = targetBox.Width);

            if (e1.GetDirX() > 0)
            {
                return (GetDiff(e1.GetPosX(), e2.GetPosX()) <= range - 8);
            }
            else
            {
                return (GetDiff(e1.GetPosX() - (entityBox.Width / 2) + 8, e2.GetPosX()) <= range - 8);
            }
        }

        public static bool InRangeZ(Entity e1, Entity e2, float range)
        {
            return (GetDiff(e1.GetPosZ(), e2.GetPosZ()) <= range);
        }
    }
}
