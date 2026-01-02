using System;

namespace LibraryK2U2.models
{
    public class User
    {
        // Identity
        public string Username { get; set; }
        public string DisplayName { get; set; }

        // Security
        public string PIN { get; set; }
        public bool IsAdminUser { get; private set; }

        public int FailedAttempts { get; set; } = 0;
        public int MaxAttempts { get; set; } = 3;

        // Account lock state
        public bool IsBlocked => FailedAttempts >= MaxAttempts;

        // Required for JSON
        public User() { }

        public User(string username, string displayName, string pin, bool isAdmin = false)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name cannot be empty");

            if (string.IsNullOrWhiteSpace(pin) || pin.Length != 4 || !int.TryParse(pin, out _))
                throw new ArgumentException("PIN must be exactly 4 digits");

            Username = username.Trim();
            DisplayName = displayName.Trim();
            PIN = pin.Trim();
            IsAdminUser = isAdmin;
        }

        // Validates PIN and block state
        public bool ValidatePin(string input)
            => !IsBlocked && input == PIN;

        public void ResetAttempts()
            => FailedAttempts = 0;

        public void GrantAdmin()
            => IsAdminUser = true;

        public void RevokeAdmin()
            => IsAdminUser = false;

        public bool IsAdmin()
            => IsAdminUser;
    }
}