using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiveNewsLetters = table.Column<bool>(type: "bit", nullable: true),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employee_Countries_CountryID",
                        column: x => x.CountryID,
                        principalTable: "Countries",
                        principalColumn: "CountryId");
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "CountryName" },
                values: new object[] { new Guid("12e15727-d369-49a9-8b13-bc22e9362179"), "USA" });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "CountryName" },
                values: new object[] { new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"), "India" });

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "EmployeeId", "Address", "CountryID", "CountryName", "DateOfBirth", "Email", "EmployeeName", "Gender", "ReceiveNewsLetters" },
                values: new object[] { new Guid("12e15727-d369-49a9-8b13-bc22e9362179"), "Nagpur", new Guid("12e15727-d369-49a9-8b13-bc22e9362179"), "India", new DateTime(1996, 3, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "dilip@gmail.com", "Dilip", "Male", true });

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "EmployeeId", "Address", "CountryID", "CountryName", "DateOfBirth", "Email", "EmployeeName", "Gender", "ReceiveNewsLetters" },
                values: new object[] { new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"), "Pune", new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"), "India", new DateTime(1996, 3, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "rushikesh@gmail.com", "Rushikesh", "Male", true });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CountryID",
                table: "Employee",
                column: "CountryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
