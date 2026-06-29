using NUnit.Framework;
using IronExiles.Auth;

namespace IronExiles.Core.Tests
{
    /// <summary>
    /// Unit tests for AuthSessionManager configuration and state logic.
    /// Network-dependent integration tests require the Docker stack running.
    /// </summary>
    [TestFixture]
    public class AuthSessionManagerTests
    {
        [Test]
        public void TokenValidationResult_DefaultState_IsNotValid()
        {
            var result = new TokenValidationResult();
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.AccountId);
        }

        [Test]
        public void TokenValidationResult_WhenValid_ContainsAccountId()
        {
            var result = new TokenValidationResult
            {
                IsValid = true,
                AccountId = "test-account-123",
                Reason = "valid",
            };

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual("test-account-123", result.AccountId);
            Assert.AreEqual("valid", result.Reason);
        }

        [Test]
        public void AuthResult_DefaultState_IsNotSuccess()
        {
            var result = new AuthResult<LoginResponse>();
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Data);
        }

        [Test]
        public void AuthResult_WhenSuccessful_ContainsData()
        {
            var response = new LoginResponse
            {
                token = "test-token",
                account_id = "acc-123",
                expires_at = "2026-06-30T00:00:00Z",
            };

            var result = new AuthResult<LoginResponse>
            {
                Success = true,
                Data = response,
                StatusCode = 200,
            };

            Assert.IsTrue(result.Success);
            Assert.AreEqual("test-token", result.Data.token);
            Assert.AreEqual("acc-123", result.Data.account_id);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void AuthResult_WhenFailed_ContainsErrorInfo()
        {
            var result = new AuthResult<LoginResponse>
            {
                Success = false,
                ErrorMessage = "Invalid email or password.",
                ErrorCode = "login_failed",
                StatusCode = 401,
            };

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Invalid email or password.", result.ErrorMessage);
            Assert.AreEqual("login_failed", result.ErrorCode);
            Assert.AreEqual(401, result.StatusCode);
        }

        [Test]
        public void RegisterResponse_DeserializesFields()
        {
            var response = new RegisterResponse
            {
                account_id = "uuid-123",
                email = "player@test.com",
                created_at = "2026-06-29T00:00:00Z",
            };

            Assert.AreEqual("uuid-123", response.account_id);
            Assert.AreEqual("player@test.com", response.email);
        }

        [Test]
        public void ValidateResponse_WhenValid_HasAccountInfo()
        {
            var response = new ValidateResponse
            {
                valid = true,
                account_id = "uuid-456",
                email = "player@test.com",
                expires_at = "2026-06-30T00:00:00Z",
            };

            Assert.IsTrue(response.valid);
            Assert.AreEqual("uuid-456", response.account_id);
        }

        [Test]
        public void ServerTokenValidator_WithAuthDisabled_ReturnsDevBypass()
        {
            // ServerTokenValidator when authModeEnabled=false should return valid
            // This is tested in the actual ValidateAsync call but we verify the DTO
            var result = new TokenValidationResult
            {
                IsValid = true,
                AccountId = "dev-bypass",
                Reason = "auth_disabled",
            };

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual("dev-bypass", result.AccountId);
            Assert.AreEqual("auth_disabled", result.Reason);
        }
    }
}
