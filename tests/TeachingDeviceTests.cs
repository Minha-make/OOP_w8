/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Devices;
using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Events;
using Xunit;

namespace TeachingDeviceManagement.Tests;

/// <summary>
/// Lớp thiết bị cụ thể dùng cho việc kiểm thử TeachingDevice
/// </summary>
internal class TestDevice : TeachingDevice
{
    public TestDevice(string id, string name) : base(id, name)
    {
    }
}

public class TeachingDeviceTests
{
    [Fact]
    public void ChangeStatus_ShouldUpdateStatusAndRaiseEvent_WhenStatusIsDifferent()
    {
        // Arrange
        var device = new TestDevice("DEV-01", "Thiết bị thử nghiệm");
        DeviceStatusChangedEventArgs? receivedArgs = null;
        device.StatusChanged += (sender, e) => receivedArgs = e;

        // Act
        device.ChangeStatus(DeviceStatus.Borrowed);

        // Assert
        Assert.Equal(DeviceStatus.Borrowed, device.Status);
        Assert.NotNull(receivedArgs);
        Assert.Equal("DEV-01", receivedArgs.DeviceId);
        Assert.Equal(DeviceStatus.Available, receivedArgs.OldStatus);
        Assert.Equal(DeviceStatus.Borrowed, receivedArgs.NewStatus);
    }

    [Fact]
    public void ChangeStatus_ShouldNotRaiseEvent_WhenStatusIsSame()
    {
        // Arrange
        var device = new TestDevice("DEV-01", "Thiết bị thử nghiệm");
        bool eventFired = false;
        device.StatusChanged += (sender, e) => eventFired = true;

        // Act: Đổi sang cùng trạng thái Available hiện tại
        device.ChangeStatus(DeviceStatus.Available);

        // Assert
        Assert.False(eventFired);
        Assert.Equal(DeviceStatus.Available, device.Status);
    }
}
