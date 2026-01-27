using System;
using System.Threading.Tasks;
using Windows.Security.Credentials.UI;

namespace FinalYearProject.MethodsToCallBackTo
{
    /// <summary>
    /// Handles Windows Hello authentication.
    /// Note: Can be extended to include student type checks (e.g., undergrad, postgrad).
    /// </summary>
    public static class AuthService
    {
        public static async Task<bool> AuthenticateAsync(string message = "Verify with Windows Hello")
        {
            var availability = await UserConsentVerifier.CheckAvailabilityAsync().AsTask();

            if (availability == UserConsentVerifierAvailability.Available)
            {
                var result = await UserConsentVerifier.RequestVerificationAsync(message).AsTask();
                return result == UserConsentVerificationResult.Verified;
            }

            return false;
        }
    }
}
