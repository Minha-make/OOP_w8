/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

namespace TeachingDeviceManagement.Events;

/// <summary>
/// Delegate dùng để lọc cảnh báo dựa theo điều kiện logic
/// </summary>
public delegate bool AlertFilter(DeviceAlertEventArgs alert);
