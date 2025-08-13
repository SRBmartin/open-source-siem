using Microsoft.AspNetCore.Mvc;

namespace Siem.Platform.User.Api.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RequireBearerTokenAttribute() : TypeFilterAttribute(typeof(BearerTokenFilter)) { }
