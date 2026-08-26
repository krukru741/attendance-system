using BiometricAttendance.Core.Entities;

namespace BiometricAttendance.Core.Interfaces;

public interface IReferenceRepository
{
    Task<List<Department>> GetDepartmentsAsync(CancellationToken ct = default);
    Task<List<Position>> GetPositionsAsync(CancellationToken ct = default);
    Task<List<ShiftTemplate>> GetShiftTemplatesAsync(CancellationToken ct = default);
}
