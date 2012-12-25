// Project: MoshAppService
// Filename: User.cs
// 
// Author: Jason Recillo

using JetBrains.Annotations;

using MoshAppService.Service.Response;

using ServiceStack.Common;
using ServiceStack.ServiceHost;
using ServiceStack.Text;

namespace MoshAppService.Service.Data {
    [PublicAPI]
    [Route("/users/{Id}")]
    public class User {
        #region Properties

        [UsedImplicitly]
        public long Id { get; set; }

        [UsedImplicitly]
        public string FirstName { get; set; }

        [UsedImplicitly]
        public string LastName { get; set; }

        [UsedImplicitly]
        public string Email { get; set; }

        [UsedImplicitly]
        public string Phone { get; set; }

        [UsedImplicitly]
        public string StudentNumber { get; set; }

        #endregion

        #region Constructors

        public User()
            : this(-1, "", "", "", "", "") { }

        public User(long id, string firstName, string lastName, string email, string phone, string studentNumber) {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            StudentNumber = studentNumber;
        }

        public User(User user)
            : this(user.Id,
                   user.FirstName,
                   user.LastName,
                   user.Email,
                   user.Phone,
                   user.StudentNumber) { }

        #endregion

        public override string ToString() {
            return JsonSerializer.SerializeToString(this);
        }

        protected internal static User FromLoginUser(LoginUser u) {
            return u == null ? null : new User().PopulateWith(u);
        }

        protected bool Equals(User other) {
            return Id == other.Id && string.Equals(FirstName, other.FirstName) && string.Equals(LastName, other.LastName) && string.Equals(Email, other.Email) && string.Equals(Phone, other.Phone) && string.Equals(StudentNumber, other.StudentNumber);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((User) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Phone != null ? Phone.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (StudentNumber != null ? StudentNumber.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public class UserResponse : ResponseBase {
        public User User { get; set; }
    }
}
