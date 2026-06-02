using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

#nullable disable

namespace AlgebraWebShop2026.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAccount : Migration
    {
        const string ADMIN_USER_GUID = "0158d4a8-4234-4899-a27f-dc18f5b119b7";
        const string ADMIN_ROLE_GUID = "22fb09c7-4364-4f05-b25c-39d195feb094";
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            var passwordHash = hasher.HashPassword(null, "Password1234!");

            StringBuilder sb = new StringBuilder();

            sb.Append("INSERT INTO AspNetUsers(Id,userName,NormalizedUserName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount) ");
            sb.Append("Values(");
            sb.Append($"'{ADMIN_USER_GUID}'");
            sb.Append(",'admin@admin.com','ADMIN@ADMIN.COM','admin@admin.com',");
            sb.Append("1,");
            sb.Append($"'{passwordHash}','',");
            sb.Append("0, 0, 0, 0)");

            migrationBuilder.Sql(sb.ToString());

            migrationBuilder.Sql($"INSERT INTO AspNetRoles (Id,Name,NormalizedName) VALUES ('{ADMIN_ROLE_GUID}','Admin','ADMIN')");

            migrationBuilder.Sql($"INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES('{ADMIN_USER_GUID}','{ADMIN_ROLE_GUID}')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DELETE FROM AspNetUserRoles WHERE UserId='{ADMIN_USER_GUID}' AND RoleId='{ADMIN_ROLE_GUID}'");
            migrationBuilder.Sql($"DELETE FROM AspNetRoles WHERE Id='{ADMIN_ROLE_GUID}'");
            migrationBuilder.Sql($"DELETE FROM AspNetUsers WHERE Id='{ADMIN_USER_GUID}'");

        }
    }
}
