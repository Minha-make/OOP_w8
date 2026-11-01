/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Events;

namespace TeachingDeviceManagement.Devices;

/// <summary>
/// Lớp trừu tượng định nghĩa các thuộc tính và hành vi chung cho các thiết bị phòng dạy
/// </summary>
public abstract class TeachingDevice
{
    public string Id { get; }
    public string Name { get; }
    public DeviceStatus Status { get; private set; }

    // Sự kiện khi trạng thái thiết bị thay đổi
    public event EventHandler<DeviceStatusChangedEventArgs>? StatusChanged;

    // Sự kiện khi thiết bị phát sinh cảnh báo
    public event EventHandler<DeviceAlertEventArgs>? AlertRaised;

    protected TeachingDevice(string id, string name)
    {
        Id = id;
        Name = name;
        Status = DeviceStatus.Available;
    }

    /// <summary>
    /// Thay đổi trạng thái thiết bị.
    /// Nếu trạng thái mới trùng với trạng thái hiện tại thì bỏ qua không phát event.
    /// Cập nhật Status trước khi kích hoạt event StatusChanged.
    /// </summary>
    public void ChangeStatus(DeviceStatus newStatus)
    {
        if (newStatus == Status)
        {
            return;
        }

        DeviceStatus oldStatus = Status;
        Status = newStatus;

        OnStatusChanged(new DeviceStatusChangedEventArgs(
            Id,
            oldStatus,
            newStatus,
            DateTime.Now));
    }

    /// <summary>
    /// Hàm tiện ích dùng trong nội bộ thiết bị để phát sinh cảnh báo
    /// </summary>
    protected void RaiseAlert(string message, AlertSeverity severity)
    {
        OnAlertRaised(new DeviceAlertEventArgs(
            Id,
            message,
            severity,
            DateTime.Now));
    }

    /// <summary>
    /// Phương thức kích hoạt sự kiện StatusChanged
    /// </summary>
    protected virtual void OnStatusChanged(DeviceStatusChangedEventArgs e)
    {
        StatusChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Phương thức kích hoạt sự kiện AlertRaised
    /// </summary>
    protected virtual void OnAlertRaised(DeviceAlertEventArgs e)
    {
        AlertRaised?.Invoke(this, e);
    }
}
