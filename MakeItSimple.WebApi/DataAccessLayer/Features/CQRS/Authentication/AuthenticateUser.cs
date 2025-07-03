using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Authentication;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Channels;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Authentication
{
    public class AuthenticateUser
    {
        public class AuthenticateUserResult
        {
            public Guid Id { get; set; }

            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string UserRoleName { get; set; }
            public string ProfilePic { get; set; }
            public decimal FileSize { get; set; }
            public string FileName { get; set; }
            public ICollection<string> Permissions { get; set; }
            public string Token { get; set; }
            public bool? IsPasswordChanged { get; set; }
            public List<ChannelInfo> Channels { get; set; }
            public List<ServiceProviderInfo> ServiceProviders { get; set; }
 


            public AuthenticateUserResult(User user, string token, List<ChannelInfo> channels, List<ServiceProviderInfo> serviceProviders)
            {
                Id = user.Id;
                EmpId = user.EmpId;
                Fullname = user.Fullname;
                Username = user.Username;
                ProfilePic = user.ProfilePic;
                FileName = user.FileName;
                FileSize = user.FileSize.Value;
                UserRoleName = user.UserRole.UserRoleName;
                Permissions = user.UserRole?.Permissions;
                IsPasswordChanged = user.IsPasswordChange;
                Token = token;
                Channels = channels ?? new List<ChannelInfo>();
                ServiceProviders = serviceProviders ?? new List<ServiceProviderInfo>();
            }

        }

        public class AuthenticateUserQuery : IRequest<Result>
        {
            [Required]
            public string UsernameOrEmail { get; set; }
            [Required]
            public string Password { get; set; }

            public AuthenticateUserQuery(string usernameOrEmail)
            {
                UsernameOrEmail = usernameOrEmail;
            }

        }
        public class ChannelInfo
        {
            public int ChannelId { get; set; }
            public string ChannelName { get; set; }
        }

        public class ServiceProviderInfo
        {
            public int Id { get; set; }
            public string ServiceProviderName { get; set; }
        }

        public class Handler : IRequestHandler<AuthenticateUserQuery, Result>
        {

            private readonly MisDbContext _context;
            private readonly TokenGenerator _tokenGenerator;

            public Handler(MisDbContext context, TokenGenerator tokenGenerator)
            {
                _context = context;
                _tokenGenerator = tokenGenerator;
            }

            public async Task<Result> Handle(AuthenticateUserQuery command, CancellationToken cancellationToken)
            {


                var user = await _context.Users.Include(x => x.UserRole)
                    .Include(x => x.Company)
                    .Include(x => x.BusinessUnit)
                    .Include(x => x.Department)
                    .Include(x => x.Units)
                    .Include(x => x.SubUnit)
                    .Include(x => x.Location)
                    .Include(x => x.Channels)
                    .Include(x => x.SeviceProviders)//kk
                    .SingleOrDefaultAsync(x => x.Username == command.UsernameOrEmail);

                var userChannels = await _context.ChannelUsers
                    .Where(x => x.UserId == user.Id && x.IsActive == true)
                    .Join(_context.Channels.Where(x => x.IsActive == true), cu => cu.ChannelId, c => c.Id,
                        (cu, c) => new ChannelInfo
                        {
                            ChannelId = c.Id,
                            ChannelName = c.ChannelName
                        }).Distinct()
                    .ToListAsync();

                var channelName = userChannels.Select(x => x.ChannelName).ToList();

                var userServiceProvider = await _context.ServiceProviderChannels
                    .Where(x => channelName.Contains(x.Channel.ChannelName) && x.IsActive == true)
                    .Join(_context.ServiceProviders.Where(x => x.IsActive == true), x => x.ServiceProviderId, y => y.Id,
                    (x, y) => new ServiceProviderInfo
                    {
                       Id = y.Id,
                       ServiceProviderName = y.ServiceProviderName

                    }).Distinct().ToListAsync();


                if (user == null || !BCrypt.Net.BCrypt.Verify(command.Password, user.Password))
                {

                    return Result.Failure(AuthenticationError.UsernameAndPasswordIncorrect());
                }

                if (user.IsActive != true)
                {
                    return Result.Failure(AuthenticationError.UserNotActive());
                }


                if (user.UserRoleId == null)
                {
                    return Result.Failure(AuthenticationError.NoRole());
                }

                await _context.SaveChangesAsync(cancellationToken);

                var token = _tokenGenerator.GenerateJwtToken(user);

                var results = user.ToGetAuthenticatedUserResult(token, userChannels, userServiceProvider);



                return Result.Success(results);

            }

        }
    }

    public static class AuthenticateMappingExtension
    {
        public static AuthenticateUser.AuthenticateUserResult ToGetAuthenticatedUserResult(this User user, string token, List<AuthenticateUser.ChannelInfo> channels, List<AuthenticateUser.ServiceProviderInfo> serviceProviders)
        {
            return new AuthenticateUser.AuthenticateUserResult(user, token, channels, serviceProviders);
        }

    }

}
