using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LoginService.Data.Entities;
using SFA.DAS.LoginService.Web.Infrastructure;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Web.UnitTests.ClaimsFactoryTests
{
    [TestFixture]
    public class When_user_authenticates
    {
        [Test, NSubstituteAutoData]
        public async Task Adds_claims(LoginUserClaimsPrincipalFactory sut, LoginUser user)
        {
            var claims = await sut.CreateAsync(user);
            claims.Should().NotBeNull();
        }

        //[Test, NSubstituteAutoData]
        //public async Task Adds_given_name(LoginUserClaimsPrincipalFactory sut, LoginUser user)
        //{
        //    var claims = await sut.CreateAsync(user);
        //    claims.Claims.Should().ContainEquivalentOf(new
        //    {
        //        Type = "given_name",
        //        Value = user.GivenName,
        //    });
        //}

        //[Test, NSubstituteAutoData]
        //public async Task Adds_family_name(LoginUserClaimsPrincipalFactory sut, LoginUser user)
        //{
        //    var claims = await sut.CreateAsync(user);
        //    claims.Claims.Should().ContainEquivalentOf(new
        //    {
        //        Type = "family_name",
        //        Value = user.FamilyName,
        //    });
        //}
    }

    internal class NSubstituteAutoDataAttribute : AutoDataAttribute
    {
        public NSubstituteAutoDataAttribute() : base(() =>
            new Fixture().Customize(
                new AutoNSubstituteCustomization { ConfigureMembers = true }))
        { }
    }
}