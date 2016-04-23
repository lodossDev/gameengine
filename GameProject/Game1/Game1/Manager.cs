using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Manager
    {
        protected List<Entity> entities;
        protected List<Level> levels;


        public Manager()
        {
            entities = new List<Entity>();
            levels = new List<Level>();
        }

        public virtual void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }

        public virtual void AddLevel(Level level)
        {
            levels.Add(level);
        }

        public virtual void AddEntity(List<Entity> entities)
        {
            this.entities.AddRange(entities);
        }
    }
}
