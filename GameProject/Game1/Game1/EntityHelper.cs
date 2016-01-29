﻿using Microsoft.Xna.Framework;
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

    static class RectangleExtensions
    {
        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which wides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        /// <summary>
        /// Gets the position of the center of the bottom edge of the rectangle.
        /// </summary>
        public static Vector2 GetBottomCenter(this Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
        }
    }
}
