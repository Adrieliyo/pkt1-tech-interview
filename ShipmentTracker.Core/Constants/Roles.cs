namespace ShipmentTracker.Core.Constants
{
    /// <summary>
    /// Nombres de rol de Identity. Los primeros cuatro coinciden textualmente con los miembros de
    /// <see cref="Enums.EmployeeRole"/> (un ApplicationUser no-SuperAdmin refleja siempre el rol de
    /// su Employee vinculado). SuperAdmin es el único rol independiente de EmployeeRole.
    /// </summary>
    public static class Roles
    {
        public const string BranchManager = "BranchManager";
        public const string Operator = "Operator";
        public const string Driver = "Driver";
        public const string WarehouseStaff = "WarehouseStaff";
        public const string SuperAdmin = "SuperAdmin";

        /// <summary>
        /// Los cuatro roles ligados a un Employee, en el mismo orden que <see cref="Enums.EmployeeRole"/>.
        /// </summary>
        public static readonly string[] EmployeeLinkedRoles = { Operator, Driver, WarehouseStaff, BranchManager };

        /// <summary>Los cinco roles de Identity que este sistema seedea al arrancar.</summary>
        public static readonly string[] All = { BranchManager, Operator, Driver, WarehouseStaff, SuperAdmin };
    }
}
