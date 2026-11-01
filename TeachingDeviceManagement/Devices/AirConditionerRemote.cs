/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Enums;

namespace TeachingDeviceManagement.Devices;

/// <summary>
/// Lớp điều khiển điều hòa nhiệt độ trong phòng học
/// </summary>
public class AirConditionerRemote : TeachingDevice
{
    public string AssignedRoom { get; }
    public bool IsResponsive { get; private set; }

    public AirConditionerRemote(string id, string name, string assignedRoom)
        : base(id, name)
    {
        AssignedRoom = assignedRoom;
        IsResponsive = true; // Khởi tạo ban đầu thiết bị phản hồi bình thường
    }

    /// <summary>
    /// Cập nhật trạng thái phản hồi của điều hòa.
    /// - Khi chuyển từ phản hồi (true) sang không phản hồi (false): phát sinh cảnh báo Critical.
    /// - Không phát sinh lặp lại nếu trạng thái trước đó đã là không phản hồi.
    /// </summary>
    public void UpdateResponseState(bool isResponsive)
    {
        if (IsResponsive && !isResponsive)
        {
            IsResponsive = false;
            RaiseAlert($"Điều khiển điều hòa tại phòng {AssignedRoom} không phản hồi tín hiệu!", AlertSeverity.Critical);
        }
        else
        {
            IsResponsive = isResponsive;
        }
    }
}
