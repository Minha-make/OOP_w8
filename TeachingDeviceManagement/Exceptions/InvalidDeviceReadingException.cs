/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

namespace TeachingDeviceManagement.Exceptions;

/// <summary>
/// Ngoại lệ phát sinh khi thông số đọc từ thiết bị (như pin, chất lượng tín hiệu) nằm ngoài khoảng 0-100
/// </summary>
public class InvalidDeviceReadingException : Exception
{
    public string DeviceId { get; }
    public int InvalidValue { get; }

    public InvalidDeviceReadingException(string deviceId, int invalidValue)
        : base($"Giá trị đọc không hợp lệ '{invalidValue}' đối với thiết bị '{deviceId}'. Giá trị hợp lệ phải từ 0 đến 100.")
    {
        DeviceId = deviceId;
        InvalidValue = invalidValue;
    }

    public InvalidDeviceReadingException(string deviceId, int invalidValue, string message)
        : base(message)
    {
        DeviceId = deviceId;
        InvalidValue = invalidValue;
    }

    public InvalidDeviceReadingException(string deviceId, int invalidValue, string message, Exception innerException)
        : base(message, innerException)
    {
        DeviceId = deviceId;
        InvalidValue = invalidValue;
    }
}
