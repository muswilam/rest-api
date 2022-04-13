
Packages:
	
    TodoApp:
        EFCore Sqlite: dotnet add package Microsoft.EntityFrameworkCore.Sqlite
        
        EFCore Tools: dotnet add package Microsoft.EntityFrameworkCore.Tools 

        JWT: dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

        Identity: dotnet add package Microsoft.AspNetCore.Identity.ENtityFrameworkCore

        Identity UI: dotnet add package Microsoft.AspNetCore.Identity.UI

    APIVersioing:
        Versioning: dotnet add package Microsoft.AspNetCore.Mvc.Versioning

Identity Error Codes Link:

	https://stackoverflow.com/questions/40583707/list-of-error-cases-in-use-usermanager-createasyncuser-password

Identity built-in Error Codes List:

    -DefaultError
    -DuplicateEmail
    -DuplicateName
    -ExternalLoginExists
    -InvalidEmail
    -InvalidToken
    -InvalidUserName
    -LockoutNotEnabled
    -NoTokenProvider
    -NoTwoFactorProvider
    -PasswordMismatch
    -PasswordRequireDigit
    -PasswordRequireLower
    -PasswordRequireNonLetterOrDigit
    -PasswordRequireUpper
    -PasswordTooShort
    -PropertyTooShort
    -RoleNotFound
    -StoreNotIQueryableRoleStore
    -StoreNotIQueryableUserStore
    -StoreNotIUserClaimStore
    -StoreNotIUserConfirmationStore
    -StoreNotIUserEmailStore
    -StoreNotIUserLockoutStore
    -StoreNotIUserLoginStore
    -StoreNotIUserPasswordStore
    -StoreNotIUserPhoneNumberStore
    -StoreNotIUserRoleStore
    -StoreNotIUserSecurityStampStore
    -StoreNotIUserTwoFactorStore
    -UserAlreadyHasPassword
    -UserAlreadyInRole
    -UserIdNotFound
    -UserNameNotFound
    -UserNotInRole

API Versioning:
    Evolving the API without breaking the clients who are using it.

API Versioning Types:
    1. Query String Versioning
        http://localhost:5000/api/users/1?api-version=1.0
    2. URI Versioning
        http://localhost:5000/api/2.0/users
    3. Media Type Versioning
        Accept = x-api-version=2.0
    4. Header Versioning
        Content-Type = application/json; x-api-version=2.0

API Versioning Tips:
    1. Install microsoft versining package.
    2. add ApiVersioning to ConfigureServices.
    3. Once we added the ApiVersioning to the ConfigureServices it will accept 
       api-version 1.0 by default in query string (if not passed it it will response with
       bad request ).