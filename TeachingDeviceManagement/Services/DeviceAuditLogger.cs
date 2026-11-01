/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Events;

namespace TeachingDeviceManagement.Services;

/// <summary>
/// Subscriber ghi lại lịch sử các lần chuyển đổi trạng thái của các thiết bị để phục vụ kiểm toán
/// </summary>
public class DeviceAuditLogger
{
    private readonly List<string> _entries = new();

    /// <summary>
    /// Danh sách chỉ đọc chứa các bản ghi lịch sử thay đổi trạng thái
    /// </summary>
    public IReadOnlyList<string> Entries => _entries.AsReadOnly();

    /// <summary>
    /// Xử lý sự kiện khi trạng thái thiết bị thay đổi và ghi nhận vào danh sách nhật ký
    /// </summary>
    /// <param name="sender">Thiết bị phát sinh sự kiện</param>
    /// <param name="e">Dữ liệu sự kiện chứa trạng thái cũ, mới và thời điểm thay đổi</param>
    public void HandleStatusChanged(object? sender, DeviceStatusChangedEventArgs e)
    {
        _entries.Add($"{e.ChangedAt}: {e.DeviceId}, {e.OldStatus} -> {e.NewStatus}");
    }
}
