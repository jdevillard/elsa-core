using Elsa.EntityFrameworkCore.Common.Contracts;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elsa.EntityFrameworkCore.SqlServer.Migrations.Management
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
                name: "DataCompressionAlgorithm",
                schema: _schema.Schema,
                table: "WorkflowInstances",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCompressionAlgorithm",
                schema: _schema.Schema,
                table: "WorkflowInstances");
        }
    }
}
