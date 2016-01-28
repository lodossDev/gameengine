using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class ComboChain
    {
        private List<ComboMove> moves;
        private int currentMoveIndex;
        public float currentMoveTime;

        public ComboChain(List<ComboMove> moves)
        {
            this.moves = moves;
            currentMoveIndex = 0;
            currentMoveTime = 0f;
        }

        public List<ComboMove> GetMoves()
        {
            return moves;
        }

        public int GetCurrentMoveIndex()
        {
            return currentMoveIndex;
        }

        public bool InLastAttackState()
        {
            return currentMoveIndex == moves.Count - 1;
        }

        public void IncrementMoveIndex()
        {
            currentMoveIndex++;
            currentMoveTime = 0f;
        }

        public void ResetMove()
        {
            currentMoveIndex = 0;
            currentMoveTime = 0f;
        }

        public ComboMove GetCurrentMove()
        {
            return moves[currentMoveIndex];
        }

        public void UpdateCombo(GameTime gameTime)
        {
            if (moves.Count > 0 && currentMoveIndex > 0)
            {
                currentMoveTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (currentMoveIndex > moves.Count - 1)
                {
                    ResetMove();
                }

                if (currentMoveTime > moves[currentMoveIndex].GetExpireTime())
                {
                    ResetMove();
                }
            }
        }
    }

    public class ComboMove
    {
        private Animation.State state;
        private float expireTime;
        private int cancelFrame;

        public ComboMove(Animation.State state, float expireTime, int cancelFrame)
        {
            this.state = state;
            this.expireTime = expireTime;
            this.cancelFrame = cancelFrame - 1;
        }

        public Animation.State GetState()
        {
            return state;
        }
         
        public float GetExpireTime()
        {
            return expireTime;
        }
        
        public int GetCancelFrame()
        {
            return cancelFrame;
        }   
    }
}
