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

        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }

        public void AddLevel(Level level)
        {
            levels.Add(level);
        }

        public void AddEntity(List<Entity> entities)
        {
            this.entities.AddRange(entities);
        }
    }
}
