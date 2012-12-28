// Project: MoshAppService
// Filename: Entity.cs
// 
// Author: Jason Recillo

using System;

using ServiceStack.Text;

namespace MoshAppService.Service.Data {
    public abstract class Entity {
        #region Properties

        public long Id { get; set; }

        #endregion

        #region Constructors

        public Entity()
            : this(-1) { }

        public Entity(long id) {
            Id = id;
        }

        public Entity(Entity other)
            : this(other.Id) { }

        #endregion

        #region Equality Members

        internal abstract bool Equals(Entity other);

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Entity) obj);
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        #endregion

        public override string ToString() {
            return JsonSerializer.SerializeToString(this);
        }
    }

    public abstract class Entity<T> : Entity where T : Entity {
        #region Constructors

        public Entity()
            : this(-1) { }

        public Entity(long id)
            : base(id) { }

        public Entity(T other)
            : this((Entity) other) { }

        public Entity(Entity other)
            : this(other.Id) { }

        #endregion

        #region Equality Members

        internal override bool Equals(Entity other) {
            return Id == other.Id && _Equals((T) other);
        }

        internal abstract bool _Equals(T other);

        #endregion
    }
}
