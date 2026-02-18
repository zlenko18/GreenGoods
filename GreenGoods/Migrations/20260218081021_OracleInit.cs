using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenBowl.Migrations
{
    /// <inheritdoc />
    public partial class OracleInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    UserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IngredientsInventories",
                columns: table => new
                {
                    IngredientLot = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Quantity = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IngredientName = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientsInventories", x => x.IngredientLot);
                });

            migrationBuilder.CreateTable(
                name: "ProductInventories",
                columns: table => new
                {
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ProductName = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    BatchWeight = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Pouch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CartonDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    QACheck = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    WeightRejected = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    XRayRejected = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SealingRejected = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NoPrint = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Sample = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    QCRetention = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TotalRejected = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProductForSale = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TotalCase = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ActualInventory = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Productivity = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInventories", x => x.Lot);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    RoleId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RoleId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InboundTrailers",
                columns: table => new
                {
                    IngredientLot = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    SupplierName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TruckPlate = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    QualityCheck = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboundTrailers", x => x.IngredientLot);
                    table.ForeignKey(
                        name: "FK_InboundTrailers_IngredientsInventories_IngredientLot",
                        column: x => x.IngredientLot,
                        principalTable: "IngredientsInventories",
                        principalColumn: "IngredientLot",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BatchingControlBatchingSteps",
                columns: table => new
                {
                    Batch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ProductName = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ProductionSupervisor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchingControlBatchingSteps", x => new { x.Batch, x.Lot });
                    table.ForeignKey(
                        name: "FK_BatchingControlBatchingSteps_ProductInventories_Lot",
                        column: x => x.Lot,
                        principalTable: "ProductInventories",
                        principalColumn: "Lot",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatchingControlEquipments",
                columns: table => new
                {
                    Batch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FillerSupervisor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchingControlEquipments", x => new { x.Batch, x.Lot });
                    table.ForeignKey(
                        name: "FK_BatchingControlEquipments_ProductInventories_Lot",
                        column: x => x.Lot,
                        principalTable: "ProductInventories",
                        principalColumn: "Lot",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatchingControlPackagings",
                columns: table => new
                {
                    Batch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FillerSupervisor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TypeOfPackaging = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchingControlPackagings", x => new { x.Batch, x.Lot });
                    table.ForeignKey(
                        name: "FK_BatchingControlPackagings_ProductInventories_Lot",
                        column: x => x.Lot,
                        principalTable: "ProductInventories",
                        principalColumn: "Lot",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatchingControlProcessings",
                columns: table => new
                {
                    Batch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ProductionSupervisor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchingControlProcessings", x => new { x.Batch, x.Lot });
                    table.ForeignKey(
                        name: "FK_BatchingControlProcessings_ProductInventories_Lot",
                        column: x => x.Lot,
                        principalTable: "ProductInventories",
                        principalColumn: "Lot",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BulgingTestOnPouches",
                columns: table => new
                {
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProductName = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumberOfPouch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TimeOfIncubation = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TemperatureOfIncubation = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Results = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Comments = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Taste = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateOfRelease = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    QAInitial = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TPC = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    YM = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BulgingTestOnPouches", x => x.Lot);
                    table.ForeignKey(
                        name: "FK_BulgingTestOnPouches_ProductInventories_Lot",
                        column: x => x.Lot,
                        principalTable: "ProductInventories",
                        principalColumn: "Lot",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProductName = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Client = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Address = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SellingDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Quantity = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Receipts_ProductInventories_Lot",
                        column: x => x.Lot,
                        principalTable: "ProductInventories",
                        principalColumn: "Lot",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XRayMonitoringRecords",
                columns: table => new
                {
                    Batch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProductName = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Indicator1 = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Indicator2 = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Indicator3 = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Deviation_CorrectiveAction = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XRayMonitoringRecords", x => new { x.Batch, x.Lot });
                    table.ForeignKey(
                        name: "FK_XRayMonitoringRecords_ProductInventories_Lot",
                        column: x => x.Lot,
                        principalTable: "ProductInventories",
                        principalColumn: "Lot",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatchBCBS",
                columns: table => new
                {
                    Batch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IngredientLot = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    IngredientName = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Allergens = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    HygenicCondition = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    AmountPerBatch = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Comments = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchBCBS", x => new { x.Batch, x.Lot, x.IngredientLot });
                    table.ForeignKey(
                        name: "FK_BatchBCBS_BatchingControlBatchingSteps_Batch_Lot",
                        columns: x => new { x.Batch, x.Lot },
                        principalTable: "BatchingControlBatchingSteps",
                        principalColumns: new[] { "Batch", "Lot" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchBCBS_IngredientsInventories_IngredientLot",
                        column: x => x.IngredientLot,
                        principalTable: "IngredientsInventories",
                        principalColumn: "IngredientLot",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BatchBCBS_ProductInventories_Lot",
                        column: x => x.Lot,
                        principalTable: "ProductInventories",
                        principalColumn: "Lot",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BatchBCEs",
                columns: table => new
                {
                    Batch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Equipments = table.Column<string>(type: "VARCHAR2(4000)", nullable: false),
                    CIP_COPBeforeStarting = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FunctionCondition = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    HygienicCondition = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Comments = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    InitialOfController = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchBCEs", x => new { x.Batch, x.Lot });
                    table.ForeignKey(
                        name: "FK_BatchBCEs_BatchingControlEquipments_Batch_Lot",
                        columns: x => new { x.Batch, x.Lot },
                        principalTable: "BatchingControlEquipments",
                        principalColumns: new[] { "Batch", "Lot" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchBCEs_ProductInventories_Lot",
                        column: x => x.Lot,
                        principalTable: "ProductInventories",
                        principalColumn: "Lot",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BatchBCPas",
                columns: table => new
                {
                    Batch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    StartTimeFilling = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FinishTimeFilling = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PouchHygienicCondition = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NumberOfPouchMade = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    WeightRejected = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SealingRejected = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NoPrintRejected = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Comments = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchBCPas", x => new { x.Batch, x.Lot });
                    table.ForeignKey(
                        name: "FK_BatchBCPas_BatchingControlPackagings_Batch_Lot",
                        columns: x => new { x.Batch, x.Lot },
                        principalTable: "BatchingControlPackagings",
                        principalColumns: new[] { "Batch", "Lot" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchBCPas_ProductInventories_Lot",
                        column: x => x.Lot,
                        principalTable: "ProductInventories",
                        principalColumn: "Lot",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BatchBCPrs",
                columns: table => new
                {
                    Batch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    StartTimeProcessing = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FinishTimeProcessing = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Alarm = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Reason = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CorrectionAction = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NumberOfRejectedPouch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Comments = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchBCPrs", x => new { x.Batch, x.Lot });
                    table.ForeignKey(
                        name: "FK_BatchBCPrs_BatchingControlProcessings_Batch_Lot",
                        columns: x => new { x.Batch, x.Lot },
                        principalTable: "BatchingControlProcessings",
                        principalColumns: new[] { "Batch", "Lot" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchBCPrs_ProductInventories_Lot",
                        column: x => x.Lot,
                        principalTable: "ProductInventories",
                        principalColumn: "Lot",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BCPaChecks",
                columns: table => new
                {
                    Batch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TimeOfCheck = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    SealingCondition = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCPaChecks", x => new { x.Batch, x.Lot, x.TimeOfCheck });
                    table.ForeignKey(
                        name: "FK_BCPaChecks_BatchBCPas_Batch_Lot",
                        columns: x => new { x.Batch, x.Lot },
                        principalTable: "BatchBCPas",
                        principalColumns: new[] { "Batch", "Lot" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BCPrChecks",
                columns: table => new
                {
                    Batch = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Lot = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TimeOfCheck = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    XRayRejectedPouches = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCPrChecks", x => new { x.Batch, x.Lot, x.TimeOfCheck });
                    table.ForeignKey(
                        name: "FK_BCPrChecks_BatchBCPrs_Batch_Lot",
                        columns: x => new { x.Batch, x.Lot },
                        principalTable: "BatchBCPrs",
                        principalColumns: new[] { "Batch", "Lot" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "\"NormalizedName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "\"NormalizedUserName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BatchBCBS_Batch_Lot_IngredientName",
                table: "BatchBCBS",
                columns: new[] { "Batch", "Lot", "IngredientName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BatchBCBS_IngredientLot",
                table: "BatchBCBS",
                column: "IngredientLot");

            migrationBuilder.CreateIndex(
                name: "IX_BatchBCBS_Lot",
                table: "BatchBCBS",
                column: "Lot");

            migrationBuilder.CreateIndex(
                name: "IX_BatchBCEs_Lot",
                table: "BatchBCEs",
                column: "Lot");

            migrationBuilder.CreateIndex(
                name: "IX_BatchBCPas_Lot",
                table: "BatchBCPas",
                column: "Lot");

            migrationBuilder.CreateIndex(
                name: "IX_BatchBCPrs_Lot",
                table: "BatchBCPrs",
                column: "Lot");

            migrationBuilder.CreateIndex(
                name: "IX_BatchingControlBatchingSteps_Lot",
                table: "BatchingControlBatchingSteps",
                column: "Lot");

            migrationBuilder.CreateIndex(
                name: "IX_BatchingControlEquipments_Lot",
                table: "BatchingControlEquipments",
                column: "Lot");

            migrationBuilder.CreateIndex(
                name: "IX_BatchingControlPackagings_Lot",
                table: "BatchingControlPackagings",
                column: "Lot");

            migrationBuilder.CreateIndex(
                name: "IX_BatchingControlProcessings_Lot",
                table: "BatchingControlProcessings",
                column: "Lot");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Lot",
                table: "Receipts",
                column: "Lot");

            migrationBuilder.CreateIndex(
                name: "IX_XRayMonitoringRecords_Lot",
                table: "XRayMonitoringRecords",
                column: "Lot");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BatchBCBS");

            migrationBuilder.DropTable(
                name: "BatchBCEs");

            migrationBuilder.DropTable(
                name: "BCPaChecks");

            migrationBuilder.DropTable(
                name: "BCPrChecks");

            migrationBuilder.DropTable(
                name: "BulgingTestOnPouches");

            migrationBuilder.DropTable(
                name: "InboundTrailers");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropTable(
                name: "XRayMonitoringRecords");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "BatchingControlBatchingSteps");

            migrationBuilder.DropTable(
                name: "BatchingControlEquipments");

            migrationBuilder.DropTable(
                name: "BatchBCPas");

            migrationBuilder.DropTable(
                name: "BatchBCPrs");

            migrationBuilder.DropTable(
                name: "IngredientsInventories");

            migrationBuilder.DropTable(
                name: "BatchingControlPackagings");

            migrationBuilder.DropTable(
                name: "BatchingControlProcessings");

            migrationBuilder.DropTable(
                name: "ProductInventories");
        }
    }
}
