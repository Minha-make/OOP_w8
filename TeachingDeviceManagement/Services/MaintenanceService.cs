/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Events;

namespace TeachingDeviceManagement.Services;

/// <summary>
/// Subscriber tự động tạo yêu cầu bảo trì khi nhận được cảnh báo mức nghiêm trọng (Critical)
/// </summary>
public class MaintenanceService
{
    private readonly List<string> requests = new();
    public IReadOnlyList<string> Requests => requests.AsReadOnly();

    public void HandleCriticalAlert(object? sender, DeviceAlertEventArgs e)
    {
        if (e.Severity != AlertSeverity.Critical)
        {
            return;
        }

        requests.Add($"Maintenance request for {e.DeviceId}: {e.Message}");
    }
}
