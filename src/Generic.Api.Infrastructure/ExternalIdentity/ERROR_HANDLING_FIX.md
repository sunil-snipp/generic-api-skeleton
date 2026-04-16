# Fix: Handling 401 Unauthorized Response in GetToken Method

## Problem

When wrong credentials were provided to the `GetToken` API endpoint, it was throwing an unhandled `System.Net.Http.HttpRequestException` with a 401 (Unauthorized) status code, instead of returning a proper HTTP 400 BadRequest response.

```
Error: System.Net.Http.HttpRequestException: 'Response status code does not indicate success: 401 (Unauthorized).'
```

---

## Root Cause

In `ExternalIdentityClient.GetTokenAsync()`:
- The method was calling `response.EnsureSuccessStatusCode()` directly without checking for specific HTTP status codes first
- When credentials were invalid, the external identity service returned HTTP 401
- `EnsureSuccessStatusCode()` throws `HttpRequestException` for any non-2xx status code
- This exception was not caught in the controller, so it propagated as an unhandled error

---

## Solution

### **Step 1: Update ExternalIdentityClient.GetTokenAsync()**

Added explicit handling for HTTP status codes before calling `EnsureSuccessStatusCode()`:

```csharp
public async Task<ExternalIdentityToken> GetTokenAsync(string username, string password, CancellationToken cancellationToken = default)
{
    // ... validation code ...

    var response = await httpClient.PostAsJsonAsync(tokenPath, payload, cancellationToken);
    
    // Handle authentication failures gracefully ✅
    if (response.StatusCode == HttpStatusCode.Unauthorized)
    {
        throw new AuthenticationException("Invalid username or password.");
    }

    if (response.StatusCode == HttpStatusCode.BadRequest)
    {
        throw new ArgumentException("Invalid request to external identity service.");
    }

    response.EnsureSuccessStatusCode();

    // ... rest of the method ...
}
```

**What Changed:**
- Added check for `HttpStatusCode.Unauthorized` (401) → throws `AuthenticationException`
- Added check for `HttpStatusCode.BadRequest` (400) → throws `ArgumentException`
- Only then calls `EnsureSuccessStatusCode()` for other potential errors

---

### **Step 2: Update AuthController.GetToken()**

Added specific exception handlers to catch authentication and validation errors:

```csharp
[HttpPost("token")]
[ProducesResponseType(typeof(ExternalIdentityToken), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<ActionResult<ExternalIdentityToken>> GetToken([FromBody] TokenRequest request, CancellationToken cancellationToken)
{
    logger.LogInformation("Token request initiated for username: {Username}", request.Username);

    try
    {
        var token = await externalIdentityClient.GetTokenAsync(request.Username, request.Password, cancellationToken);
        
        logger.LogInformation("Token successfully issued for username: {Username}", request.Username);
        return Ok(token);
    }
    catch (System.Security.Authentication.AuthenticationException ex)
    {
        logger.LogWarning("Authentication failed for username: {Username}, Reason: {Message}", request.Username, ex.Message);
        return BadRequest(new { message = "Invalid username or password." });
    }
    catch (ArgumentException ex)
    {
        logger.LogWarning("Bad request for token: {Message}", ex.Message);
        return BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        logger.LogError("Token issuance failed for username: {Username}", ex, request.Username);
        throw;
    }
}
```

**What Changed:**
- Added `catch` for `AuthenticationException` → returns HTTP 400 with user-friendly message
- Added `catch` for `ArgumentException` → returns HTTP 400 with validation message
- Added structured logging for each error type
- Other unexpected exceptions still bubble up (for debugging)

---

## Behavior After Fix

| Scenario | Before | After |
|----------|--------|-------|
| **Valid Credentials** | ✅ 200 OK with token | ✅ 200 OK with token |
| **Invalid Username/Password** | ❌ 500 Internal Server Error | ✅ 400 Bad Request with message |
| **Missing Username/Password** | ❌ 500 Internal Server Error | ✅ 400 Bad Request with message |
| **External Service Error** | ❌ 500 Internal Server Error | ✅ 500 Internal Server Error (logged) |

---

## Benefits

✅ **Proper HTTP Status Codes** - Returns 400 for client errors, not 500  
✅ **User-Friendly Messages** - "Invalid username or password." instead of raw exception  
✅ **Better Logging** - Warnings for authentication failures, errors for system issues  
✅ **Consistency** - Matches pattern already used in `GetProfileAsync()`  
✅ **Security** - Doesn't expose internal exception details to clients  

---

## Testing

### Test Case 1: Valid Credentials
```bash
curl -X POST http://localhost:5001/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"username":"validuser","password":"validpass"}'
```
**Expected:** 200 OK with token

### Test Case 2: Invalid Password
```bash
curl -X POST http://localhost:5001/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"username":"validuser","password":"wrongpass"}'
```
**Expected:** 400 Bad Request with message "Invalid username or password."

### Test Case 3: Missing Credentials
```bash
curl -X POST http://localhost:5001/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"username":"","password":""}'
```
**Expected:** 400 Bad Request with validation message

---

## Files Modified

1. **`src/Generic.Api.Infrastructure/ExternalIdentity/ExternalIdentityClient.cs`**
   - Added HTTP 401 and 400 status code handling in `GetTokenAsync()`

2. **`src/Generic.Api.Web/Controllers/AuthController.cs`**
   - Added exception handlers for `AuthenticationException` and `ArgumentException`
   - Improved logging for authentication failures
   - Returns proper HTTP 400 responses

---

## Build Status

✅ **Build: SUCCESS**
- No compilation errors
- All references resolved
- Ready for testing

---

## Summary

The fix follows the **"fail fast with meaningful exceptions"** pattern already used in `GetProfileAsync()`. Now both token and profile retrieval have consistent error handling:

1. Check for specific HTTP status codes
2. Throw meaningful exception types
3. Catch specific exceptions in controller
4. Return appropriate HTTP status codes
5. Log the details for monitoring

This ensures better user experience, proper HTTP semantics, and easier debugging! 🎉
