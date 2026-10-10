using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;

namespace WiloNutrientSystem.UiTests.Infrastructure;

/// <summary>Convenience helpers for locating elements by automation id with retry.</summary>
public static class UiExtensions
{
    public static AutomationElement? WaitFor(this AutomationElement root, string automationId,
        int timeoutMs = 5000)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromMilliseconds(timeoutMs);
        while (DateTime.UtcNow < deadline)
        {
            var element = root.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
            if (element is not null)
            {
                return element;
            }

            Thread.Sleep(200);
        }

        return null;
    }

    public static Button GetButton(this AutomationElement root, string automationId)
    {
        var element = root.WaitFor(automationId)
            ?? throw new InvalidOperationException($"Button '{automationId}' not found.");
        return element.AsButton();
    }

    public static AutomationElement Require(this AutomationElement root, string automationId)
        => root.WaitFor(automationId)
           ?? throw new InvalidOperationException($"Element '{automationId}' not found.");

    public static void ClickById(this AutomationElement root, string automationId)
        => root.GetButton(automationId).Invoke();
}
