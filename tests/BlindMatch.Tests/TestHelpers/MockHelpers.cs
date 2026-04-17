using BlindMatch.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace BlindMatch.Tests.TestHelpers;

public static class MockHelpers
{
    public static Mock<UserManager<ApplicationUser>> CreateUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        store.Setup(s => s.GetUserIdAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(string.Empty);
        store.Setup(s => s.GetUserNameAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(string.Empty);

        return new Mock<UserManager<ApplicationUser>>(
            store.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<ApplicationUser>>().Object,
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new IdentityErrorDescriber(),
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
    }

    public static Mock<RoleManager<IdentityRole>> CreateRoleManager()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        return new Mock<RoleManager<IdentityRole>>(
            store.Object,
            Array.Empty<IRoleValidator<IdentityRole>>(),
            new Mock<ILookupNormalizer>().Object,
            new IdentityErrorDescriber(),
            new Mock<ILogger<RoleManager<IdentityRole>>>().Object);
    }

    public static ILogger<T> CreateNullLogger<T>() => NullLogger<T>.Instance;
}
