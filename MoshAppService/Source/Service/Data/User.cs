// Project: MoshAppService
// Filename: User.cs
// 
// Author: Jason Recillo

using System;

using JetBrains.Annotations;

using ServiceStack.Common;
using ServiceStack.ServiceHost;

namespace MoshAppService.Service.Data {
    [PublicAPI]
    [Route("/users/{Id}")]
    public class User : Entity<User> {
        #region Properties

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

        #region Equality Members

        internal override bool _Equals(User other) {
            return string.Equals(FirstName, other.FirstName) &&
                   string.Equals(LastName, other.LastName) &&
                   string.Equals(Email, other.Email) &&
                   string.Equals(Phone, other.Phone) &&
                   string.Equals(StudentNumber, other.StudentNumber);
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

        #endregion

        protected internal static User FromLoginUser(LoginUser u) {
            return u == null ? null : new User().PopulateWith(u);
        }
    }
}
