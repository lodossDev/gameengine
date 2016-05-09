using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Effect
    {
        public enum EffectType { HIT_SPARK }
        public enum EffectState { NONE, LIGHT, MEDIUM, HEAVY }

        private Vector2 offset;
        private EffectType effectType;
        private EffectState effectState;
        

        public Effect(EffectType effectType, EffectState effectState, float x1, float y1)
        {
            this.effectType = effectType;
            this.effectState = effectState;
            offset = new Vector2(x1, y1);
        }

        public Vector2 GetOffset()
        {
            return offset;
        }

        public EffectState GetEffectState()
        {
            return effectState;
        }

        public EffectType GetEffectType()
        {
            return effectType;
        }
    }
}
