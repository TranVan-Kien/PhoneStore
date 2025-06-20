﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhoneStoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityToOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Không thay đổi cột Id vì đã tồn tại và không cần thêm IDENTITY
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Không cần rollback thay đổi
        }
    }
}