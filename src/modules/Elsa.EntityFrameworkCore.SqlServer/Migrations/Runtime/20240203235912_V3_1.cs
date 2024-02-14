using Elsa.EntityFrameworkCore.Common.Contracts;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elsa.EntityFrameworkCore.SqlServer.Migrations.Runtime
{
    /// <inheritdoc />
    public partial class V3_1 : Migration
    {
        private readonly IElsaDbContextSchema _schema;
        public V3_1(IElsaDbContextSchema schema)
        {
            _schema = schema ?? throw new ArgumentNullException(nameof(schema));
        }
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SerializedActivityStateCompressionAlgorithm",
                schema: _schema.Schema,
                table: "ActivityExecutionRecords",
                type: "nvarchar(max)",
                nullable: true);;
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerializedActivityStateCompressionAlgorithm",
                schema: _schema.Schema,
                table: "ActivityExecutionRecords");
        }
    }
}
