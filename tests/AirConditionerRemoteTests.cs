/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Devices;
using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Events;
using Xunit;

namespace TeachingDeviceManagement.Tests;

public class AirConditionerRemoteTests
{
    [Fact]
    public void UpdateResponseState_ShouldRaiseCritical_WhenChangesFromResponsiveToUnresponsive()
    {
        // Arrange
        var ac = new AirConditionerRemote("AC-01", "Điều khiển điều hòa P101", "P101");
        DeviceAlertEventArgs? alertArgs = null;
        ac.AlertRaised += (sender, e) => alertArgs = e;

        // Act: Đang true chuyển sang false
        ac.UpdateResponseState(false);

        // Assert
        Assert.False(ac.IsResponsive);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Critical, alertArgs.Severity);
        Assert.Equal("AC-01", alertArgs.DeviceId);
    }

    [Fact]
    public void UpdateResponseState_ShouldNotRaiseAlert_WhenAlreadyUnresponsive()
    {
        // Arrange
        var ac = new AirConditionerRemote("AC-01", "Điều khiển điều hòa P101", "P101");
        ac.UpdateResponseState(false); // Lần 1: Chuyển sang không phản hồi

        int alertCount = 0;
        ac.AlertRaised += (sender, e) => alertCount++;

        // Act: Lần 2: Tiếp tục không phản hồi
        ac.UpdateResponseState(false);

        // Assert
        Assert.Equal(0, alertCount);
    }
}
