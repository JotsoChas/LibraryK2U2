using System;

namespace LibraryK2U2.auth
{
    public class User
    {
        // --- Identity ---
        public string Username { get; set; }          // Unique login name
        public string DisplayName { get; set; }       // Name shown in UI

        // --- Security ---
        public string PIN { get; set; }               // Login PIN (future hashing possible)
        public bool IsAdminUser { get; private set; } // Admin access flag

        public int FailedAttempts { get; set; } = 0;  // Failed login attempts
        public int MaxAttempts { get; set; } = 3;     // Lock threshold

        // Account lock rule
        public bool IsBlocked => FailedAttempts >= MaxAttempts;


        // Empty constructor required for JSON deserialization
        public User() { }

        // Normal constructor
        public User(string username, string displayName, string pin, bool admin = false)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name cannot be empty");

            if (string.IsNullOrWhiteSpace(pin) || pin.Length < 4)
                throw new ArgumentException("PIN must contain 4 digits");

            Username = username.Trim();
            DisplayName = displayName.Trim();
            PIN = pin.Trim();
            IsAdminUser = admin;
        }


        // Verify PIN login
        public bool ValidatePin(string input)
            => !IsBlocked && input == PIN;

        // Reset login attempt counter
        public void ResetAttempts()
            => FailedAttempts = 0;

        // Role management
        public void GrantAdmin()
            => IsAdminUser = true;

        public void RevokeAdmin()
            => IsAdminUser = false;

        public bool IsAdmin()
            => IsAdminUser;
    }
}
