/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Events;

namespace TeachingDeviceManagement.Services;

/// <summary>
/// Subscriber ghi lại lịch sử các lần chuyển trạng thái của thiết bị
/// </summary>
public class DeviceAuditLogger
{
    private readonly List<string> entries = new();
    public IReadOnlyList<string> Entries => entries.AsReadOnly();

    public void HandleStatusChanged(object? sender, DeviceStatusChangedEventArgs e)
    {
        entries.Add($"{e.ChangedAt}: {e.DeviceId}, {e.OldStatus} -> {e.NewStatus}");
    }
}
