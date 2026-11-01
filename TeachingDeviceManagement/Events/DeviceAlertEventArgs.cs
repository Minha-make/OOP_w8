/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Enums;

namespace TeachingDeviceManagement.Events;

/// <summary>
/// Dữ liệu đi kèm sự kiện cảnh báo phát sinh từ thiết bị
/// </summary>
public class DeviceAlertEventArgs : EventArgs
{
    /// <summary>
    /// Mã thiết bị phát sinh cảnh báo
    /// </summary>
    public string DeviceId { get; }

    /// <summary>
    /// Nội dung chi tiết của cảnh báo
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Mức độ nghiêm trọng của cảnh báo (Information, Warning, Critical)
    /// </summary>
    public AlertSeverity Severity { get; }

    /// <summary>
    /// Thời điểm phát sinh cảnh báo
    /// </summary>
    public DateTime OccurredAt { get; }

    /// <summary>
    /// Khởi tạo đối tượng dữ liệu sự kiện cảnh báo thiết bị
    /// </summary>
    /// <param name="deviceId">Mã thiết bị</param>
    /// <param name="message">Thông điệp cảnh báo</param>
    /// <param name="severity">Mức độ nghiêm trọng</param>
    /// <param name="occurredAt">Thời điểm xảy ra</param>
    public DeviceAlertEventArgs(
        string deviceId,
        string message,
        AlertSeverity severity,
        DateTime occurredAt)
    {
        DeviceId = deviceId;
        Message = message;
        Severity = severity;
        OccurredAt = occurredAt;
    }
}
