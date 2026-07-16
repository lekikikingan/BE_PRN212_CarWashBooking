/* ============================================================
   AutoWash / Car Wash Booking System - Database Schema
   Target: Microsoft SQL Server
   Generated from: Module_Feature_List_CarWash (Epics/US/AC), Module 1-6
   ============================================================ */

IF DB_ID('CarWashDB') IS NULL
BEGIN
    CREATE DATABASE CarWashDB;
END
GO

USE CarWashDB;
GO

/* ------------------------------------------------------------
   1. ROLES
   Lookup table for Users.RoleId (replaces the CUSTOMER/ADMIN string
   check constraint with a proper reference table).
   ------------------------------------------------------------ */
CREATE TABLE dbo.Roles (
    Id      INT IDENTITY(1,1)   NOT NULL PRIMARY KEY,
    Name    VARCHAR(20)         NOT NULL,

    CONSTRAINT UQ_Roles_Name UNIQUE (Name)
);
GO

INSERT INTO dbo.Roles (Name) VALUES ('CUSTOMER'), ('ADMIN');
GO

/* ------------------------------------------------------------
   2. USERS
   Covers: US-01 (Register), US-02 (Login), US-04 (Profile),
           US-19/US-20 (Admin views customers)
   ------------------------------------------------------------ */
CREATE TABLE dbo.Users (
    Id              INT IDENTITY(1,1)   NOT NULL PRIMARY KEY,
    FullName        NVARCHAR(100)       NOT NULL,
    Email           VARCHAR(255)        NOT NULL,
    PasswordHash    VARCHAR(255)        NOT NULL,
    Phone           VARCHAR(10)         NULL,
    RoleId          INT                 NOT NULL,
    TotalPoints     INT                 NOT NULL
        CONSTRAINT DF_Users_TotalPoints DEFAULT (0),
    LicensePlate    VARCHAR(20)         NULL,
    CreatedAt       DATETIME2           NOT NULL
        CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),

    -- AC1/AC5 US-01: email format validated at app layer; DB enforces uniqueness (AC3 US-01, AC3 US-04)
    CONSTRAINT UQ_Users_Email UNIQUE (Email),

    -- Role must reference Roles (CUSTOMER/ADMIN) (US-19 AC1 excludes ADMIN from customer list)
    -- ON DELETE NO ACTION (explicit): a Role must never be deleted while Users reference it
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId)
        REFERENCES dbo.Roles (Id)
        ON DELETE NO ACTION,

    -- users.total_points must never go negative (US-15 AC2/AC4, US-04 AC5)
    CONSTRAINT CK_Users_TotalPoints CHECK (TotalPoints >= 0),

    -- phone format: exactly 10 digits, starts with 0 (US-04 AC2) -- enforced again in app layer for exact message
    CONSTRAINT CK_Users_Phone CHECK (Phone IS NULL OR Phone LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
);
GO

CREATE INDEX IX_Users_RoleId ON dbo.Users (RoleId);
GO

/* ------------------------------------------------------------
   3. SESSIONS
   Covers: US-02 (Login creates session), US-03 (Logout deletes session)
   ------------------------------------------------------------ */
CREATE TABLE dbo.Sessions (
    SessionId       VARCHAR(64)         NOT NULL PRIMARY KEY,   -- value stored in "session_id" cookie
    UserId          INT                 NOT NULL,
    CreatedAt       DATETIME2           NOT NULL
        CONSTRAINT DF_Sessions_CreatedAt DEFAULT (SYSUTCDATETIME()),
    ExpiresAt       DATETIME2           NOT NULL,

    CONSTRAINT FK_Sessions_Users FOREIGN KEY (UserId)
        REFERENCES dbo.Users (Id)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_Sessions_UserId ON dbo.Sessions (UserId);
GO

/* ------------------------------------------------------------
   4. PACKAGES
   Covers: US-05 (Create), US-06 (List), US-07 (Edit), US-08 (Deactivate)
   ------------------------------------------------------------ */
CREATE TABLE dbo.Packages (
    Id              INT IDENTITY(1,1)   NOT NULL PRIMARY KEY,
    Name            NVARCHAR(100)       NOT NULL,
    Description     NVARCHAR(MAX)       NULL,
    Price           DECIMAL(12,2)       NOT NULL,
    RewardPoints    INT                 NOT NULL,
    IsActive        BIT                 NOT NULL
        CONSTRAINT DF_Packages_IsActive DEFAULT (1),

    -- Name required, not blank/whitespace-only (US-05 AC4)
    CONSTRAINT CK_Packages_Name CHECK (LTRIM(RTRIM(Name)) <> ''),

    -- Price must be > 0 (US-05 AC2, US-07 AC3)
    CONSTRAINT CK_Packages_Price CHECK (Price > 0),

    -- RewardPoints must be a non-negative integer (US-05 AC3, US-07 AC4)
    CONSTRAINT CK_Packages_RewardPoints CHECK (RewardPoints >= 0)
);
GO

-- Frequently filtered by is_active = true when listing packages for Customers (US-06 AC1, US-09 AC1)
CREATE INDEX IX_Packages_IsActive ON dbo.Packages (IsActive);
GO

/* ------------------------------------------------------------
   5. TIMESLOTS
   Lookup table for the 24 fixed 30-minute slots per day (08:00..19:30).
   Covers: US-10 AC3/AC8 (only these 24 fixed times are selectable).
   Replaces the CK_Bookings_TimeSlot check constraint with a proper
   reference table (same pattern as Roles), and lets availability be
   computed with a single JOIN instead of app-side set-difference logic.
   ------------------------------------------------------------ */
CREATE TABLE dbo.TimeSlots (
    Id          INT IDENTITY(1,1)   NOT NULL PRIMARY KEY,
    SlotTime    TIME(0)             NOT NULL,

    CONSTRAINT UQ_TimeSlots_SlotTime UNIQUE (SlotTime)
);
GO

-- Seed the 24 fixed slots: 08:00, 08:30, ..., 19:30
;WITH Seq AS (
    SELECT 0 AS n
    UNION ALL
    SELECT n + 1 FROM Seq WHERE n < 23
)
INSERT INTO dbo.TimeSlots (SlotTime)
SELECT CAST(DATEADD(MINUTE, n * 30, '08:00:00') AS TIME(0))
FROM Seq
OPTION (MAXRECURSION 24);
GO

/* ------------------------------------------------------------
   6. BOOKINGS
   Covers: US-09..US-13 (booking flow), US-14..US-17 (payment/points),
           US-21..US-23 (admin transactions)
   ------------------------------------------------------------ */
CREATE TABLE dbo.Bookings (
    Id                  INT IDENTITY(1,1)   NOT NULL PRIMARY KEY,
    CustomerId          INT                 NOT NULL,
    PackageId           INT                 NOT NULL,
    AppointmentDate     DATE                NOT NULL,
    TimeSlotId          INT                 NOT NULL,
    Status              VARCHAR(20)         NOT NULL
        CONSTRAINT DF_Bookings_Status DEFAULT ('PENDING_PAYMENT'),
    AmountPaid          DECIMAL(12,2)       NULL,       -- NULL until paid (US-11 AC1)
    PointsUsed          INT                 NOT NULL
        CONSTRAINT DF_Bookings_PointsUsed DEFAULT (0),
    PointsEarned        INT                 NOT NULL
        CONSTRAINT DF_Bookings_PointsEarned DEFAULT (0),
    CreatedAt           DATETIME2           NOT NULL
        CONSTRAINT DF_Bookings_CreatedAt DEFAULT (SYSUTCDATETIME()),
    PaidAt              DATETIME2           NULL,
    CancelledAt         DATETIME2           NULL,
    CompletedAt         DATETIME2           NULL,

    -- ON DELETE NO ACTION (explicit): a Customer with existing Bookings can never be hard-deleted
    CONSTRAINT FK_Bookings_Customer FOREIGN KEY (CustomerId)
        REFERENCES dbo.Users (Id)
        ON DELETE NO ACTION,

    -- ON DELETE NO ACTION (explicit): a Package with existing Bookings can never be hard-deleted
    -- (deactivation goes through Packages.IsActive instead, see US-08)
    CONSTRAINT FK_Bookings_Package FOREIGN KEY (PackageId)
        REFERENCES dbo.Packages (Id)
        ON DELETE NO ACTION,

    -- Appointment time must be one of the 24 fixed slots (US-10 AC3/AC8)
    -- ON DELETE NO ACTION (explicit): a TimeSlot referenced by Bookings can never be deleted
    CONSTRAINT FK_Bookings_TimeSlots FOREIGN KEY (TimeSlotId)
        REFERENCES dbo.TimeSlots (Id)
        ON DELETE NO ACTION,

    -- Booking lifecycle states (US-12 AC1 labels, US-13, US-16, US-23)
    CONSTRAINT CK_Bookings_Status CHECK (Status IN ('PENDING_PAYMENT', 'PAID', 'COMPLETED', 'CANCELLED')),

    -- NOTE: "only today/tomorrow/day-after" (US-10 AC1/AC2 + max 2-day booking window) is intentionally
    -- NOT a CHECK constraint here. A CHECK re-validates on every UPDATE, not just INSERT, using the
    -- current SYSUTCDATETIME() at that moment -- so a legitimate later UPDATE (e.g. Admin marks a
    -- booking Completed, or a Customer cancels) would get rejected once the appointment date has
    -- rolled outside the rolling window, even though the row was valid when created. This rule is a
    -- creation-time UX rule, not a standing data-integrity invariant, so it is validated in the
    -- application/service layer at booking-creation time (US-10/US-11) instead.

    -- points_used cannot be negative (US-15 AC4)
    CONSTRAINT CK_Bookings_PointsUsed CHECK (PointsUsed >= 0),
    CONSTRAINT CK_Bookings_PointsEarned CHECK (PointsEarned >= 0),

    -- amount_paid, if set, cannot be negative
    CONSTRAINT CK_Bookings_AmountPaid CHECK (AmountPaid IS NULL OR AmountPaid >= 0)
);
GO

-- One active (non-cancelled) booking per date/slot (US-10 AC3/AC6/AC7, US-11 AC2)
-- Filtered unique index: only PENDING_PAYMENT / PAID / COMPLETED occupy a slot; CANCELLED frees it.
CREATE UNIQUE INDEX UX_Bookings_ActiveSlot
    ON dbo.Bookings (AppointmentDate, TimeSlotId)
    WHERE Status IN ('PENDING_PAYMENT', 'PAID', 'COMPLETED');
GO

CREATE INDEX IX_Bookings_CustomerId ON dbo.Bookings (CustomerId);
CREATE INDEX IX_Bookings_PackageId ON dbo.Bookings (PackageId);
CREATE INDEX IX_Bookings_Status ON dbo.Bookings (Status);

-- No separate single-column index on TimeSlotId: UX_Bookings_ActiveSlot (AppointmentDate, TimeSlotId)
-- already covers TimeSlotId as its second key column for the queries that need it.
GO

/* ------------------------------------------------------------
   7. AVAILABILITY LOOKUP (US-10 AC3)
   Inline table-valued function: given a date, returns all 24 slots
   with their availability status, computed with a single JOIN
   instead of app-side set-difference logic.
   Usage: SELECT * FROM dbo.fn_SlotAvailability('2026-07-20');
   ------------------------------------------------------------ */
GO
CREATE FUNCTION dbo.fn_SlotAvailability (@AppointmentDate DATE)
RETURNS TABLE
AS
RETURN
(
    SELECT
        ts.Id          AS TimeSlotId,
        ts.SlotTime,
        CASE WHEN b.Id IS NULL THEN 'Available' ELSE 'Booked' END AS SlotStatus
    FROM dbo.TimeSlots ts
    LEFT JOIN dbo.Bookings b
        ON b.TimeSlotId = ts.Id
        AND b.AppointmentDate = @AppointmentDate
        AND b.Status IN ('PENDING_PAYMENT', 'PAID', 'COMPLETED')
);
GO
