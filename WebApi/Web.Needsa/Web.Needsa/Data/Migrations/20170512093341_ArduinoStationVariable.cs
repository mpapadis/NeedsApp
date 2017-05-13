using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Needsa.Data.Migrations
{
    public partial class ArduinoStationVariable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ip",
                table: "ArduinoStations",
                newName: "Uri");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateCaptured",
                table: "ArduinoStationVariables",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<decimal>(
                name: "ValueCaptured",
                table: "ArduinoStationVariables",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCaptured",
                table: "ArduinoStationVariables");

            migrationBuilder.DropColumn(
                name: "ValueCaptured",
                table: "ArduinoStationVariables");

            migrationBuilder.RenameColumn(
                name: "Uri",
                table: "ArduinoStations",
                newName: "Ip");
        }
    }
}
