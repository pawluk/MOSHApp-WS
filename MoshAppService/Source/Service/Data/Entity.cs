// Project: MoshAppService
// Filename: Entity.cs
// 
// Author: Jason Recillo

using System;

using ServiceStack.Text;

namespace MoshAppService.Service.Data {
    public abstract class Entity {
        public long Id { get; set; }

        public override string ToString() {
            return JsonSerializer.SerializeToString(this);
        }

        protected bool Equals(Entity other) {
            return Id == other.Id;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Entity) obj);
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}
