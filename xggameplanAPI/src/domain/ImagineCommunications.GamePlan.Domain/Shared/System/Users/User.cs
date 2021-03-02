using System;
using System.Collections.Generic;
using RegularExpressions = System.Text.RegularExpressions;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Users
{
    using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;

    public class User
    {
        public User()
        {
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public string Surname { get; private set; }

        public string Email { get; private set; }

        public UserPassword Password { get; private set; }

        public PreviewFile Preview { get; set; }

        public string ThemeName { get; private set; }

        public string Location { get; private set; }

        public string Role { get; private set; }

        public int TenantId { get; private set; }

        public string DefaultTimeZone { get; private set; }

        public Dictionary<string, string> UserSettings = new Dictionary<string, string>();

        public void ChangeName(
            string name)
        {
            Name = name;
        }

        public void ChangePassword(
                    string plainTextPassword,
                    List<string> bannedPasswordLiteralList,
                    List<string> bannedPasswordPatternList)
        {
            // Validate format
            UserPassword.ValidateFormat(plainTextPassword);

            // Validate banned list
            UserPassword.ValidateOnBannedList(plainTextPassword, bannedPasswordLiteralList, bannedPasswordPatternList);

            // Set password
            Password = UserPassword.Hash(plainTextPassword);
        }

        public bool IsPasswordValid(
            string plainTextPassword) => Password.Matches(plainTextPassword);

        public static User Create(
            int id,
            string name,
            string surname,
            string email,
            string plainTextPassword,
            string themeName,
            string location,
            string role,
            int tenantId,
            string defaultTimeZone
            )
        {
            Validate(name,
                     surname,
                     email,
                     plainTextPassword,
                     tenantId);

            var user = new User()
            {
                Id = id,
                Name = name,
                Surname = surname,
                Email = email,
                Password = plainTextPassword == null ? null : UserPassword.Hash(plainTextPassword),
                ThemeName = themeName,
                Location = location,
                Role = role,
                TenantId = tenantId,
                DefaultTimeZone = defaultTimeZone,
            };

            return user;
        }

        public void Change(
            string name,
            string surname,
            string email,
            string themeName,
            string location,
            string role,
            int tenantId,
            string defaultTimeZone)
        {
            Validate(name,
                     surname,
                     email,
                     tenantId);

            Name = name;
            Surname = surname;
            Email = email;
            ThemeName = themeName;
            Location = location;
            Role = role;
            TenantId = tenantId;
            DefaultTimeZone = defaultTimeZone;
        }

        private static void Validate(
            string name,
            string surname,
            string email,
            int tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(surname))
            {
                throw new ArgumentNullException(nameof(surname));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (tenantId < 0)
            {
                throw new ArgumentException("Tenant Id cannot be zero or negative", nameof(tenantId));
            }
        }

        private static void Validate(
           string name,
           string surname,
           string email,
           string plainTextPassword,
           int tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(surname))
            {
                throw new ArgumentNullException(nameof(surname));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (plainTextPassword != null && string.IsNullOrWhiteSpace(plainTextPassword))  // Allow null
            {
                throw new ArgumentNullException(nameof(Password));
            }
            else if (plainTextPassword != null)
            {
                UserPassword.ValidateFormat(plainTextPassword);
            }

            if (tenantId < 0)
            {
                throw new ArgumentException("Tenant Id cannot be zero or negative", nameof(tenantId));
            }
        }

        public class UserPassword
        {
            private UserPassword()
            {
            }

            public static UserPassword CreateFromHash(string hashedPassword)
            {
                return new UserPassword()
                {
                    HashedValue = hashedPassword,
                };
            }

            public string HashedValue { get; private set; }

            public bool Matches(
                string plainTextPassword)
            {
                return
                    BCrypt.Net.BCrypt.Verify(
                        plainTextPassword,
                        HashedValue);
            }

            /// <summary>
            /// Validates whether password meets format rules.
            /// </summary>
            /// <param name="plainTextPassword">Password to validate</param>
            public static void ValidateFormat(string plainTextPassword)
            {
                byte status = 0;            // Default=Success
                short minLength = 5;
                short maxLength = 100;       // Handle long passwords (Robust security, GUIDS for internal testing)
                short minDistinct = 3;

                if (status == 0 && string.IsNullOrWhiteSpace(plainTextPassword))
                {
                    status = 1;
                }

                // Checl length
                if (status == 0 && (plainTextPassword.Length < minLength || plainTextPassword.Length > maxLength))
                {
                    status = 2;
                }

                // Check password characters
                if (status == 0)
                {
                    short countLetter = 0;
                    short countDigit = 0;
                    var distinctCharacters = new List<char>();
                    foreach (char currentChar in plainTextPassword)
                    {
                        if (Char.IsDigit(currentChar))
                        {
                            countDigit++;
                        }
                        else if (Char.IsLetter(currentChar))
                        {
                            countLetter++;
                        }
                        if (!distinctCharacters.Contains(currentChar))
                        {
                            distinctCharacters.Add(currentChar);
                        }
                    }
                    if (countDigit == 0 || countLetter == 0)
                    {
                        status = 3;
                    }
                    else if (distinctCharacters.Count < minDistinct)
                    {
                        status = 4;
                    }
                }

                if (status != 0 && !plainTextPassword.Equals("123123"))      // TODO: Remove this exception for allowing internal password
                {
                    string message = string.Format("Password must be between {0} and {1} characters long, it must contain at least {2} different characters and " +
                                          "it must contain letters and digits", minLength, maxLength, minDistinct);
                    throw new ArgumentException(message);
                }
            }

            /// <summary>
            /// Validates whether password is on banned list, necessary where we want to stop commonly uses passwords and ones
            /// that can easily be guessed (E.g. Surname, email)
            /// </summary>
            /// <param name="plainTextPassword">Password to check</param>
            /// <param name="bannedPasswordLiteralList">List of banned passwords</param>
            /// <param name="bannedPasswordPatternList">List of banned passwords patterns (RegEx)</param>
            public static void ValidateOnBannedList(string plainTextPassword,
                                             List<string> bannedPasswordLiteralList,
                                             List<string> bannedPasswordPatternList)
            {
                byte status = 0;    // Default=Valid

                // Check if on banned literal list
                if (status == 0 && bannedPasswordLiteralList != null && bannedPasswordLiteralList.Contains(plainTextPassword))
                {
                    status = 1;
                }

                // Check if on banned pattern list
                if (status == 0 && bannedPasswordPatternList != null)
                {
                    foreach (string bannedPasswordPattern in bannedPasswordPatternList)
                    {
                        if (!RegularExpressions.Regex.IsMatch(plainTextPassword, bannedPasswordPattern))
                        {
                            status = 1;
                            break;
                        }
                    }
                }

                if (status != 0)
                {
                    throw new ArgumentException("Password is on the banned list");
                }
            }

            public static UserPassword Hash(
                string plainTextPassword)
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(
                    plainTextPassword,
                    workFactor: 10);

                return new UserPassword()
                {
                    HashedValue = hashedPassword,
                };
            }
        }
    }
}
