using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public static class CollisionHelper
    {
        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which sides the rectangles
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

        public static float GetHorizontalIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;

            // Calculate centers.
            float centerA = rectA.Left + halfWidthA;
            float centerB = rectB.Left + halfWidthB;

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA - centerB;
            float minDistanceX = halfWidthA + halfWidthB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX)
                return 0f;

            // Calculate and return intersection depths.
            return distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
        }

        public static float GetVerticalIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfHeightA = rectA.Height / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            float centerA = rectA.Top + halfHeightA;
            float centerB = rectB.Top + halfHeightB;

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceY = centerA - centerB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceY) >= minDistanceY)
                return 0f;

            // Calculate and return intersection depths.
            return distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
        }

        public static bool TouchLeft(this Rectangle r1, Rectangle r2, int offset = 20)
        {
            return (r1.Right <= r2.Right
                        && r1.Right >= r2.Left - offset
                        && r1.Top <= r2.Bottom - (r2.Width / offset)
                        && r1.Bottom >= r2.Top + (r2.Width / offset));
        }

        public static bool TouchRight(this Rectangle r1, Rectangle r2, int offset = 20)
        {
            return (r1.Left >= r2.Left
                        && r1.Left <= r2.Right + offset
                        && r1.Top <= r2.Bottom - (r2.Width / offset)
                        && r1.Bottom >= r2.Top + (r2.Width / offset));
        }

        public static bool TouchTop(this Rectangle r1, Rectangle r2, int offset = 40)
        {
            return r1.Bottom >= r2.Top - 1
                        && r1.Bottom <= r2.Top + (r2.Height / 2)
                        && r1.Right >= r2.Left + (r2.Width / offset)
                        && r1.Left <= r2.Right - (r2.Width / offset);
        }

        public static bool TouchBottom(this Rectangle r1, Rectangle r2, int offset = 10)
        {
            return r1.Top <= r2.Bottom + 1
                        && r1.Top >= r2.Bottom - (r2.Height / 2)
                        && r1.Right >= r2.Left + (r2.Width / offset)
                        && r1.Left <= r2.Right - (r2.Width / offset);
        }

        public static float GetDiff(float e1, float e2)
        {
            return Math.Abs(e1 - e2);
        }

        public static bool InRangeX(this Entity e1, Entity e2, float range)
        {
            return (GetDiff(e1.GetPosX(), e2.GetPosX()) <= range); 
        }

        public static bool InRangeZ(this Entity e1, Entity e2, float range)
        {
            return (GetDiff(e1.GetPosZ(), e2.GetPosZ()) <= range);
        }

        public static bool InRangeZ(this Entity e1, Entity e2)
        {
            return (GetDiff(e1.GetPosZ(), e2.GetPosZ()) <= e2.GetDepth());
        }

        public static bool InBoundsZ(this Entity e1, Entity e2, float range)
        {
            return InRangeZ(e1, e2, range)
                        && !(e1.GetPosZ() <= e2.GetPosZ() - range)
                        && !(e1.GetPosZ() >= e2.GetPosZ() + range);
        }

        public static bool InBoundsZ(this Entity e1, Entity e2)
        {
            return InBoundsZ(e1, e2, e2.GetDepth());
        }

        public static bool InBoundsX(this Rectangle r1, Rectangle r2, int offset = 40)
        {
            return (r1.Right >= r2.Left + (r2.Width / offset)
                        && r1.Left <= r2.Right - (r2.Width / offset));
        }
    }
}
