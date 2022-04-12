using System.Collections.Generic;
using System.Security;
using Microsoft.AspNetCore.Identity;
using TodoApp.Dtos.Requests.UserManagement;
using TodoApp.Dtos.Responses.Base;
using TodoApp.ErrorCodes;

namespace TodoApp.Extensions
{
    public static class ErrorExtension
    {
        public static void AddValidationError<T>(this BaseResponseDto<T> response, string field, string errorCode, string message = null)
        {
            response.Errors.Add(new ErrorDto
            {
                IsValidationError = true,
                Field = field,
                Codes = new List<ErrorCode>
                {
                    new ErrorCode
                    {
                        Code = errorCode,
                        Message = message
                    }
                }
            });
        }

        public static void AddBusinessError<T>(this BaseResponseDto<T> response, string errorCode, string message = null)
        {
            response.Errors.Add(new ErrorDto
            {
                IsValidationError = false,
                Codes = new List<ErrorCode>
                {
                    new ErrorCode
                    {
                        Code = errorCode,
                        Message = message
                    }
                }
            });
        }

        public static void AddError<T>(this BaseResponseDto<T> response, ErrorDto error)
        {
            response.Errors.Add(error);
        }

        public static void ValidateIdentity<T>(this BaseResponseDto<T> respnse, IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                var identityError = new IdentityErrorDescriber();

                switch (error.Code)
                {
                    case nameof(identityError.PasswordRequiresDigit):
                        respnse.AddValidationError(nameof(UserBaseDto.Password), ValidationErrorCode.PasswordRequiresDigit);
                        break;
                    case nameof(identityError.PasswordRequiresLower):
                        respnse.AddValidationError(nameof(UserBaseDto.Password), ValidationErrorCode.PasswordRequiresLower);
                        break;
                    case nameof(identityError.PasswordRequiresNonAlphanumeric):
                        respnse.AddValidationError(nameof(UserBaseDto.Password), ValidationErrorCode.PasswordRequiresNonAlphanumeric);
                        break;
                    case nameof(identityError.PasswordRequiresUniqueChars):
                        respnse.AddValidationError(nameof(UserBaseDto.Password), ValidationErrorCode.PasswordRequiresUniqueChars);
                        break;
                    case nameof(identityError.PasswordRequiresUpper):
                        respnse.AddValidationError(nameof(UserBaseDto.Password), ValidationErrorCode.PasswordRequiresUpper);
                        break;
                    case nameof(identityError.PasswordTooShort):
                        respnse.AddValidationError(nameof(UserBaseDto.Password), ValidationErrorCode.PasswordTooShort);
                        break;
                    case nameof(identityError.UserNotInRole):
                        respnse.AddBusinessError(BusinessErrorCode.UserNotInRole);
                        break;
                    default:
                        respnse.AddBusinessError(BusinessErrorCode.UnKnownFail);
                        break;
                }
            }
        }
    }
}