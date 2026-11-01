/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

namespace TeachingDeviceManagement.Enums;

/// <summary>
/// Mức độ nghiêm trọng của cảnh báo thiết bị
/// </summary>
public enum AlertSeverity
{
    Information,    // Mức thông tin thông thường
    Warning,        // Mức cảnh báo (pin yếu, tín hiệu yếu)
    Critical        // Mức nghiêm trọng (hết pin, mất tín hiệu hoàn toàn, mất phản hồi)
}
