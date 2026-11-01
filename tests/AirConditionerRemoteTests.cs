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

    [Fact]
    public void UpdateResponseState_FullTransitionSequence_ShouldRaiseCriticalOnlyOnResponsiveToUnresponsive()
    {
        // Sequence: true (ban đầu) -> false (báo) -> false (không lặp) -> true (bình thường) -> false (báo lần 2)
        var ac = new AirConditionerRemote("AC-01", "Điều khiển điều hòa", "P201");
        var alerts = new List<DeviceAlertEventArgs>();
        ac.AlertRaised += (sender, e) => alerts.Add(e);

        ac.UpdateResponseState(false); // 1. Critical
        ac.UpdateResponseState(false); // 2. Bỏ qua
        ac.UpdateResponseState(true);  // 3. Phục hồi
        ac.UpdateResponseState(false); // 4. Critical

        Assert.Equal(2, alerts.Count);
        Assert.All(alerts, a => Assert.Equal(AlertSeverity.Critical, a.Severity));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_NullOrWhitespaceRoom_ShouldThrowArgumentException(string? invalidRoom)
    {
        Assert.Throws<ArgumentException>(() => new AirConditionerRemote("AC-01", "Điều hòa", invalidRoom!));
    }
}
