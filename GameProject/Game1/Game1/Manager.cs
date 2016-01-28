using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Manager
    {
        protected List<Entity> entities;

        public Manager()
        {
            entities = new List<Entity>();
        }

        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }

        public void AddEntity(List<Entity> entities)
        {
            this.entities.AddRange(entities);
        }
    }
}
