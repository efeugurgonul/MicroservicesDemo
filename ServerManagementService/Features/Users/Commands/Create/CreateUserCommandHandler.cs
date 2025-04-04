using Common.Core.CQRS.Commands;
using ServerManagementService.Data;
using ServerManagementService.Domain.Entities;
using ServerManagementService.Services;

namespace ServerManagementService.Features.Users.Commands.Create
{
    public class CreateUserCommandHandler : CommandHandler<CreateUserCommand, int>
    {
        private readonly ServerManDbContext _dbContext;
        private readonly IPasswordService _passwordService;

        public CreateUserCommandHandler(ServerManDbContext dbContext, IPasswordService passwordService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
        }

        public override async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Şifre hashleme
            _passwordService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                DefaultOrganizationId = request.DefaultOrganizationId,
                IsActive = request.IsActive
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Kullanıcıyı organizasyona ekle
            _dbContext.UserOrganizations.Add(new UserOrganization
            {
                UserId = user.Id,
                OrganizationId = request.DefaultOrganizationId
            });

            await _dbContext.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}