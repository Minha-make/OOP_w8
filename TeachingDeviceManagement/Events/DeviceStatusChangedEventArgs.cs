/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Enums;

namespace TeachingDeviceManagement.Events;

/// <summary>
/// Dữ liệu đi kèm sự kiện thay đổi trạng thái của thiết bị giảng dạy
/// </summary>
public class DeviceStatusChangedEventArgs : EventArgs
{
    /// <summary>
    /// Mã thiết bị xảy ra thay đổi trạng thái
    /// </summary>
    public string DeviceId { get; }

    /// <summary>
    /// Trạng thái cũ trước khi thay đổi
    /// </summary>
    public DeviceStatus OldStatus { get; }

    /// <summary>
    /// Trạng thái mới sau khi thay đổi
    /// </summary>
    public DeviceStatus NewStatus { get; }

    /// <summary>
    /// Thời điểm diễn ra sự kiện thay đổi trạng thái
    /// </summary>
    public DateTime ChangedAt { get; }

    /// <summary>
    /// Khởi tạo đối tượng dữ liệu sự kiện thay đổi trạng thái
    /// </summary>
    /// <param name="deviceId">Mã thiết bị</param>
    /// <param name="oldStatus">Trạng thái cũ</param>
    /// <param name="newStatus">Trạng thái mới</param>
    /// <param name="changedAt">Thời điểm thay đổi</param>
    public DeviceStatusChangedEventArgs(
        string deviceId,
        DeviceStatus oldStatus,
        DeviceStatus newStatus,
        DateTime changedAt)
    {
        DeviceId = deviceId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ChangedAt = changedAt;
    }
}
