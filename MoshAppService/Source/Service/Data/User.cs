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
    [Route("/users")]
    [Route("/users/{Id}", "GET")]
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

        [UsedImplicitly]
        public string Nickname { get; set; }

        #endregion

        #region Constructors

        public User()
            : this(-1, "", "", "", "", "", "") { }

        public User(long id, string nickname, string firstName, string lastName, string email, string phone, string studentNumber) {
            Id = id;
            Nickname = nickname;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            StudentNumber = studentNumber;
        }

        public User(User user)
            : this(user.Id,
                   user.Nickname,
                   user.FirstName,
                   user.LastName,
                   user.Email,
                   user.Phone,
                   user.StudentNumber) { }

        #endregion

        #region Equality Members

        protected override bool _Equals(User other) {
            return string.Equals(Nickname, other.Nickname) &&
                   string.Equals(FirstName, other.FirstName) &&
                   string.Equals(LastName, other.LastName) &&
                   string.Equals(Email, other.Email) &&
                   string.Equals(Phone, other.Phone) &&
                   string.Equals(StudentNumber, other.StudentNumber);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Nickname != null ? Nickname.GetHashCode() : 0);
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
