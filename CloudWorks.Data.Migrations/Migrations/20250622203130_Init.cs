using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudWorks.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "profiles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    identity_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sites",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sites", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "access_points",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_points", x => x.id);
                    table.ForeignKey(
                        name: "FK_access_points_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "booking",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking", x => x.id);
                    table.ForeignKey(
                        name: "FK_booking_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "site_profiles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_site_profiles_profiles_profile_id",
                        column: x => x.profile_id,
                        principalSchema: "public",
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_site_profiles_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "booking_access_points",
                schema: "public",
                columns: table => new
                {
                    AccessPointsId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_access_points", x => new { x.AccessPointsId, x.BookingId });
                    table.ForeignKey(
                        name: "FK_booking_access_points_access_points_AccessPointsId",
                        column: x => x.AccessPointsId,
                        principalSchema: "public",
                        principalTable: "access_points",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_booking_access_points_booking_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "public",
                        principalTable: "booking",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "schedules",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedules", x => x.id);
                    table.ForeignKey(
                        name: "FK_schedules_booking_booking_id",
                        column: x => x.booking_id,
                        principalSchema: "public",
                        principalTable: "booking",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_schedules_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "booking_site_profiles",
                schema: "public",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfilesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_site_profiles", x => new { x.BookingId, x.ProfilesId });
                    table.ForeignKey(
                        name: "FK_booking_site_profiles_booking_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "public",
                        principalTable: "booking",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_booking_site_profiles_site_profiles_ProfilesId",
                        column: x => x.ProfilesId,
                        principalSchema: "public",
                        principalTable: "site_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_access_points_site_id",
                schema: "public",
                table: "access_points",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_site_id",
                schema: "public",
                table: "booking",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_access_points_BookingId",
                schema: "public",
                table: "booking_access_points",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_booking_site_profiles_ProfilesId",
                schema: "public",
                table: "booking_site_profiles",
                column: "ProfilesId");

            migrationBuilder.CreateIndex(
                name: "IX_profiles_email",
                schema: "public",
                table: "profiles",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_schedules_booking_id",
                schema: "public",
                table: "schedules",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedules_site_id",
                schema: "public",
                table: "schedules",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_profiles_profile_id",
                schema: "public",
                table: "site_profiles",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_profiles_site_id",
                schema: "public",
                table: "site_profiles",
                column: "site_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "booking_access_points",
                schema: "public");

            migrationBuilder.DropTable(
                name: "booking_site_profiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "schedules",
                schema: "public");

            migrationBuilder.DropTable(
                name: "access_points",
                schema: "public");

            migrationBuilder.DropTable(
                name: "site_profiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "booking",
                schema: "public");

            migrationBuilder.DropTable(
                name: "profiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "sites",
                schema: "public");
        }
    }
}
