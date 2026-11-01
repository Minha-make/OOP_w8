/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Events;

namespace TeachingDeviceManagement.Services;

/// <summary>
/// Subscriber phụ trách tiếp nhận và tạo yêu cầu sửa chữa/bảo trì khẩn cấp.
/// Chỉ lọc và xử lý các cảnh báo ở mức nghiêm trọng (Critical).
/// </summary>
public class MaintenanceService
{
    private readonly List<string> _requests = new();

    /// <summary>
    /// Danh sách chỉ đọc chứa các yêu cầu bảo trì đã được hệ thống tạo
    /// </summary>
    public IReadOnlyList<string> Requests => _requests.AsReadOnly();

    /// <summary>
    /// Xử lý sự kiện cảnh báo từ thiết bị.
    /// Nếu mức độ là Critical thì tự động bổ sung yêu cầu bảo trì vào hệ thống.
    /// </summary>
    /// <param name="sender">Thiết bị gửi cảnh báo</param>
    /// <param name="e">Dữ liệu cảnh báo</param>
    public void HandleCriticalAlert(object? sender, DeviceAlertEventArgs e)
    {
        if (e.Severity != AlertSeverity.Critical)
        {
            return;
        }

        _requests.Add($"Maintenance request for {e.DeviceId}: {e.Message}");
    }
}
