using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudWorks.Data.Migrations.Migrations
{
    /// <inheritdoc />
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    public partial class addedbookingconfig : Migration
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_booking_site_profiles_site_profiles_ProfilesId",
                schema: "public",
                table: "booking_site_profiles");

            migrationBuilder.RenameColumn(
                name: "ProfilesId",
                schema: "public",
                table: "booking_site_profiles",
                newName: "SiteProfilesId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_site_profiles_ProfilesId",
                schema: "public",
                table: "booking_site_profiles",
                newName: "IX_booking_site_profiles_SiteProfilesId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "sites",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "public",
                table: "sites",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "sites",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                schema: "public",
                table: "sites",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "site_profiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "public",
                table: "site_profiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "site_profiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                schema: "public",
                table: "site_profiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "schedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "public",
                table: "schedules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "schedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                schema: "public",
                table: "schedules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "profiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "public",
                table: "profiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "profiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                schema: "public",
                table: "profiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "booking",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "public",
                table: "booking",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "booking",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                schema: "public",
                table: "booking",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "access_points",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "public",
                table: "access_points",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "public",
                table: "access_points",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                schema: "public",
                table: "access_points",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AccessPointHistories",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessPointId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessPointStatus = table.Column<string>(type: "text", nullable: true),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    SiteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPointHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessPointHistories_access_points_AccessPointId",
                        column: x => x.AccessPointId,
                        principalSchema: "public",
                        principalTable: "access_points",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessPointHistories_profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "public",
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessPointHistories_sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "public",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingAssessPoints",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessPointsId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingAssessPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingAssessPoints_access_points_AccessPointsId",
                        column: x => x.AccessPointsId,
                        principalSchema: "public",
                        principalTable: "access_points",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingAssessPoints_booking_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "public",
                        principalTable: "booking",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingSiteProfiles",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSiteProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingSiteProfiles_booking_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "public",
                        principalTable: "booking",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingSiteProfiles_profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "public",
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessPointHistories_AccessPointId",
                schema: "public",
                table: "AccessPointHistories",
                column: "AccessPointId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessPointHistories_ProfileId",
                schema: "public",
                table: "AccessPointHistories",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessPointHistories_SiteId",
                schema: "public",
                table: "AccessPointHistories",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingAssessPoints_AccessPointsId",
                schema: "public",
                table: "BookingAssessPoints",
                column: "AccessPointsId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingAssessPoints_BookingId",
                schema: "public",
                table: "BookingAssessPoints",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSiteProfiles_BookingId",
                schema: "public",
                table: "BookingSiteProfiles",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSiteProfiles_ProfileId",
                schema: "public",
                table: "BookingSiteProfiles",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_booking_site_profiles_site_profiles_SiteProfilesId",
                schema: "public",
                table: "booking_site_profiles",
                column: "SiteProfilesId",
                principalSchema: "public",
                principalTable: "site_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_booking_site_profiles_site_profiles_SiteProfilesId",
                schema: "public",
                table: "booking_site_profiles");

            migrationBuilder.DropTable(
                name: "AccessPointHistories",
                schema: "public");

            migrationBuilder.DropTable(
                name: "BookingAssessPoints",
                schema: "public");

            migrationBuilder.DropTable(
                name: "BookingSiteProfiles",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "sites");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "sites");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "sites");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "sites");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "site_profiles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "site_profiles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "site_profiles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "site_profiles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "profiles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "profiles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "profiles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "profiles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "access_points");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "access_points");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "access_points");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "access_points");

            migrationBuilder.RenameColumn(
                name: "SiteProfilesId",
                schema: "public",
                table: "booking_site_profiles",
                newName: "ProfilesId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_site_profiles_SiteProfilesId",
                schema: "public",
                table: "booking_site_profiles",
                newName: "IX_booking_site_profiles_ProfilesId");

            migrationBuilder.AddForeignKey(
                name: "FK_booking_site_profiles_site_profiles_ProfilesId",
                schema: "public",
                table: "booking_site_profiles",
                column: "ProfilesId",
                principalSchema: "public",
                principalTable: "site_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
