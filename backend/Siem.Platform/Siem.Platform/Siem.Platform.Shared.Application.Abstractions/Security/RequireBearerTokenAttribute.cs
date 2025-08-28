using Microsoft.AspNetCore.Mvc;

namespace Siem.Platform.Shared.Application.Abstractions.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RequireBearerTokenAttribute() : TypeFilterAttribute(typeof(BearerTokenFilter)) { }
