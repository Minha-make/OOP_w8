/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

namespace TeachingDeviceManagement.Enums;

/// <summary>
/// Trạng thái mượn / sử dụng của thiết bị giảng dạy
/// </summary>
public enum DeviceStatus
{
    Available,          // Thiết bị sẵn sàng sử dụng
    Borrowed,           // Thiết bị đang được mượn
    UnderMaintenance    // Thiết bị đang bảo trì
}
