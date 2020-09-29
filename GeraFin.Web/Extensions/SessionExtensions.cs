using System;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Web.Extensions
{
    public static class SessionExtensions
    {
        public static void SetInt(this ISession session, string key, int value) => session.Set(key, BitConverter.GetBytes(value));
    }
}
