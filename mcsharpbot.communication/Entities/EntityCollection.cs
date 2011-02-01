using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication.Entities
{
    public class EntityCollection
    {
        public HashSet<EntityType> Entities = new HashSet<EntityType>();

        public void Add(EntityType entity)
        {
            if (Entities.Count(x => x.EntityID == entity.EntityID) == 0)
            {
                Entities.Add(entity);
            }
        }

        public EntityType GetFromId(int eid)
        {
            var selected = Entities.Where(x => x.EntityID == eid);
            if (selected.Count() == 0)
            {
                return null;
            }
            else
            {
                return selected.First();
            }
        }

        public void Remove(int eid)
        {
            Entities.RemoveWhere(x => x.EntityID == eid);
        }

        public void Replace(EntityType toreplace, EntityType entity)
        {
            Entities.Remove(toreplace);
            Entities.Add(entity);
        }
    }
}
