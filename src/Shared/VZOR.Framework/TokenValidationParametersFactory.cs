﻿using System.Text;
using Microsoft.IdentityModel.Tokens;
using VZOR.Core.Options;

namespace VZOR.Framework;


public static class TokenValidationParametersFactory
{
    public static TokenValidationParameters CreateWithLifeTime(JwtOptions jwtOptions)
    {
        return new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }
    
    public static TokenValidationParameters CreateWithoutLifeTime(JwtOptions jwtOptions)
    {
        return new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };
    }
}