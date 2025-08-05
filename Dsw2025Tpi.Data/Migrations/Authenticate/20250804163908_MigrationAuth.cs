using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dsw2025Tpi.Data.Migrations.Authenticate
{
    /// <inheritdoc />
    public partial class MigrationAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosClaims_Usuarios_UserId",
                table: "UsuariosClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosLogins_Usuarios_UserId",
                table: "UsuariosLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosRoles_Roles_RoleId",
                table: "UsuariosRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosRoles_Usuarios_UserId",
                table: "UsuariosRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosTokens_Usuarios_UserId",
                table: "UsuariosTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuariosTokens",
                table: "UsuariosTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuariosRoles",
                table: "UsuariosRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuariosLogins",
                table: "UsuariosLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuariosClaims",
                table: "UsuariosClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.RenameTable(
                name: "UsuariosTokens",
                newName: "UsersTokens");

            migrationBuilder.RenameTable(
                name: "UsuariosRoles",
                newName: "UsersRoles");

            migrationBuilder.RenameTable(
                name: "UsuariosLogins",
                newName: "UsersLogins");

            migrationBuilder.RenameTable(
                name: "UsuariosClaims",
                newName: "UsersClaims");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_UsuariosRoles_RoleId",
                table: "UsersRoles",
                newName: "IX_UsersRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuariosLogins_UserId",
                table: "UsersLogins",
                newName: "IX_UsersLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuariosClaims_UserId",
                table: "UsersClaims",
                newName: "IX_UsersClaims_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersTokens",
                table: "UsersTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersRoles",
                table: "UsersRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersLogins",
                table: "UsersLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersClaims",
                table: "UsersClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersClaims_Users_UserId",
                table: "UsersClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersLogins_Users_UserId",
                table: "UsersLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersRoles_Roles_RoleId",
                table: "UsersRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersRoles_Users_UserId",
                table: "UsersRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTokens_Users_UserId",
                table: "UsersTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersClaims_Users_UserId",
                table: "UsersClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersLogins_Users_UserId",
                table: "UsersLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Roles_RoleId",
                table: "UsersRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Users_UserId",
                table: "UsersRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersTokens_Users_UserId",
                table: "UsersTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersTokens",
                table: "UsersTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersRoles",
                table: "UsersRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersLogins",
                table: "UsersLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersClaims",
                table: "UsersClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "UsersTokens",
                newName: "UsuariosTokens");

            migrationBuilder.RenameTable(
                name: "UsersRoles",
                newName: "UsuariosRoles");

            migrationBuilder.RenameTable(
                name: "UsersLogins",
                newName: "UsuariosLogins");

            migrationBuilder.RenameTable(
                name: "UsersClaims",
                newName: "UsuariosClaims");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Usuarios");

            migrationBuilder.RenameIndex(
                name: "IX_UsersRoles_RoleId",
                table: "UsuariosRoles",
                newName: "IX_UsuariosRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UsersLogins_UserId",
                table: "UsuariosLogins",
                newName: "IX_UsuariosLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UsersClaims_UserId",
                table: "UsuariosClaims",
                newName: "IX_UsuariosClaims_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuariosTokens",
                table: "UsuariosTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuariosRoles",
                table: "UsuariosRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuariosLogins",
                table: "UsuariosLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuariosClaims",
                table: "UsuariosClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosClaims_Usuarios_UserId",
                table: "UsuariosClaims",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosLogins_Usuarios_UserId",
                table: "UsuariosLogins",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosRoles_Roles_RoleId",
                table: "UsuariosRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosRoles_Usuarios_UserId",
                table: "UsuariosRoles",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosTokens_Usuarios_UserId",
                table: "UsuariosTokens",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
